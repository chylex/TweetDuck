using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Notification.Screenshot{
    sealed class TweetScreenshotManager : IDisposable{
        private readonly FormBrowser browser;
        private readonly FormNotificationScreenshotable screenshot;
        private readonly Timer timeout;

        public TweetScreenshotManager(FormBrowser browser){
            this.browser = browser;

            this.timeout = new Timer{
                Interval = 10000
            };

            this.timeout.Tick += safetyTimer_Tick;

            this.screenshot = new FormNotificationScreenshotable(browser, NotificationFlags.DisableScripts | NotificationFlags.DisableContextMenu | NotificationFlags.TopMost){
                CanMoveWindow = () => false
            };

            this.screenshot.PrepareNotificationForScreenshot(Callback);
        }

        public void Trigger(string html, int width, int height){
            screenshot.LoadNotificationForScreenshot(new TweetNotification(html, string.Empty, 0), width, height);
            screenshot.Show();
            timeout.Start();
        }

        private void Callback(){
            if (!timeout.Enabled){
                return;
            }

            timeout.Stop();

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

        private void safetyTimer_Tick(object sender, EventArgs e){
            timeout.Stop();
            screenshot.Reset();
        }

        public void Dispose(){
            timeout.Dispose();
            screenshot.Dispose();
        }
    }
}
