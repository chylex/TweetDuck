using System;
using System.Windows.Forms;
using TweetDck.Core.Bridge;
using TweetDck.Resources;
using System.Drawing;
using System.Drawing.Imaging;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Notification.Screenshot{
    sealed class FormNotificationScreenshotable : FormNotificationBase{
        public FormNotificationScreenshotable(Action callback, Form owner) : base(owner, false){
            browser.RegisterAsyncJsObject("$TD_NotificationScreenshot", new CallbackBridge(this, callback));

            browser.FrameLoadEnd += (sender, args) => {
                if (args.Frame.IsMain && browser.Address != "about:blank"){
                    ScriptLoader.ExecuteScript(args.Frame, "window.setTimeout($TD_NotificationScreenshot.trigger, 67)", "gen:screenshot");
                }
            };
        }

        protected override string GetTweetHTML(TweetNotification tweet){
            return tweet.GenerateHtml(enableCustomCSS: false);
        }

        public void LoadNotificationForScreenshot(TweetNotification tweet, int width, int height){
            LoadTweet(tweet);
            SetNotificationSize(width, height);
        }

        public void TakeScreenshot(){
            IntPtr context = NativeMethods.GetDC(this.Handle);

            if (context == IntPtr.Zero){
                MessageBox.Show("Could not retrieve a graphics context handle for the notification window to take the screenshot.", "Screenshot Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else{
                using(Bitmap bmp = new Bitmap(ClientSize.Width, ClientSize.Height, PixelFormat.Format32bppRgb)){
                    try{
                        NativeMethods.RenderSourceIntoBitmap(context, bmp);
                    }finally{
                        NativeMethods.ReleaseDC(this.Handle, context);
                    }

                    Clipboard.SetImage(bmp);
                }
            }
        }
    }
}
