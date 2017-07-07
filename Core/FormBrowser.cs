using CefSharp;
using CefSharp.WinForms;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Notification.Screenshot;
using TweetDuck.Core.Other;
using TweetDuck.Core.Other.Settings;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;
using TweetDuck.Plugins.Enums;
using TweetDuck.Plugins.Events;
using TweetDuck.Resources;
using TweetDuck.Updates;
using TweetDuck.Updates.Events;
using TweetLib.Audio;

namespace TweetDuck.Core{
    sealed partial class FormBrowser : Form{
        private static UserConfig Config => Program.UserConfig;

        public string UpdateInstallerPath { get; private set; }

        private readonly ChromiumWebBrowser browser;
        private readonly PluginManager plugins;
        private readonly UpdateHandler updates;
        private readonly FormNotificationTweet notification;
        private readonly ContextMenu contextMenu;

        private bool isLoaded;
        private bool isBrowserReady;
        private FormWindowState prevState;

        private TweetScreenshotManager notificationScreenshotManager;
        private SoundNotification soundNotification;

        public FormBrowser(PluginManager pluginManager, UpdaterSettings updaterSettings){
            InitializeComponent();

            Text = Program.BrandName;

            this.plugins = pluginManager;
            this.plugins.Reloaded += plugins_Reloaded;
            this.plugins.PluginChangedState += plugins_PluginChangedState;

            this.contextMenu = ContextMenuBrowser.CreateMenu(this);

            this.notification = new FormNotificationTweet(this, plugins){
                #if DEBUG
                CanMoveWindow = () => (ModifierKeys & Keys.Alt) == Keys.Alt
                #else
                CanMoveWindow = () => false
                #endif
            };

            this.notification.Show();

            this.browser = new ChromiumWebBrowser("https://tweetdeck.twitter.com/"){
                MenuHandler = new ContextMenuBrowser(this),
                JsDialogHandler = new JavaScriptDialogHandler(),
                LifeSpanHandler = new LifeSpanHandler(),
                RequestHandler = new RequestHandlerBrowser()
            };

            #if DEBUG
            this.browser.ConsoleMessage += BrowserUtils.HandleConsoleMessage;
            #endif

            this.browser.LoadingStateChanged += browser_LoadingStateChanged;
            this.browser.FrameLoadStart += browser_FrameLoadStart;
            this.browser.FrameLoadEnd += browser_FrameLoadEnd;
            this.browser.LoadError += browser_LoadError;
            this.browser.RegisterAsyncJsObject("$TD", new TweetDeckBridge(this, notification));
            this.browser.RegisterAsyncJsObject("$TDP", plugins.Bridge);
            
            browser.BrowserSettings.BackgroundColor = (uint)BrowserUtils.BackgroundColor.ToArgb();
            browser.Dock = DockStyle.None;
            browser.Location = ControlExtensions.InvisibleLocation;
            Controls.Add(browser);

            Controls.Add(new MenuStrip{ Visible = false }); // fixes Alt freezing the program in Win 10 Anniversary Update

            Disposed += (sender, args) => {
                browser.Dispose();
                contextMenu.Dispose();

                notificationScreenshotManager?.Dispose();
                soundNotification?.Dispose();
            };

            this.trayIcon.ClickRestore += trayIcon_ClickRestore;
            this.trayIcon.ClickClose += trayIcon_ClickClose;
            Config.TrayBehaviorChanged += Config_TrayBehaviorChanged;

            UpdateTrayIcon();

            Config.MuteToggled += Config_MuteToggled;
            Config.ZoomLevelChanged += Config_ZoomLevelChanged;

            this.updates = new UpdateHandler(browser, updaterSettings);
            this.updates.UpdateAccepted += updates_UpdateAccepted;
            this.updates.UpdateDismissed += updates_UpdateDismissed;

            RestoreWindow();
        }

