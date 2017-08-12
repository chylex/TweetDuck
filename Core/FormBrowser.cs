using CefSharp;
using CefSharp.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Notification.Screenshot;
using TweetDuck.Core.Other;
using TweetDuck.Core.Other.Management;
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

        public bool IsWaiting{
            set{
                if (value){
                    browser.Enabled = false;
                    Cursor = Cursors.WaitCursor;
                }
                else{
                    browser.Enabled = true;
                    Cursor = Cursors.Default;

                    if (Focused){ // re-focus browser only if the window or a child is activated
                        browser.Focus();
                    }
                }
            }
        }

        public string UpdateInstallerPath { get; private set; }

        private readonly ChromiumWebBrowser browser;
        private readonly PluginManager plugins;
        private readonly UpdateHandler updates;
        private readonly FormNotificationTweet notification;
        private readonly ContextMenu contextMenu;
        private readonly MemoryUsageTracker memoryUsageTracker;

        private bool isLoaded;
        private bool isBrowserReady;
        private FormWindowState prevState;

        private TweetScreenshotManager notificationScreenshotManager;
        private SoundNotification soundNotification;
        private VideoPlayer videoPlayer;

        public FormBrowser(UpdaterSettings updaterSettings){
            InitializeComponent();

            Text = Program.BrandName;

            this.plugins = new PluginManager(Program.PluginPath, Program.PluginConfigFilePath);
            this.plugins.Reloaded += plugins_Reloaded;
            this.plugins.Executed += plugins_Executed;
            this.plugins.PluginChangedState += plugins_PluginChangedState;
            this.plugins.Reload();

            this.contextMenu = ContextMenuBrowser.CreateMenu(this);
            this.memoryUsageTracker = new MemoryUsageTracker("TDGF_tryRunCleanup");

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
            
            browser.BrowserSettings.BackgroundColor = (uint)TwitterUtils.BackgroundColor.ToArgb();
            browser.Dock = DockStyle.None;
            browser.Location = ControlExtensions.InvisibleLocation;
            Controls.Add(browser);

            Controls.Add(new MenuStrip{ Visible = false }); // fixes Alt freezing the program in Win 10 Anniversary Update

            Disposed += (sender, args) => {
                memoryUsageTracker.Dispose();

                browser.Dispose();
                contextMenu.Dispose();

                notificationScreenshotManager?.Dispose();
                soundNotification?.Dispose();
                videoPlayer?.Dispose();
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
                foreach(string word in TwitterUtils.DictionaryWords){
                    browser.AddWordToDictionary(word);
                }

                BeginInvoke(new Action(OnBrowserReady));
                browser.LoadingStateChanged -= browser_LoadingStateChanged;
            }
        }

        private void browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e){
            if (e.Frame.IsMain){
                memoryUsageTracker.Stop();

                if (Config.ZoomLevel != 100){
                    BrowserUtils.SetZoomLevel(browser.GetBrowser(), Config.ZoomLevel);
                }

                if (TwitterUtils.IsTwitterWebsite(e.Frame)){
                    ScriptLoader.ExecuteFile(e.Frame, "twitter.js");
                }
            }
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && TwitterUtils.IsTweetDeckWebsite(e.Frame)){
                e.Frame.ExecuteJavaScriptAsync(TwitterUtils.BackgroundColorFix);

                UpdateProperties(PropertyBridge.Environment.Browser);
                TweetDeckBridge.RestoreSessionData(e.Frame);
                ScriptLoader.ExecuteFile(e.Frame, "code.js");
                ReinjectCustomCSS(Config.CustomBrowserCSS);
                plugins.ExecutePlugins(e.Frame, PluginEnvironment.Browser);

                TweetDeckBridge.ResetStaticProperties();

                if (Program.SystemConfig.EnableBrowserGCReload){
                    memoryUsageTracker.Start(this, e.Browser, Program.SystemConfig.BrowserMemoryThreshold);
                }
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

            if (!browser.Enabled){      // when taking a screenshot, the window is unfocused and
                browser.Enabled = true; // the browser is disabled; if the user clicks back into
            }                           // the window, enable the browser again
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
            UpdateProperties(PropertyBridge.Environment.Browser);
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
            if (e.HasErrors){
                FormMessage.Error("Error Loading Plugins", "The following plugins will not be available until the issues are resolved:\n\n"+string.Join("\n\n", e.Errors), FormMessage.OK);
            }

            if (isLoaded){
                ReloadToTweetDeck();
            }
        }
        
        private static void plugins_Executed(object sender, PluginErrorEventArgs e){
            if (e.HasErrors){
                FormMessage.Error("Error Executing Plugins", "Failed to execute the following plugins:\n\n"+string.Join("\n\n", e.Errors), FormMessage.OK);
            }
        }

        private void plugins_PluginChangedState(object sender, PluginChangedStateEventArgs e){
            browser.ExecuteScriptAsync("window.TDPF_setPluginState", e.Plugin, e.IsEnabled);
        }

        private void updates_UpdateAccepted(object sender, UpdateAcceptedEventArgs e){
            this.InvokeAsyncSafe(() => {
                FormManager.CloseAllDialogs();
            
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

            using(FormMessage form = new FormMessage("Notification Sound Error", "Could not play custom notification sound.\n"+e.Message, MessageBoxIcon.Error)){
                form.AddButton(FormMessage.Ignore, ControlType.Cancel | ControlType.Focused);

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
                if (m.Msg == Program.WindowRestoreMessage){
                    if (WindowsUtils.CurrentProcessID == m.WParam.ToInt32()){
                        trayIcon_ClickRestore(trayIcon, new EventArgs());
                    }

                    return;
                }
                else if (m.Msg == Program.SubProcessMessage){
                    int processId = m.WParam.ToInt32();

                    if (WindowsUtils.IsChildProcess(processId)){ // child process is checked in two places for safety
                        BrowserProcesses.Link(m.LParam.ToInt32(), processId);
                    }

                    return;
                }
                else if (m.Msg == Program.VideoPlayerMessage){
                    int volume = m.WParam.ToInt32();

                    if (Handle == m.LParam && volume != Config.VideoPlayerVolume){
                        Config.VideoPlayerVolume = volume;
                        Config.Save();
                    }

                    return;
                }
            }
            
            if (isBrowserReady && m.Msg == NativeMethods.WM_PARENTNOTIFY && (m.WParam.ToInt32() & 0xFFFF) == NativeMethods.WM_XBUTTONDOWN){
                if (videoPlayer != null && videoPlayer.Running){
                    videoPlayer.Close();
                }
                else{
                    browser.ExecuteScriptAsync("TDGF_onMouseClickExtra", (m.WParam.ToInt32() >> 16) & 0xFFFF);
                }

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

        public void UpdateProperties(PropertyBridge.Environment environment){
            browser.ExecuteScriptAsync(PropertyBridge.GenerateScript(environment));
        }

        public void ReloadToTweetDeck(){
            browser.ExecuteScriptAsync($"if(window.TDGF_reload)window.TDGF_reload();else window.location.href='{TwitterUtils.TweetDeckURL}'");
        }

        // callback handlers

        public void OpenContextMenu(){
            contextMenu.Show(this, PointToClient(Cursor.Position));
        }

        public void OpenSettings(){
            OpenSettings(null);
        }

        public void OpenSettings(Type startTab){
            if (!FormManager.TryBringToFront<FormSettings>()){
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

                    if (Program.SystemConfig.EnableBrowserGCReload){
                        memoryUsageTracker.Start(this, browser.GetBrowser(), Program.SystemConfig.BrowserMemoryThreshold);
                    }
                    else{
                        memoryUsageTracker.Stop();
                    }
                    
                    UpdateProperties(PropertyBridge.Environment.Browser);

                    notification.RequiresResize = true;
                    form.Dispose();
                };

                ShowChildForm(form);
            }
        }

        public void OpenAbout(){
            if (!FormManager.TryBringToFront<FormAbout>()){
                ShowChildForm(new FormAbout());
            }
        }

        public void OpenPlugins(){
            if (!FormManager.TryBringToFront<FormPlugins>()){
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

        public void PlayVideo(string url){
            if (string.IsNullOrEmpty(url)){
                videoPlayer?.Close();
                return;
            }

            if (videoPlayer == null){
                videoPlayer = new VideoPlayer(this);

                videoPlayer.ProcessExited += (sender, args) => {
                    browser.GetBrowser().GetHost().SendFocusEvent(true);
                    HideVideoOverlay();
                };
            }
            
            videoPlayer.Launch(url);
        }

        public void HideVideoOverlay(){
            browser.ExecuteScriptAsync("$('#td-video-player-overlay').remove()");
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
