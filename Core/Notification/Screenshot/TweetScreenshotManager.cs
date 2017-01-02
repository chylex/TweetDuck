using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Notification.Screenshot{
    sealed class TweetScreenshotManager : IDisposable{
        private readonly FormBrowser browser;
        private readonly FormNotificationScreenshotable screenshot;
        private readonly Timer timeout;

        public TweetScreenshotManager(FormBrowser browser){
            this.browser = browser;

            this.screenshot = new FormNotificationScreenshotable(browser, NotificationFlags.DisableScripts | NotificationFlags.DisableContextMenu | NotificationFlags.TopMost){
                CanMoveWindow = () => false
            };

            this.screenshot.PrepareNotificationForScreenshot(Callback);

            this.timeout = WindowsUtils.CreateSingleTickTimer(10000);
            this.timeout.Tick += (sender, args) => screenshot.Reset();
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

        public void Dispose(){
            timeout.Dispose();
            screenshot.Dispose();
        }
    }
}
