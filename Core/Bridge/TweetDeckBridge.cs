using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Other;
using TweetDuck.Core.Utils;
using TweetDuck.Resources;

namespace TweetDuck.Core.Bridge{
    sealed class TweetDeckBridge{
        public static string FontSizeClass;
        public static string NotificationHeadContents;

        public static string LastHighlightedTweetUrl = string.Empty;
        public static string LastHighlightedQuoteUrl = string.Empty;
        private static string LastHighlightedTweetAuthors = string.Empty;
        private static string LastHighlightedTweetImages = string.Empty;

        public static string[] LastHighlightedTweetAuthorsArray => LastHighlightedTweetAuthors.Split(';');
        public static string[] LastHighlightedTweetImagesArray => LastHighlightedTweetImages.Split(';');

        private static readonly Dictionary<string, string> SessionData = new Dictionary<string, string>(2);

        public static void ResetStaticProperties(){
            FontSizeClass = NotificationHeadContents = null;
            LastHighlightedTweetUrl = LastHighlightedQuoteUrl = LastHighlightedTweetAuthors = LastHighlightedTweetImages = string.Empty;
        }

        public static void RestoreSessionData(IFrame frame){
            if (SessionData.Count > 0){
                StringBuilder build = new StringBuilder().Append("window.TD_SESSION={");
                
                foreach(KeyValuePair<string, string> kvp in SessionData){
                    build.Append(kvp.Key).Append(":'").Append(kvp.Value.Replace("'", "\\'")).Append("',");
                }

                ScriptLoader.ExecuteScript(frame, build.Append("}").ToString(), "gen:session");
                SessionData.Clear();
            }
        }

        private readonly FormBrowser form;
        private readonly FormNotificationMain notification;

        public TweetDeckBridge(FormBrowser form, FormNotificationMain notification){
            this.form = form;
            this.notification = notification;
        }

        public void OnIntroductionClosed(bool showGuide, bool allowDataCollection){
            form.InvokeAsyncSafe(() => {
                form.OnIntroductionClosed(showGuide, allowDataCollection);
            });
        }

        public void LoadFontSizeClass(string fsClass){
            form.InvokeAsyncSafe(() => FontSizeClass = fsClass);
        }

        public void LoadNotificationHeadContents(string headContents){
            form.InvokeAsyncSafe(() => NotificationHeadContents = headContents);
        }

        public void SetLastRightClickInfo(string type, string link){
            form.InvokeAsyncSafe(() => ContextMenuBase.SetContextInfo(type, link));
        }

        public void SetLastHighlightedTweet(string tweetUrl, string quoteUrl, string authors, string imageList){
            form.InvokeAsyncSafe(() => {
                LastHighlightedTweetUrl = tweetUrl;
                LastHighlightedQuoteUrl = quoteUrl;
                LastHighlightedTweetAuthors = authors;
                LastHighlightedTweetImages = imageList;
            });
        }

        public void OpenContextMenu(){
            form.InvokeAsyncSafe(form.OpenContextMenu);
        }

        public void OnTweetPopup(string columnId, string chirpId, string columnName, string tweetHtml, int tweetCharacters, string tweetUrl, string quoteUrl){
            notification.InvokeAsyncSafe(() => {
                form.OnTweetNotification();
                notification.ShowNotification(new TweetNotification(columnId, chirpId, columnName, tweetHtml, tweetCharacters, tweetUrl, quoteUrl));
            });
        }

        public void OnTweetSound(){
            form.InvokeAsyncSafe(() => {
                form.OnTweetNotification();
                form.PlayNotificationSound();
            });
        }

        public void DisplayTooltip(string text, bool showInNotification){
            if (showInNotification){
                notification.InvokeAsyncSafe(() => notification.DisplayTooltip(text));
            }
            else{
                form.InvokeAsyncSafe(() => form.DisplayTooltip(text));
            }
        }

        public void SetSessionData(string key, string value){
            form.InvokeSafe(() => { // do not use InvokeAsyncSafe, return only after invocation
                SessionData.Add(key, value);
            });
        }

        public void LoadNextNotification(){
            notification.InvokeAsyncSafe(notification.FinishCurrentNotification);
        }

        public void ScreenshotTweet(string html, int width, int height){
            form.InvokeAsyncSafe(() => form.OnTweetScreenshotReady(html, width, height));
        }

        public void PlayVideo(string url){
            form.InvokeAsyncSafe(() => form.PlayVideo(url));
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
            Log(message);
            System.Diagnostics.Debugger.Break();
            #endif
        }

        public void Log(string data){
            System.Diagnostics.Debug.WriteLine(data);
        }
    }
}
