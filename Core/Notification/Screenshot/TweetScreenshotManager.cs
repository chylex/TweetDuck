using System;
using System.Drawing;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Notification.Screenshot{
    sealed class TweetScreenshotManager : IDisposable{
        private readonly FormBrowser browser;
        private readonly FormNotificationScreenshotable screenshot;

        public TweetScreenshotManager(FormBrowser browser){
            this.browser = browser;

            this.screenshot = new FormNotificationScreenshotable(browser, NotificationFlags.DisableScripts | NotificationFlags.DisableContextMenu | NotificationFlags.TopMost){
                CanMoveWindow = () => false
            };

            this.screenshot.PrepareNotificationForScreenshot(Callback);
        }

        public void Trigger(string html, int width, int height){
            screenshot.LoadNotificationForScreenshot(new TweetNotification(html, string.Empty, 0), width, height);
            screenshot.Show();

            // TODO start a timer on 10 seconds to close the window if anything fails or takes too long
        }

        private void Callback(){
            FormNotification notification = browser.BrowserNotificationForm;

            Point? prevNotificationLocation = null;
            bool prevFreezeTimer = false;

            if (notification.IsNotificationVisible){
                prevNotificationLocation = notification.Location;
                prevFreezeTimer = notification.FreezeTimer;

                notification.Location = ControlExtensions.InvisibleLocation;
                notification.FreezeTimer = true;
            }

            screenshot.TakeScreenshotAndHide();

            if (prevNotificationLocation.HasValue){
                notification.Location = prevNotificationLocation.Value;
                notification.FreezeTimer = prevFreezeTimer;
            }
        }

        public void Dispose(){
            screenshot.Dispose();
        }
    }
}
