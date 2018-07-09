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
        private int height;

        public FormNotificationScreenshotable(Action callback, FormBrowser owner, PluginManager pluginManager, string html, int width) : base(owner, false){
            this.plugins = pluginManager;

            browser.RegisterAsyncJsObject("$TD_NotificationScreenshot", new ScreenshotBridge(this, SetScreenshotHeight, callback));

            browser.LoadingStateChanged += (sender, args) => {
                if (args.IsLoading){
                    return;
                }

                string script = ScriptLoader.LoadResourceSilent("screenshot.js");
                        
                if (script == null){
                    this.InvokeAsyncSafe(callback);
                    return;
                }
                
                using(IFrame frame = args.Browser.MainFrame){
                    ScriptLoader.ExecuteScript(frame, script.Replace("{width}", BrowserUtils.Scale(width, DpiScale).ToString()).Replace("{frames}", TweetScreenshotManager.WaitFrames.ToString()), "gen:screenshot");
                }
            };
            
            SetNotificationSize(width, 1024);
            LoadTweet(new TweetNotification(string.Empty, string.Empty, string.Empty, html, 0, string.Empty, string.Empty));
        }

        protected override string GetTweetHTML(TweetNotification tweet){
            string html = tweet.GenerateHtml("td-screenshot", this);

            foreach(InjectedHTML injection in plugins.NotificationInjections){
                html = injection.InjectInto(html);
            }

            return html;
        }

        private void SetScreenshotHeight(int browserHeight){
            this.height = BrowserUtils.Scale(browserHeight, SizeScale);
        }

        public bool TakeScreenshot(bool ignoreHeightError = false){
            if (!ignoreHeightError){
                if (height == 0){
                    FormMessage.Error("Screenshot Failed", "Could not detect screenshot size.", FormMessage.OK);
                    return false;
                }
                else if (height > ClientSize.Height){
                    FormMessage.Error("Screenshot Failed", $"Screenshot is too large: {height}px > {ClientSize.Height}px", FormMessage.OK);
                    return false;
                }
            }

            if (!WindowsUtils.IsAeroEnabled){
                MoveToVisibleLocation(); // TODO make this look nicer I guess
            }
            
            IntPtr context = NativeMethods.GetDC(this.Handle);

            if (context == IntPtr.Zero){
                FormMessage.Error("Screenshot Failed", "Could not retrieve a graphics context handle for the notification window to take the screenshot.", FormMessage.OK);
                return false;
            }
            else{
                using(Bitmap bmp = new Bitmap(ClientSize.Width, Math.Max(1, height), PixelFormat.Format32bppRgb)){
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
