using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDck.Configuration;
using TweetDck.Core.Handling;
using TweetDck.Resources;
using TweetDck.Core.Utils;
using TweetDck.Plugins;

namespace TweetDck.Core{
    sealed partial class FormNotification : Form{
        private const string NotificationScriptFile = "notification.js";

        private static readonly string NotificationScriptIdentifier = ScriptLoader.GetRootIdentifier(NotificationScriptFile);
        private static readonly string PluginScriptIdentifier = ScriptLoader.GetRootIdentifier(PluginManager.PluginNotificationScriptFile);

        public Func<bool> CanMoveWindow = () => true;

        private readonly Form owner;
        private readonly PluginManager plugins;
        private readonly TrayIcon trayIcon;
        private readonly ChromiumWebBrowser browser;

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);
        private readonly bool autoHide;
        private int timeLeft, totalTime;

        private bool? prevDisplayTimer;
        private int? prevFontSize;

        private bool RequiresResize{
            get{
                return !prevDisplayTimer.HasValue || !prevFontSize.HasValue || prevDisplayTimer != Program.UserConfig.DisplayNotificationTimer || prevFontSize != TweetNotification.FontSizeLevel;
            }

            set{
                if (value){
                    prevDisplayTimer = null;
                    prevFontSize = null;
                }
                else{
                    prevDisplayTimer = Program.UserConfig.DisplayNotificationTimer;
                    prevFontSize = TweetNotification.FontSizeLevel;
                }
            }
        }

        private readonly string notificationJS;
        private readonly string pluginJS;

        protected override bool ShowWithoutActivation{
            get{
                return true;
            }
        }

        public bool FreezeTimer { get; set; }
        public bool ContextMenuOpen { get; set; }
        public string CurrentUrl { get; private set; }

        private static int BaseClientWidth{
            get{
                int level = TweetNotification.FontSizeLevel;
                return level == 0 ? 284 : (int)Math.Round(284.0*(1.0+0.05*level));
            }
        }

        private static int BaseClientHeight{
            get{
                int level = TweetNotification.FontSizeLevel;
                return level == 0 ? 118 : (int)Math.Round(118.0*(1.0+0.075*level));
            }
        }

        public FormNotification(FormBrowser owner, PluginManager plugins, TrayIcon trayIcon, bool autoHide){
            InitializeComponent();

            Text = Program.BrandName;

            this.owner = owner;
            this.plugins = plugins;
            this.trayIcon = trayIcon;
            this.autoHide = autoHide;

            owner.FormClosed += (sender, args) => Close();

            notificationJS = ScriptLoader.LoadResource(NotificationScriptFile);
            pluginJS = ScriptLoader.LoadResource(PluginManager.PluginNotificationScriptFile);

            browser = new ChromiumWebBrowser("about:blank"){
                MenuHandler = new ContextMenuNotification(this,autoHide),
                LifeSpanHandler = new LifeSpanHandler()
            };

            browser.FrameLoadEnd += Browser_FrameLoadEnd;
            browser.RegisterJsObject("$TD",new TweetDeckBridge(owner,this));

            panelBrowser.Controls.Add(browser);

            if (autoHide){
                Program.UserConfig.MuteToggled += Config_MuteToggled;
                Disposed += (sender, args) => Program.UserConfig.MuteToggled -= Config_MuteToggled;
            }

            Disposed += (sender, args) => browser.Dispose();
        }

        protected override void WndProc(ref Message m){
            if (m.Msg == 0x0112 && (m.WParam.ToInt32() & 0xFFF0) == 0xF010 && !CanMoveWindow()){ // WM_SYSCOMMAND, SC_MOVE
                return;
            }

            base.WndProc(ref m);
        }

        // event handlers

        private void timerHideProgress_Tick(object sender, EventArgs e){
            if (Bounds.Contains(Cursor.Position) || FreezeTimer || ContextMenuOpen)return;

            timeLeft -= timerProgress.Interval;
            progressBarTimer.SetValueInstant((int)Math.Min(1000,Math.Round(1025.0*(totalTime-timeLeft)/totalTime)));

            if (timeLeft <= 0){
                FinishCurrentTweet();
            }
        }

