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

namespace TweetDck.Core{
    sealed partial class FormNotification : Form{
        public Func<bool> CanMoveWindow = () => true;

        private readonly Form owner;
        private readonly ChromiumWebBrowser browser;

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);
        private readonly bool autoHide;
        private int timeLeft, totalTime;

        private readonly string notificationJS;

        protected override bool ShowWithoutActivation{
            get{
                return true;
            }
        }

        public bool FreezeTimer { get; set; }
        public string CurrentUrl { get; private set; }

        public FormNotification(Form owner, TweetDeckBridge bridge, bool autoHide){
            InitializeComponent();

            Text = Program.BrandName;

            this.owner = owner;
            this.autoHide = autoHide;

            owner.FormClosed += (sender, args) => Close();

            notificationJS = ScriptLoader.LoadResource("notification.js");

            browser = new ChromiumWebBrowser("about:blank"){ MenuHandler = new ContextMenuNotification(this,autoHide) };
            browser.FrameLoadEnd += Browser_FrameLoadEnd;

            if (bridge != null){
                browser.RegisterJsObject("$TD",bridge);
            }

            panelBrowser.Controls.Add(browser);

            if (autoHide){
                Program.UserConfig.MuteToggled += Config_MuteToggled;
                Disposed += (sender, args) => Program.UserConfig.MuteToggled -= Config_MuteToggled;
            }

            Disposed += (sender, args) => browser.Dispose();
        }

        public FormNotification(Form owner, bool autoHide) : this(owner,null,autoHide){}

        protected override void WndProc(ref Message m){
            if (m.Msg == 0x0112 && (m.WParam.ToInt32() & 0xFFF0) == 0xF010 && !CanMoveWindow()){ // WM_SYSCOMMAND, SC_MOVE
                return;
            }

            base.WndProc(ref m);
        }

        // event handlers

        private void timerHideProgress_Tick(object sender, EventArgs e){
            if (Bounds.Contains(Cursor.Position) || FreezeTimer)return;

            timeLeft -= timerProgress.Interval;
            progressBarTimer.SetValueInstant((int)Math.Min(1000,Math.Round(1050.0*(totalTime-timeLeft)/totalTime)));

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
                    MoveToVisibleLocation();
                    LoadNextNotification();
                }
            }
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && notificationJS != null && browser.Address != "about:blank"){
                browser.ExecuteScriptAsync(notificationJS);
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
            }
            else{
                MoveToVisibleLocation();

                tweetQueue.Enqueue(notification);
                UpdateTitle();

                if (!timerProgress.Enabled){
                    LoadNextNotification();
                }
            }
        }

        public void ShowNotificationForSettings(bool reset){
            if (browser.Address == "about:blank"){
                browser.Load("about:blank"); // required, otherwise shit breaks
                reset = true;
            }
            
            if (reset){
                LoadTweet(TweetNotification.ExampleTweet);
                timerProgress.Start();
            }

            MoveToVisibleLocation();
        }

        public void HideNotification(){
            browser.LoadHtml("","about:blank");
            Location = new Point(-32000,-32000);
            progressBarTimer.Value = 0;
            timerProgress.Stop();
        }

        public void FinishCurrentTweet(){
            if (tweetQueue.Count > 0){
                LoadNextNotification();
            }
            else if (autoHide){
                HideNotification();
            }
        }

        private void LoadNextNotification(){
            TweetNotification tweet = tweetQueue.Dequeue();

            if (browser.Address == "about:blank"){
                browser.Load("about:blank"); // required, otherwise shit breaks
            }

            LoadTweet(tweet);
            timerProgress.Stop();
            timerProgress.Start();

            UpdateTitle();
        }

        private void LoadTweet(TweetNotification tweet){
            browser.LoadHtml(tweet.GenerateHtml(),"http://tweetdeck.twitter.com/");

            totalTime = timeLeft = tweet.GetDisplayDuration(Program.UserConfig.NotificationDuration);
            progressBarTimer.Value = 0;

            CurrentUrl = tweet.Url;
        }

        private void MoveToVisibleLocation(){
            bool needsReactivating = Location.X == -32000;

            UserConfig config = Program.UserConfig;
            Screen screen = Screen.FromControl(owner);

            if (config.DisplayNotificationTimer){
                ClientSize = new Size(ClientSize.Width,122);
                progressBarTimer.Visible = true;
            }
            else{
                ClientSize = new Size(ClientSize.Width,118);
                progressBarTimer.Visible = false;
            }

            if (config.NotificationDisplay > 0 && config.NotificationDisplay <= Screen.AllScreens.Length){
                screen = Screen.AllScreens[config.NotificationDisplay-1];
            }

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
    }
}
