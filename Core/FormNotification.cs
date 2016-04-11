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

        public FormNotification(Form owner){
            InitializeComponent();

            this.owner = owner;

            browser = new ChromiumWebBrowser(""){ MenuHandler = new MenuHandlerEmpty() };
            Controls.Add(browser);
        }

        public void ShowNotification(TweetNotification notification){
            MoveToVisibleLocation();

            tweetQueue.Enqueue(notification);

            if (!timer.Enabled){
                LoadNextNotification();
            }
        }

        public void ShowNotificationForSettings(){
            browser.Load("about:blank");
            MoveToVisibleLocation();
        }

        public void HideNotification(){
            browser.Load("about:blank");
            Location = new Point(32000,32000);
        }

        private void LoadNextNotification(){
            TweetNotification tweet = tweetQueue.Dequeue();

            browser.Load("about:blank");
            browser.LoadHtml(tweet.GenerateHtml(),"http://tweetdeck.twitter.com/");

            timer.Stop();
            timer.Interval = tweet.GetDisplayDuration(Program.UserConfig.NotificationDuration);
            timer.Start();
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
