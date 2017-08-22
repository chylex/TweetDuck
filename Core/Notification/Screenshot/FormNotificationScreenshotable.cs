using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Other;
using TweetDuck.Core.Utils;
using TweetDuck.Data;
using TweetDuck.Plugins;
using TweetDuck.Resources;

namespace TweetDuck.Core.Notification.Screenshot{
    sealed class FormNotificationScreenshotable : FormNotificationBase{
        private readonly PluginManager plugins;

        public FormNotificationScreenshotable(Action callback, FormBrowser owner, PluginManager pluginManager) : base(owner, false){
            this.plugins = pluginManager;

            browser.RegisterAsyncJsObject("$TD_NotificationScreenshot", new CallbackBridge(this, callback));

            browser.LoadingStateChanged += (sender, args) => {
                if (!args.IsLoading){
                    using(IFrame frame = args.Browser.MainFrame){
                        ScriptLoader.ExecuteScript(frame, "window.setTimeout($TD_NotificationScreenshot.trigger, 129)", "gen:screenshot");
                    }
                }
            };
        }
        
        protected override string GetTweetHTML(TweetNotification tweet){
            string html = tweet.GenerateHtml("td-screenshot", false);

            foreach(InjectedHTML injection in plugins.Bridge.NotificationInjections){
                html = injection.Inject(html);
            }

            return html;
        }

        public void LoadNotificationForScreenshot(TweetNotification tweet, int width, int height){
            LoadTweet(tweet);
            SetNotificationSize(width, height);
        }

        public void TakeScreenshot(){
            IntPtr context = NativeMethods.GetDC(this.Handle);

            if (context == IntPtr.Zero){
                FormMessage.Error("Screenshot Failed", "Could not retrieve a graphics context handle for the notification window to take the screenshot.", FormMessage.OK);
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