        private bool TryBringToFront<T>() where T : Form{
            T form = Application.OpenForms.OfType<T>().FirstOrDefault();

            if (form != null){
                form.BringToFront();
                return true;
            }
            else return false;
        }

        private void ShowChildForm(Form form){
            form.VisibleChanged += (sender, args) => form.MoveToCenter(this);
            form.Show(this);
        }

        public void ForceClose(){
            trayIcon.Visible = false; // checked in FormClosing event
            Close();
        }

        // window setup

        private void RestoreWindow(){
            Config.BrowserWindow.Restore(this, true);
            prevState = WindowState;
            isLoaded = true;
        }

        private void OnBrowserReady(){
            if (!isBrowserReady){
                browser.Location = Point.Empty;
                browser.Dock = DockStyle.Fill;
                isBrowserReady = true;
            }
        }

        private void UpdateTrayIcon(){
            trayIcon.Visible = Config.TrayBehavior.ShouldDisplayIcon();
        }

        // active event handlers

        private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e){
            if (!e.IsLoading){
                foreach(string word in BrowserUtils.DictionaryWords){
                    browser.AddWordToDictionary(word);
                }

                BeginInvoke(new Action(OnBrowserReady));
                browser.LoadingStateChanged -= browser_LoadingStateChanged;
            }
        }

