using System;
using System.Windows.Forms;
using CefSharp;
using TweetDck.Core.Bridge;
using TweetDck.Core.Controls;
using TweetDck.Resources;

namespace TweetDck.Core.Notification.Screenshot{
    sealed class FormNotificationScreenshotable : FormNotification{
        public FormNotificationScreenshotable(FormBrowser owner, NotificationFlags flags) : base(owner, null, flags){
            UpdateTitle();
        }

        public void PrepareNotificationForScreenshot(Action callback){
            browser.RegisterAsyncJsObject("$TD_NotificationScreenshot", new CallbackBridge(this, callback));

            browser.FrameLoadEnd += (sender, args) => {
                if (args.Frame.IsMain && browser.Address != "about:blank"){
                    ScriptLoader.ExecuteScript(args.Frame, "window.setTimeout(() => $TD_NotificationScreenshot.trigger(), 25)", "gen:screenshot");
                }
            };
        }

        public void LoadNotificationForScreenshot(TweetNotification tweet, int width, int height){
            browser.LoadHtml(tweet.GenerateHtml(enableCustomCSS: false), "http://tweetdeck.twitter.com/?"+DateTime.Now.Ticks);
            
            Location = ControlExtensions.InvisibleLocation;
            SetNotificationSize(width, height, false);
        }

        public void TakeScreenshotAndHide(){
            MoveToVisibleLocation();
            Activate();
            SendKeys.SendWait("%{PRTSC}");
            
            Location = ControlExtensions.InvisibleLocation;
            browser.LoadHtml("", "about:blank");
        }
    }
}
