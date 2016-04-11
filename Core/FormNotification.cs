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
        private readonly bool autoHide;
        private int timeLeft, totalTime;

        public FormNotification(Form owner, bool autoHide){
            InitializeComponent();

            this.owner = owner;
            this.autoHide = autoHide;

            browser = new ChromiumWebBrowser(""){ MenuHandler = new MenuHandlerEmpty() };
            panelBrowser.Controls.Add(browser);
        }

        public void ShowNotification(TweetNotification notification){
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
            Location = new Point(32000,32000);
            timerProgress.Stop();
        }

        private void LoadNextNotification(){
            TweetNotification tweet = tweetQueue.Dequeue();

            browser.Load("about:blank"); // required, otherwise shit breaks
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
        }

        private void timerHideProgress_Tick(object sender, EventArgs e){
            if (Bounds.Contains(Cursor.Position))return;

            timeLeft -= timerProgress.Interval;
            progressBarTimer.SetValueInstant((int)Math.Min(1000,Math.Round(1001.0*(totalTime-timeLeft)/totalTime)));

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
