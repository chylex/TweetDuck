using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Management;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Other;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Bridge{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    class TweetDeckBridge{
        public static string FontSize { get; private set; }
        public static string NotificationHeadLayout { get; private set; }
        public static readonly ContextInfo ContextInfo = new ContextInfo();

        public static void ResetStaticProperties(){
            FontSize = NotificationHeadLayout = null;
        }

        private readonly FormBrowser form;
        private readonly FormNotificationMain notification;

        protected TweetDeckBridge(FormBrowser form, FormNotificationMain notification){
            this.form = form;
            this.notification = notification;
        }

        // Browser only

        public sealed class Browser : TweetDeckBridge{
            public Browser(FormBrowser form, FormNotificationMain notification) : base(form, notification){}

            public void OpenContextMenu(){
                form.InvokeAsyncSafe(form.OpenContextMenu);
            }

            public void OpenProfileImport(){
                form.InvokeAsyncSafe(form.OpenProfileImport);
            }
            
            public void OnIntroductionClosed(bool showGuide, bool allowDataCollection){
                form.InvokeAsyncSafe(() => {
                    form.OnIntroductionClosed(showGuide, allowDataCollection);
                });
            }

            public void LoadNotificationLayout(string fontSize, string headLayout){
                form.InvokeAsyncSafe(() => {
                    FontSize = fontSize;
                    NotificationHeadLayout = headLayout;
                });
            }

            public void SetRightClickedLink(string type, string url){
                ContextInfo.SetLink(type, url);
            }

            public void SetRightClickedChirp(string tweetUrl, string quoteUrl, string chirpAuthors, string chirpImages){
                ContextInfo.SetChirp(tweetUrl, quoteUrl, chirpAuthors, chirpImages);
            }

            public void DisplayTooltip(string text){
                form.InvokeAsyncSafe(() => form.DisplayTooltip(text));
            }
        }

        // Notification only
        
        public sealed class Notification : TweetDeckBridge{
            public Notification(FormBrowser form, FormNotificationMain notification) : base(form, notification){}

            public void DisplayTooltip(string text){
                notification.InvokeAsyncSafe(() => notification.DisplayTooltip(text));
            }

            public void LoadNextNotification(){
                notification.InvokeAsyncSafe(notification.FinishCurrentNotification);
            }

            public void ShowTweetDetail(){
                notification.InvokeAsyncSafe(notification.ShowTweetDetail);
            }
        }

        // Global

        public void OnTweetPopup(string columnId, string chirpId, string columnName, string tweetHtml, int tweetCharacters, string tweetUrl, string quoteUrl){
            notification.InvokeAsyncSafe(() => {
                form.OnTweetNotification();
                notification.ShowNotification(new TweetNotification(columnId, chirpId, columnName, tweetHtml, tweetCharacters, tweetUrl, quoteUrl));
            });
        }

        public void OnTweetSound(){
            form.InvokeAsyncSafe(() => {
                form.OnTweetNotification();
                form.OnTweetSound();
            });
        }

        public void ScreenshotTweet(string html, int width){
            form.InvokeAsyncSafe(() => form.OnTweetScreenshotReady(html, width));
        }

        public void PlayVideo(string url, string username){
            form.InvokeAsyncSafe(() => form.PlayVideo(url, username));
        }

        public void FixClipboard(){
            form.InvokeAsyncSafe(WindowsUtils.ClipboardStripHtmlStyles);
        }

        public void OpenBrowser(string url){
            form.InvokeAsyncSafe(() => BrowserUtils.OpenExternalBrowser(url));
        }

        public int GetIdleSeconds(){
            return NativeMethods.GetIdleSeconds();
        }

        public void Alert(string type, string contents){
            MessageBoxIcon icon;

            switch(type){
                case "error": icon = MessageBoxIcon.Error; break;
                case "warning": icon = MessageBoxIcon.Warning; break;
                case "info": icon = MessageBoxIcon.Information; break;
                default: icon = MessageBoxIcon.None; break;
            }

            FormMessage.Show("TweetDuck Browser Message", contents, icon, FormMessage.OK);
        }

        public void CrashDebug(string message){
            #if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
            System.Diagnostics.Debugger.Break();
            #endif
        }
    }
}
