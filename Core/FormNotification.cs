using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDck.Configuration;
using TweetDck.Core.Handling;
using TweetDck.Resources;

namespace TweetDck.Core{
    sealed partial class FormNotification : Form{
        public Func<bool> CanMoveWindow = () => true;

        private readonly Form owner;
        private readonly ChromiumWebBrowser browser;

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);
        private readonly bool autoHide;
        private int timeLeft, totalTime;

        private readonly string notificationJS;

        public FormNotification(Form owner, TweetDeckBridge bridge, bool autoHide){
            InitializeComponent();

            Text = Program.BrandName;

            this.owner = owner;
            this.autoHide = autoHide;

            owner.FormClosed += (sender, args) => Close();

            notificationJS = ScriptLoader.LoadResource("notification.js");

            browser = new ChromiumWebBrowser("about:blank"){ MenuHandler = new MenuHandlerEmpty() };
            browser.FrameLoadEnd += Browser_FrameLoadEnd;

            if (bridge != null){
                browser.RegisterJsObject("$TD",bridge);
            }

            panelBrowser.Controls.Add(browser);

            Disposed += (sender, args) => browser.Dispose();
        }

        public FormNotification(Form owner, bool autoHide) : this(owner,null,autoHide){}

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && notificationJS != null){
                browser.ExecuteScriptAsync(notificationJS);
            }
        }

        protected override void WndProc(ref Message m){
            if (m.Msg == 0x0112 && (m.WParam.ToInt32() & 0xFFF0) == 0xF010 && !CanMoveWindow()){ // WM_SYSCOMMAND, SC_MOVE
                return;
            }

            base.WndProc(ref m);
        }

        public void ShowNotification(TweetNotification notification){
            if (Program.UserConfig.DisplayNotificationTimer){
                panelBrowser.Height = 156;
                progressBarTimer.Visible = true;
            }
            else{
                panelBrowser.Height = 152;
                progressBarTimer.Visible = false;
            }

            MoveToVisibleLocation();

            tweetQueue.Enqueue(notification);

            if (!timerProgress.Enabled){
                LoadNextNotification();
            }
        }

        public void ShowNotificationForSettings(bool resetAnimation){
            if (browser.Address == "about:blank"){
                browser.Load("about:blank"); // required, otherwise shit breaks
                browser.LoadHtml(TweetNotification.ExampleTweet.GenerateHtml(),"http://tweetdeck.twitter.com/");
                resetAnimation = true;
            }

            if (resetAnimation){
                totalTime = timeLeft = TweetNotification.ExampleTweet.GetDisplayDuration(Program.UserConfig.NotificationDuration);
                timerProgress.Start();
            }

            MoveToVisibleLocation();
        }

        public void HideNotification(){
            browser.LoadHtml("","about:blank");
            Location = new Point(-32000,-32000);
            TopMost = false;
            timerProgress.Stop();
        }

        private void LoadNextNotification(){
            TweetNotification tweet = tweetQueue.Dequeue();

            if (browser.Address == "about:blank"){
                browser.Load("about:blank"); // required, otherwise shit breaks
            }

            browser.LoadHtml(tweet.GenerateHtml(),"http://tweetdeck.twitter.com/");

            totalTime = timeLeft = tweet.GetDisplayDuration(Program.UserConfig.NotificationDuration);
            timerProgress.Stop();
            timerProgress.Start();
        }

        private void MoveToVisibleLocation(){
            UserConfig config = Program.UserConfig;
            Screen screen = Screen.FromControl(owner);

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

            TopMost = true;
        }

        private void timerHideProgress_Tick(object sender, EventArgs e){
            if (Bounds.Contains(Cursor.Position))return;

            timeLeft -= timerProgress.Interval;
            progressBarTimer.SetValueInstant((int)Math.Min(1000,Math.Round(1050.0*(totalTime-timeLeft)/totalTime)));

            if (timeLeft <= 0){
                if (tweetQueue.Count > 0){
                    LoadNextNotification();
                }
                else if (autoHide){
                    HideNotification();
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

        private class MenuHandlerEmpty : IContextMenuHandler{
            public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
                model.Clear();
            }

            public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
                return false;
            }

            public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame){}

            public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback){
                return false;
            }
        }
    }
}