        private void Config_MuteToggled(object sender, EventArgs e){
            if (Program.UserConfig.MuteNotifications){
                HideNotification();
            }
            else{
                if (tweetQueue.Count > 0){
                    LoadNextNotification();
                }

                trayIcon.HasNotifications = false;
            }
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && notificationJS != null && browser.Address != "about:blank"){
                ScriptLoader.ExecuteScript(e.Frame,notificationJS,NotificationScriptIdentifier);

                if (plugins.HasAnyPlugin(PluginEnvironment.Notification)){
                    ScriptLoader.ExecuteScript(e.Frame,pluginJS,PluginScriptIdentifier);
                    plugins.ExecutePlugins(e.Frame,PluginEnvironment.Notification,false);
                }
            }
        }

        private void FormNotification_FormClosing(object sender, FormClosingEventArgs e){
            if (e.CloseReason == CloseReason.UserClosing){
                HideNotification();
                tweetQueue.Clear();
                e.Cancel = true;
            }
        }

        // notification methods

        public void ShowNotification(TweetNotification notification){
            if (Program.UserConfig.MuteNotifications){
                tweetQueue.Enqueue(notification);
                trayIcon.HasNotifications = true;
            }
            else{
                tweetQueue.Enqueue(notification);
                UpdateTitle();

                if (!timerProgress.Enabled){
                    LoadNextNotification();
                }
            }
        }

        public void ShowNotificationForSettings(bool reset){
            if (reset || browser.Address == "about:blank"){
                LoadTweet(TweetNotification.ExampleTweet);
            }
            else{
                MoveToVisibleLocation();
            }
        }

        public void HideNotification(){
            browser.LoadHtml("","about:blank");
            Location = new Point(-32000,-32000);
            progressBarTimer.Value = 0;
            timerProgress.Stop();
        }

        public void OnNotificationReady(){
            UpdateTitle();
            MoveToVisibleLocation();
            timerProgress.Start();
        }

        public void FinishCurrentTweet(){
            if (tweetQueue.Count > 0){
                LoadNextNotification();
            }
            else if (autoHide){
                HideNotification();
            }
            else{
                timerProgress.Stop();
            }
        }

        private void LoadNextNotification(){
            LoadTweet(tweetQueue.Dequeue());
        }

        private void LoadTweet(TweetNotification tweet){
            CurrentUrl = tweet.Url;
            
            timerProgress.Stop();
            totalTime = timeLeft = tweet.GetDisplayDuration(Program.UserConfig.NotificationDuration);
            progressBarTimer.Value = 0;

            browser.LoadHtml(tweet.GenerateHtml(),"http://tweetdeck.twitter.com/?"+DateTime.Now.Ticks);
        }

        private void MoveToVisibleLocation(){
            UserConfig config = Program.UserConfig;

            if (RequiresResize){
                RequiresResize = false;

                if (config.DisplayNotificationTimer){
                    ClientSize = new Size(BaseClientWidth,BaseClientHeight+4);
                    progressBarTimer.Visible = true;
                }
                else{
                    ClientSize = new Size(BaseClientWidth,BaseClientHeight);
                    progressBarTimer.Visible = false;
                }

                panelBrowser.Height = BaseClientHeight;
            }
            
            Screen screen = Screen.FromControl(owner);

            if (config.NotificationDisplay > 0 && config.NotificationDisplay <= Screen.AllScreens.Length){
                screen = Screen.AllScreens[config.NotificationDisplay-1];
            }
            
            bool needsReactivating = Location.X == -32000;
            int edgeDist = config.NotificationEdgeDistance;

            switch(config.NotificationPosition){
                case TweetNotification.Position.TopLeft:
                    Location = new Point(screen.WorkingArea.X+edgeDist,screen.WorkingArea.Y+edgeDist);
                    break;

                case TweetNotification.Position.TopRight:
                    Location = new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width,screen.WorkingArea.Y+edgeDist);
                    break;

                case TweetNotification.Position.BottomLeft:
                    Location = new Point(screen.WorkingArea.X+edgeDist,screen.WorkingArea.Y+screen.WorkingArea.Height-edgeDist-Height);
                    break;

                case TweetNotification.Position.BottomRight:
                    Location = new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width,screen.WorkingArea.Y+screen.WorkingArea.Height-edgeDist-Height);
                    break;

                case TweetNotification.Position.Custom:
                    if (!config.IsCustomNotificationPositionSet){
                        config.CustomNotificationPosition = new Point(screen.WorkingArea.X+screen.WorkingArea.Width-edgeDist-Width,screen.WorkingArea.Y+edgeDist);
                        config.Save();
                    }

                    Location = config.CustomNotificationPosition;
                    break;
            }

            if (needsReactivating){
                NativeMethods.SetFormPos(this,NativeMethods.HWND_TOPMOST,NativeMethods.SWP_NOACTIVATE);
            }
        }

        private void UpdateTitle(){
            Text = tweetQueue.Count > 0 ? Program.BrandName+" ("+tweetQueue.Count+" more left)" : Program.BrandName;
        }

        public void DisplayTooltip(string text){
            if (string.IsNullOrEmpty(text)){
                toolTip.Hide(this);
            }
            else{
                Point position = PointToClient(Cursor.Position);
                position.Offset(20,5);
                toolTip.Show(text,this,position);
            }
        }
    }
}
