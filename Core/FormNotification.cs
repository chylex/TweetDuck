using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Drawing;
using System;
using System.Collections.Generic;
using TweetDick.Core.Handling;

namespace TweetDick.Core{
    partial class FormNotification : Form{
        private readonly FormBrowser owner;
        private readonly ChromiumWebBrowser browser;

        private readonly Queue<TweetNotification> tweetQueue = new Queue<TweetNotification>(4);

        public FormNotification(FormBrowser owner){
            InitializeComponent();

            this.owner = owner;

            browser = new ChromiumWebBrowser(""){ MenuHandler = new MenuHandlerEmpty() };
            Controls.Add(browser);
        }

        public void ShowNotification(TweetNotification notification){
            Screen screen = Screen.FromControl(owner);
            Location = new Point(screen.WorkingArea.X+screen.WorkingArea.Width-16-Width,screen.WorkingArea.Y+16);

            tweetQueue.Enqueue(notification);

            if (!timer.Enabled){
                LoadNextNotification();
            }
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
            timer.Interval = 5000;
            timer.Start();
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
