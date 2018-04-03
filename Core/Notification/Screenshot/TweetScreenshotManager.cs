// Uncomment to keep screenshot windows visible for debugging
// #define NO_HIDE_SCREENSHOTS

using System;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Plugins;

namespace TweetDuck.Core.Notification.Screenshot{
    sealed class TweetScreenshotManager : IDisposable{
        private readonly FormBrowser owner;
        private readonly PluginManager plugins;
        private readonly Timer timeout;
        private readonly Timer disposer;
        
        private FormNotificationScreenshotable screenshot;

        public TweetScreenshotManager(FormBrowser owner, PluginManager pluginManager){
            this.owner = owner;
            this.plugins = pluginManager;

            this.timeout = new Timer{ Interval = 8000 };
            this.timeout.Tick += timeout_Tick;

            this.disposer = new Timer{ Interval = 1 };
            this.disposer.Tick += disposer_Tick;
        }

        private void timeout_Tick(object sender, EventArgs e){
            timeout.Stop();
            OnFinished();
        }

        private void disposer_Tick(object sender, EventArgs e){
            disposer.Stop();
            screenshot.Dispose();
            screenshot = null;
        }

        public void Trigger(string html, int width){
            if (screenshot != null){
                return;
            }

            screenshot = new FormNotificationScreenshotable(Callback, owner, plugins, html, width);
            screenshot.Show();
            timeout.Start();

            #if !(DEBUG && NO_HIDE_SCREENSHOTS)
            owner.IsWaiting = true;
            #endif
        }

        private void Callback(){
            if (!timeout.Enabled){
                return;
            }

            timeout.Stop();
            screenshot.TakeScreenshot();

            #if !(DEBUG && NO_HIDE_SCREENSHOTS)
            OnFinished();
            #else
            screenshot.MoveToVisibleLocation();
            screenshot.FormClosed += (sender, args) => disposer.Start();
            #endif
        }

        private void OnFinished(){
            screenshot.Location = ControlExtensions.InvisibleLocation;
            owner.IsWaiting = false;
            disposer.Start();
        }

        public void Dispose(){
            timeout.Dispose();
            disposer.Dispose();
            screenshot?.Dispose();
        }
    }
}
