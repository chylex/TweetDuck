using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other;
using TweetDuck.Core.Utils;
using TweetDuck.Data;
using TweetDuck.Plugins;
using TweetDuck.Resources;

namespace TweetDuck.Core.Notification.Screenshot{
    sealed class FormNotificationScreenshotable : FormNotificationBase{
        protected override bool CanDragWindow => false;

        private readonly PluginManager plugins;
        private readonly int width;

        public FormNotificationScreenshotable(Action callback, FormBrowser owner, PluginManager pluginManager, string html, int width) : base(owner, false){
            this.plugins = pluginManager;
            this.width = width;

            browser.RegisterAsyncJsObject("$TD_NotificationScreenshot", new ScreenshotBridge(this, SetScreenshotHeight, callback));

            browser.LoadingStateChanged += (sender, args) => {
                if (args.IsLoading){
                    return;
                }

                string script = ScriptLoader.LoadResource("screenshot.js", true);
                        
                if (script == null){
                    this.InvokeAsyncSafe(callback);
                    return;
                }
                
                using(IFrame frame = args.Browser.MainFrame){
                    ScriptLoader.ExecuteScript(frame, script.Replace("{width}", ClientSize.Width.ToString()), "screenshot");
                }
            };
            
            SetScreenshotHeight(1);
            LoadTweet(new TweetNotification(string.Empty, string.Empty, string.Empty, html, 0, string.Empty, string.Empty));
        }

        protected override string GetTweetHTML(TweetNotification tweet){
            string html = tweet.GenerateHtml("td-screenshot");

            foreach(InjectedHTML injection in plugins.NotificationInjections){
                html = injection.Inject(html);
            }

            return html;
        }

        private void SetScreenshotHeight(int height){
            SetNotificationSize(width, height);
        }

        public bool TakeScreenshot(){
            if (ClientSize.Height == 0){
                FormMessage.Error("Screenshot Failed", "Could not detect screenshot size.", FormMessage.OK);
                return false;
            }
            
            IntPtr context = NativeMethods.GetDC(this.Handle);

            if (context == IntPtr.Zero){
                FormMessage.Error("Screenshot Failed", "Could not retrieve a graphics context handle for the notification window to take the screenshot.", FormMessage.OK);
                return false;
            }
            else{
                using(Bitmap bmp = new Bitmap(ClientSize.Width, ClientSize.Height, PixelFormat.Format32bppRgb)){
                    try{
                        NativeMethods.RenderSourceIntoBitmap(context, bmp);
                    }finally{
                        NativeMethods.ReleaseDC(this.Handle, context);
                    }

                    Clipboard.SetImage(bmp);
                    return true;
                }
            }
        }
    }
}
