using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Notification.Screenshot;
using TweetDuck.Core.Other;
using TweetDuck.Core.Other.Analytics;
using TweetDuck.Core.Other.Management;
using TweetDuck.Core.Other.Settings;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;
using TweetDuck.Plugins.Events;
using TweetDuck.Updates;
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

        private readonly TweetDeckBrowser browser;
        private readonly PluginManager plugins;
        private readonly UpdateHandler updates;
        private readonly FormNotificationTweet notification;
        private readonly ContextMenu contextMenu;

        private bool isLoaded;
        private FormWindowState prevState;
        
        private TweetScreenshotManager notificationScreenshotManager;
        private SoundNotification soundNotification;
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
            
            this.browser = new TweetDeckBrowser(this, plugins, new TweetDeckBridge(this, notification));
            this.contextMenu = ContextMenuBrowser.CreateMenu(this);

            Controls.Add(new MenuStrip{ Visible = false }); // fixes Alt freezing the program in Win 10 Anniversary Update

            Disposed += (sender, args) => {
                browser.Dispose();
                contextMenu.Dispose();

                notificationScreenshotManager?.Dispose();
                soundNotification?.Dispose();
                videoPlayer?.Dispose();
                analytics?.Dispose();
            };

            this.trayIcon.ClickRestore += trayIcon_ClickRestore;
            this.trayIcon.ClickClose += trayIcon_ClickClose;
            Config.TrayBehaviorChanged += Config_TrayBehaviorChanged;

            UpdateTrayIcon();

            this.updates = browser.CreateUpdateHandler(updaterSettings);
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

            if (Config.AllowDataCollection){
                analytics = new AnalyticsManager(this, plugins, Program.AnalyticsFilePath);
            }
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
                        trayIcon_ClickRestore(trayIcon, EventArgs.Empty);
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
            }
            
            if (browser.Ready && m.Msg == NativeMethods.WM_PARENTNOTIFY && (m.WParam.ToInt32() & 0xFFFF) == NativeMethods.WM_XBUTTONDOWN){
                if (videoPlayer != null && videoPlayer.Running){
                    videoPlayer.Close();
                }
                else{
                    browser.OnMouseClickExtra(m.WParam);
                }

                return;
            }

            base.WndProc(ref m);
        }

        // notification helpers

        public void PauseNotification(){
            notification.PauseNotification();
        }

        public void ResumeNotification(){
            notification.ResumeNotification();
        }

        // browser bridge methods

        public void ReinjectCustomCSS(string css){
            browser.ReinjectCustomCSS(css);
        }

        public void ReloadToTweetDeck(){
            browser.ReloadToTweetDeck();
        }

        public void TriggerTweetScreenshot(){
            browser.TriggerTweetScreenshot();
        }

        public void ApplyROT13(){
            browser.ApplyROT13();
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
                ShowChildForm(new FormGuide());
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

                    browser.RefreshMemoryTracker();

                    if (Config.AllowDataCollection){
                        if (analytics == null){
                            analytics = new AnalyticsManager(this, plugins, Program.AnalyticsFilePath);
                        }
                    }
                    else if (analytics != null){
                        analytics.Dispose();
                        analytics = null;
                    }
                    
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

            soundNotification.SetVolume(Config.NotificationSoundVolume);
            soundNotification.Play(Config.NotificationSoundPath);
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
    }
}
