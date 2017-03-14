using System;
using System.Windows.Forms;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Notification.Screenshot{
    sealed class TweetScreenshotManager : IDisposable{
        private readonly FormNotificationScreenshotable screenshot;
        private readonly Timer timeout;

        public TweetScreenshotManager(Form owner){
            this.screenshot = new FormNotificationScreenshotable(Callback, owner, NotificationFlags.DisableContextMenu | NotificationFlags.TopMost){
                CanMoveWindow = () => false
            };

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
            screenshot.TakeScreenshotAndHide();
        }

        public void Dispose(){
            screenshot.Dispose();
            timeout.Dispose();
        }
    }
}
