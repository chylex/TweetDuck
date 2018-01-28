using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Management;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Notification.Screenshot;
using TweetDuck.Core.Other;
using TweetDuck.Core.Other.Analytics;
using TweetDuck.Core.Other.Settings;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;
using TweetDuck.Plugins.Events;
using TweetDuck.Updates;

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

        private readonly TweetDeckBrowser browser;
        private readonly PluginManager plugins;
        private readonly UpdateHandler updates;
        private readonly FormNotificationTweet notification;
        private readonly ContextMenu contextMenu;

        private bool isLoaded;
        private FormWindowState prevState;
        
        private TweetScreenshotManager notificationScreenshotManager;
        private VideoPlayer videoPlayer;
        private AnalyticsManager analytics;

        public FormBrowser(UpdaterSettings updaterSettings){
            InitializeComponent();

            Text = Program.BrandName;

            this.plugins = new PluginManager(Program.PluginPath, Program.PluginConfigFilePath);
            this.plugins.Reloaded += plugins_Reloaded;
            this.plugins.Executed += plugins_Executed;
            this.plugins.Reload();

            this.notification = new FormNotificationTweet(this, plugins);
            this.notification.Show();
            
            this.browser = new TweetDeckBrowser(this, plugins, new TweetDeckBridge.Browser(this, notification));
            this.contextMenu = ContextMenuBrowser.CreateMenu(this);

            Controls.Add(new MenuStrip{ Visible = false }); // fixes Alt freezing the program in Win 10 Anniversary Update

            Disposed += (sender, args) => {
                Config.MuteToggled -= Config_MuteToggled;
                Config.TrayBehaviorChanged -= Config_TrayBehaviorChanged;

                browser.Dispose();
                contextMenu.Dispose();

                notificationScreenshotManager?.Dispose();
                videoPlayer?.Dispose();
                analytics?.Dispose();
            };

            Config.MuteToggled += Config_MuteToggled;

            this.trayIcon.ClickRestore += trayIcon_ClickRestore;
            this.trayIcon.ClickClose += trayIcon_ClickClose;
            Config.TrayBehaviorChanged += Config_TrayBehaviorChanged;

            UpdateTrayIcon();

            this.updates = browser.CreateUpdateHandler(updaterSettings);
            this.updates.UpdateAccepted += updates_UpdateAccepted;
            this.updates.UpdateDismissed += updates_UpdateDismissed;

            if (Config.AllowDataCollection){
                analytics = new AnalyticsManager(this, plugins, Program.AnalyticsFilePath);
            }

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

        private void UpdateTrayIcon(){
            trayIcon.Visible = Config.TrayBehavior.ShouldDisplayIcon();
        }

        // event handlers

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
            TriggerAnalyticsEvent(AnalyticsFile.Event.MuteNotification);
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

        private void updates_UpdateAccepted(object sender, UpdateEventArgs e){
            this.InvokeAsyncSafe(() => {
                FormManager.CloseAllDialogs();

                if (!string.IsNullOrEmpty(Config.DismissedUpdate)){
                    Config.DismissedUpdate = null;
                    Config.Save();
                }
            
                updates.BeginUpdateDownload(this, e.UpdateInfo, update => {
                    if (update.DownloadStatus == UpdateDownloadStatus.Done){
                        UpdateInstallerPath = update.InstallerPath;
                    }

                    ForceClose();
                });
            });
        }

        private void updates_UpdateDismissed(object sender, UpdateEventArgs e){
            this.InvokeAsyncSafe(() => {
                Config.DismissedUpdate = e.UpdateInfo.VersionTag;
                Config.Save();
            });
        }

        protected override void WndProc(ref Message m){
            if (isLoaded && m.Msg == Program.WindowRestoreMessage){
                if (WindowsUtils.CurrentProcessID == m.WParam.ToInt32()){
                    trayIcon_ClickRestore(trayIcon, EventArgs.Empty);
                }

                return;
            }
            
            if (browser.Ready && m.Msg == NativeMethods.WM_PARENTNOTIFY && (m.WParam.ToInt32() & 0xFFFF) == NativeMethods.WM_XBUTTONDOWN){
                if (videoPlayer != null && videoPlayer.Running){
                    videoPlayer.Close();
                }
                else{
                    browser.OnMouseClickExtra(m.WParam);
                    TriggerAnalyticsEvent(AnalyticsFile.Event.BrowserExtraMouseButton);
                }

                return;
            }

            base.WndProc(ref m);
        }

        // bridge methods

        public void PauseNotification(){
            notification.PauseNotification();
        }

        public void ResumeNotification(){
            notification.ResumeNotification();
        }
        
        public void ReinjectCustomCSS(string css){
            browser.ReinjectCustomCSS(css);
        }

        public void ReloadToTweetDeck(){
            browser.ReloadToTweetDeck();
        }

        public void TriggerTweetScreenshot(){
            browser.TriggerTweetScreenshot();
        }

        public void ReloadColumns(){
            browser.ReloadColumns();
        }

        public void PlaySoundNotification(){
            browser.PlaySoundNotification();
        }

        public void ApplyROT13(){
            browser.ApplyROT13();
        }

        public void TriggerAnalyticsEvent(AnalyticsFile.Event e){
            analytics?.TriggerEvent(e);
        }

        // callback handlers
        
        public void OnIntroductionClosed(bool showGuide, bool allowDataCollection){
            if (Config.FirstRun){
                Config.FirstRun = false;
                Config.AllowDataCollection = allowDataCollection;
                Config.Save();

                if (allowDataCollection && analytics == null){
                    analytics = new AnalyticsManager(this, plugins, Program.AnalyticsFilePath);
                }
            }

            if (showGuide){
                FormGuide.Show();
            }
        }

        public void OpenContextMenu(){
            contextMenu.Show(this, PointToClient(Cursor.Position));
        }

        public void OpenSettings(){
            OpenSettings(null);
        }

        public void OpenSettings(Type startTab){
            if (!FormManager.TryBringToFront<FormSettings>()){
                bool prevEnableUpdateCheck = Config.EnableUpdateCheck;

                FormSettings form = new FormSettings(this, plugins, updates, analytics, startTab);

                form.FormClosed += (sender, args) => {
                    if (!prevEnableUpdateCheck && Config.EnableUpdateCheck){
                        Config.DismissedUpdate = null;
                        Config.Save();
                        
                        updates.Check(true);
                    }

                    if (!Config.EnableTrayHighlight){
                        trayIcon.HasNotifications = false;
                    }

                    if (Config.AllowDataCollection){
                        if (analytics == null){
                            analytics = new AnalyticsManager(this, plugins, Program.AnalyticsFilePath);
                        }
                    }
                    else if (analytics != null){
                        analytics.Dispose();
                        analytics = null;
                    }

                    BrowserCache.RefreshTimer();
                    
                    if (form.ShouldReloadBrowser){
                        FormManager.TryFind<FormPlugins>()?.Close();
                        plugins.Reload(); // also reloads the browser
                    }
                    else{
                        browser.UpdateProperties();
                    }

                    notification.RequiresResize = true;
                    form.Dispose();
                };
                
                TriggerAnalyticsEvent(AnalyticsFile.Event.OpenOptions);
                ShowChildForm(form);
            }
        }

        public void OpenAbout(){
            if (!FormManager.TryBringToFront<FormAbout>()){
                TriggerAnalyticsEvent(AnalyticsFile.Event.OpenAbout);
                ShowChildForm(new FormAbout());
            }
        }

        public void OpenPlugins(){
            if (!FormManager.TryBringToFront<FormPlugins>()){
                TriggerAnalyticsEvent(AnalyticsFile.Event.OpenPlugins);
                ShowChildForm(new FormPlugins(plugins));
            }
        }

        public void OnTweetNotification(){ // may be called multiple times, once for each type of notification
            if (Config.EnableTrayHighlight && !ContainsFocus){
                trayIcon.HasNotifications = true;
            }
        }

        public void OnTweetSound(){
            TriggerAnalyticsEvent(AnalyticsFile.Event.SoundNotification);
        }

        public void PlayVideo(string url, string username){
            if (string.IsNullOrEmpty(url)){
                videoPlayer?.Close();
                return;
            }

            if (videoPlayer == null){
                videoPlayer = new VideoPlayer(this);

                videoPlayer.ProcessExited += (sender, args) => {
                    browser.HideVideoOverlay(true);
                };
            }
            
            videoPlayer.Launch(url, username);
            TriggerAnalyticsEvent(AnalyticsFile.Event.VideoPlay);
        }

        public bool ProcessBrowserKey(Keys key){
            if (videoPlayer != null && videoPlayer.Running){
                videoPlayer.SendKeyEvent(key);
                return true;
            }

            return false;
        }

        public void ShowTweetDetail(string columnId, string chirpId, string fallbackUrl){
            Activate();

            if (!browser.IsTweetDeckWebsite){
                FormMessage.Error("View Tweet Detail", "TweetDeck is not currently loaded.", FormMessage.OK);
                return;
            }

            notification.FinishCurrentNotification();
            browser.ShowTweetDetail(columnId, chirpId, fallbackUrl);
            TriggerAnalyticsEvent(AnalyticsFile.Event.TweetDetail);
        }

        public void OnTweetScreenshotReady(string html, int width, int height){
            if (notificationScreenshotManager == null){
                notificationScreenshotManager = new TweetScreenshotManager(this, plugins);
            }

            notificationScreenshotManager.Trigger(html, width, height);
            TriggerAnalyticsEvent(AnalyticsFile.Event.TweetScreenshot);
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
    }
}
