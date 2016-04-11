using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Drawing;
using System;
using System.Collections.Generic;
using TweetDick.Core.Handling;
using TweetDick.Configuration;

namespace TweetDick.Core{
    partial class FormNotification : Form{
        private readonly Form owner;
        private readonly ChromiumWebBrowser browser;

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);
        private DateTime timeLeftStart;

        public FormNotification(Form owner){
            InitializeComponent();

            this.owner = owner;

            browser = new ChromiumWebBrowser("about:blank"){ MenuHandler = new MenuHandlerEmpty() };
            panelBrowser.Controls.Add(browser);
        }

        public void ShowNotification(TweetNotification notification){
            MoveToVisibleLocation();

            tweetQueue.Enqueue(notification);

            if (!timerNext.Enabled){
                LoadNextNotification();
            }
        }

        public void ShowNotificationForSettings(bool resetAnimation){
            if (browser.Address == "about:blank"){
                browser.Load("about:blank");
                browser.LoadHtml(TweetNotification.ExampleTweet.GenerateHtml(),"http://tweetdeck.twitter.com/");
                resetAnimation = true;
            }

            if (resetAnimation){
                timerNext.Interval = TweetNotification.ExampleTweet.GetDisplayDuration(Program.UserConfig.NotificationDuration);
                timeLeftStart = DateTime.Now;
                timerHideProgress.Stop();
                timerHideProgress.Start();
            }

            MoveToVisibleLocation();
        }

        public void HideNotification(){
            browser.Load("about:blank");
            Location = new Point(32000,32000);

            timerNext.Stop();
            timerHideProgress.Stop();
        }

        private void LoadNextNotification(){
            TweetNotification tweet = tweetQueue.Dequeue();

            browser.Load("about:blank");
            browser.LoadHtml(tweet.GenerateHtml(),"http://tweetdeck.twitter.com/");

            timerNext.Stop();
            timerNext.Interval = tweet.GetDisplayDuration(Program.UserConfig.NotificationDuration);
            timerNext.Start();

            timeLeftStart = DateTime.Now;
            timerHideProgress.Stop();
            timerHideProgress.Start();
        }

        private void MoveToVisibleLocation(){
            UserConfig config = Program.UserConfig;
            Screen screen = Screen.FromControl(owner);

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
        }

        private void timer_Tick(object sender, EventArgs e){
            if (tweetQueue.Count > 0){
                LoadNextNotification();
            }
            else{
                HideNotification();
            }
        }

        private void timerHideProgress_Tick(object sender, EventArgs e){
            int elapsed = (int)(DateTime.Now-timeLeftStart).TotalMilliseconds;
            progressBarTimer.SetValueInstant((int)Math.Min(1000,Math.Round(1001.0*elapsed/timerNext.Interval)));
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
