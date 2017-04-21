// Uncomment to keep screenshot windows visible for debugging
// #define NO_HIDE_SCREENSHOTS

using System;
using System.Windows.Forms;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Notification.Screenshot{
    sealed class TweetScreenshotManager : IDisposable{
        private readonly Form owner;
        private readonly Timer timeout;
        private readonly Timer disposer;
        
        private FormNotificationScreenshotable screenshot;

        public TweetScreenshotManager(Form owner){
            this.owner = owner;

            this.timeout = new Timer{ Interval = 8000 };
            this.timeout.Tick += timeout_Tick;

            this.disposer = new Timer{ Interval = 1 };
            this.disposer.Tick += disposer_Tick;
        }

        private void timeout_Tick(object sender, EventArgs e){
            timeout.Stop();
            screenshot.Location = ControlExtensions.InvisibleLocation;
            disposer.Start();
        }

        private void disposer_Tick(object sender, EventArgs e){
            disposer.Stop();
            screenshot.Dispose();
            screenshot = null;
        }

        public void Trigger(string html, int width, int height){
            if (screenshot != null){
                return;
            }

            screenshot = new FormNotificationScreenshotable(Callback, owner){
                CanMoveWindow = () => false
            };

            screenshot.LoadNotificationForScreenshot(new TweetNotification(string.Empty, html, 0, string.Empty, string.Empty), width, height);
            screenshot.Show();
            timeout.Start();
        }

        private void Callback(){
            if (!timeout.Enabled){
                return;
            }

            timeout.Stop();
            screenshot.TakeScreenshot();

            #if !(DEBUG && NO_HIDE_SCREENSHOTS)
            screenshot.Location = ControlExtensions.InvisibleLocation;
            disposer.Start();
            #else
            screenshot.MoveToVisibleLocation();
            screenshot.FormClosed += (sender, args) => disposer.Start();
            #endif
        }

        public void Dispose(){
            timeout.Dispose();
            disposer.Dispose();

            if (screenshot != null){
                screenshot.Dispose();
            }
        }
    }
}