        private void browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e){
            if (e.Frame.IsMain){
                if (Config.ZoomLevel != 100){
                    BrowserUtils.SetZoomLevel(browser.GetBrowser(), Config.ZoomLevel);
                }

                if (BrowserUtils.IsTwitterWebsite(e.Frame)){
                    ScriptLoader.ExecuteFile(e.Frame, "twitter.js");
                }
            }
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && BrowserUtils.IsTweetDeckWebsite(e.Frame)){
                e.Frame.ExecuteJavaScriptAsync(BrowserUtils.BackgroundColorFix);

                UpdateProperties(PropertyBridge.Properties.AllBrowser);
                ScriptLoader.ExecuteFile(e.Frame, "code.js");
                ReinjectCustomCSS(Config.CustomBrowserCSS);

                if (plugins.HasAnyPlugin(PluginEnvironment.Browser)){
                    ScriptLoader.ExecuteFile(e.Frame, PluginManager.PluginBrowserScriptFile);
                    ScriptLoader.ExecuteFile(e.Frame, PluginManager.PluginGlobalScriptFile);
                    plugins.ExecutePlugins(e.Frame, PluginEnvironment.Browser, true);
                }

                TweetDeckBridge.ResetStaticProperties();
            }
        }

        private void browser_LoadError(object sender, LoadErrorEventArgs e){
            if (e.ErrorCode == CefErrorCode.Aborted){
                return;
            }

            if (!e.FailedUrl.StartsWith("http://td/", StringComparison.Ordinal)){
                string errorPage = ScriptLoader.LoadResource("pages/error.html", true);

                if (errorPage != null){
                    browser.LoadHtml(errorPage.Replace("{err}", BrowserUtils.GetErrorName(e.ErrorCode)), "http://td/error");
                }
            }
        }

        private void timerResize_Tick(object sender, EventArgs e){
            FormBrowser_ResizeEnd(this, e); // also stops timer
        }

        private void FormBrowser_Activated(object sender, EventArgs e){
            if (!isLoaded)return;

            trayIcon.HasNotifications = false;
        }

        private void FormBrowser_LocationChanged(object sender, EventArgs e){
            if (!isLoaded)return;
            
            timerResize.Stop();
            timerResize.Start();
        }

        private void FormBrowser_Resize(object sender, EventArgs e){
            if (!isLoaded)return;

            if (WindowState != prevState){
                prevState = WindowState;

                if (WindowState == FormWindowState.Minimized){
                    if (Config.TrayBehavior.ShouldHideOnMinimize()){
                        Hide(); // hides taskbar too?! welp that works I guess
                    }
                }
                else{
                    FormBrowser_ResizeEnd(sender, e);
                }
            }
            else{
                timerResize.Stop();
                timerResize.Start();
            }
        }

        private void FormBrowser_ResizeEnd(object sender, EventArgs e){ // also triggers when the window moves
            if (!isLoaded)return;

            timerResize.Stop();
            
            if (Location != ControlExtensions.InvisibleLocation){
                Config.BrowserWindow.Save(this);
                Config.Save();
            }
        }

        private void FormBrowser_FormClosing(object sender, FormClosingEventArgs e){
            if (!isLoaded)return;

            if (Config.TrayBehavior.ShouldHideOnClose() && trayIcon.Visible && e.CloseReason == CloseReason.UserClosing){
                Hide(); // hides taskbar too?! welp that works I guess
                e.Cancel = true;
            }
        }

        private void FormBrowser_FormClosed(object sender, FormClosedEventArgs e){
            if (isLoaded && UpdateInstallerPath == null){
                updates.CleanupDownload();
            }
        }

        private void Config_MuteToggled(object sender, EventArgs e){
            UpdateProperties(PropertyBridge.Properties.MuteNotifications);
        }

        private void Config_ZoomLevelChanged(object sender, EventArgs e){
            BrowserUtils.SetZoomLevel(browser.GetBrowser(), Config.ZoomLevel);
        }

        private void Config_TrayBehaviorChanged(object sender, EventArgs e){
            UpdateTrayIcon();
        }

        private void trayIcon_ClickRestore(object sender, EventArgs e){
            Show();
            RestoreWindow();
            Activate();
            UpdateTrayIcon();
        }

        private void trayIcon_ClickClose(object sender, EventArgs e){
            ForceClose();
        }
        
        private void plugins_Reloaded(object sender, PluginErrorEventArgs e){
            browser.GetBrowser().Reload();
        }

        private void plugins_PluginChangedState(object sender, PluginChangedStateEventArgs e){
            browser.ExecuteScriptAsync("window.TDPF_setPluginState", e.Plugin, e.IsEnabled);
        }

        private void updates_UpdateAccepted(object sender, UpdateAcceptedEventArgs e){
            this.InvokeAsyncSafe(() => {
                foreach(Form form in Application.OpenForms.Cast<Form>().Reverse()){
                    if (form is FormSettings || form is FormPlugins || form is FormAbout){
                        form.Close();
                    }
                }
            
                updates.BeginUpdateDownload(this, e.UpdateInfo, update => {
                    if (update.DownloadStatus == UpdateDownloadStatus.Done){
                        UpdateInstallerPath = update.InstallerPath;
                    }

                    ForceClose();
                });
            });
        }

        private void updates_UpdateDismissed(object sender, UpdateDismissedEventArgs e){
            this.InvokeAsyncSafe(() => {
                Config.DismissedUpdate = e.VersionTag;
                Config.Save();
            });
        }

        private void soundNotification_PlaybackError(object sender, PlaybackErrorEventArgs e){
            e.Ignore = true;

            using(FormMessage form = new FormMessage("Notification Sound Error", "Could not play custom notification sound."+Environment.NewLine+e.Message, MessageBoxIcon.Error)){
                form.CancelButton = form.AddButton("Ignore");

                Button btnOpenSettings = form.AddButton("View Options");
                btnOpenSettings.Width += 16;
                btnOpenSettings.Location = new Point(btnOpenSettings.Location.X-16, btnOpenSettings.Location.Y);

                if (form.ShowDialog() == DialogResult.OK && form.ClickedButton == btnOpenSettings){
                    OpenSettings(typeof(TabSettingsSounds));
                }
            }
        }

        protected override void WndProc(ref Message m){
            if (isLoaded){
                if (m.Msg == Program.WindowRestoreMessage && WindowsUtils.CurrentProcessID == m.WParam.ToInt32()){
                    trayIcon_ClickRestore(trayIcon, new EventArgs());
                    return;
                }
            }
            
            if (isBrowserReady && m.Msg == NativeMethods.WM_PARENTNOTIFY && (m.WParam.ToInt32() & 0xFFFF) == NativeMethods.WM_XBUTTONDOWN){
                browser.ExecuteScriptAsync("TDGF_onMouseClickExtra", (m.WParam.ToInt32() >> 16) & 0xFFFF);
                return;
            }

            base.WndProc(ref m);
        }

        // notification helpers

        public FormNotificationMain CreateNotificationForm(bool enableContextMenu){
            return new FormNotificationMain(this, plugins, enableContextMenu);
        }

        public void PauseNotification(){
            notification.PauseNotification();
        }

        public void ResumeNotification(){
            notification.ResumeNotification();
        }

        // javascript calls

        public void ReinjectCustomCSS(string css){
            browser.ExecuteScriptAsync("TDGF_reinjectCustomCSS", css?.Replace(Environment.NewLine, " ") ?? string.Empty);
        }

        public void UpdateProperties(PropertyBridge.Properties properties){
            browser.ExecuteScriptAsync(PropertyBridge.GenerateScript(properties));
        }

        public void ReloadToTweetDeck(){
            browser.ExecuteScriptAsync($"window.location.href = '{BrowserUtils.TweetDeckURL}'");
        }

        // callback handlers

        public void OpenContextMenu(){
            contextMenu.Show(this, PointToClient(Cursor.Position));
        }

        public void OpenSettings(){
            OpenSettings(null);
        }

        public void OpenSettings(Type startTab){
            if (!TryBringToFront<FormSettings>()){
                bool prevEnableUpdateCheck = Config.EnableUpdateCheck;

                FormSettings form = new FormSettings(this, plugins, updates, startTab);

                form.FormClosed += (sender, args) => {
                    if (!prevEnableUpdateCheck && Config.EnableUpdateCheck){
                        updates.DismissUpdate(string.Empty);
                        updates.Check(false);
                    }

                    if (!Config.EnableTrayHighlight){
                        trayIcon.HasNotifications = false;
                    }
                    
                    UpdateProperties(PropertyBridge.Properties.ExpandLinksOnHover | PropertyBridge.Properties.SwitchAccountSelectors | PropertyBridge.Properties.HasCustomNotificationSound);

                    notification.RequiresResize = true;
                    form.Dispose();
                };

                ShowChildForm(form);
            }
        }

        public void OpenAbout(){
            if (!TryBringToFront<FormAbout>()){
                ShowChildForm(new FormAbout());
            }
        }

        public void OpenPlugins(){
            if (!TryBringToFront<FormPlugins>()){
                ShowChildForm(new FormPlugins(plugins));
            }
        }

        public void OnTweetNotification(){ // may be called multiple times, once for each type of notification
            if (Config.EnableTrayHighlight && !ContainsFocus){
                trayIcon.HasNotifications = true;
            }
        }

        public void PlayNotificationSound(){
            if (Config.NotificationSoundPath.Length == 0){
                return;
            }

            if (soundNotification == null){
                soundNotification = new SoundNotification();
                soundNotification.PlaybackError += soundNotification_PlaybackError;
            }

            soundNotification.Play(Config.NotificationSoundPath);
        }

        public void OnTweetScreenshotReady(string html, int width, int height){
            if (notificationScreenshotManager == null){
                notificationScreenshotManager = new TweetScreenshotManager(this, plugins);
            }

            notificationScreenshotManager.Trigger(html, width, height);
        }

        public void DisplayTooltip(string text){
            if (string.IsNullOrEmpty(text)){
                toolTip.Hide(this);
            }
            else{
                Point position = PointToClient(Cursor.Position);
                position.Offset(20, 10);
                toolTip.Show(text, this, position);
            }
        }

        public void TriggerTweetScreenshot(){
            browser.ExecuteScriptAsync("TDGF_triggerScreenshot()");
        }
    }
}
