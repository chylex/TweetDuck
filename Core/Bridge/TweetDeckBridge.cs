using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Core.Notification;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Bridge{
    sealed class TweetDeckBridge{
        public static string LastRightClickedLink = string.Empty;
        public static string LastHighlightedTweet = string.Empty;
        public static string LastHighlightedQuotedTweet = string.Empty;
        public static string ClipboardImagePath = string.Empty;

        public static void ResetStaticProperties(){
            LastRightClickedLink = LastHighlightedTweet = LastHighlightedQuotedTweet = ClipboardImagePath = string.Empty;
        }

        private readonly FormBrowser form;
        private readonly FormNotification notification;

        public TweetDeckBridge(FormBrowser form, FormNotification notification){
            this.form = form;
            this.notification = notification;
        }

        public void LoadFontSizeClass(string fsClass){
            form.InvokeAsyncSafe(() => {
               TweetNotification.SetFontSizeClass(fsClass);
            });
        }

        public void LoadNotificationHeadContents(string headContents){
            form.InvokeAsyncSafe(() => {
               TweetNotification.SetHeadTag(headContents); 
            });
        }

        public void SetLastRightClickedLink(string link){
            form.InvokeAsyncSafe(() => LastRightClickedLink = link);
        }

        public void SetLastHighlightedTweet(string link, string quotedLink){
            form.InvokeAsyncSafe(() => {
                LastHighlightedTweet = link;
                LastHighlightedQuotedTweet = quotedLink;
            });
        }

        public void SetNotificationQuotedTweet(string link){
            notification.InvokeAsyncSafe(() => notification.CurrentQuotedTweetUrl = link);
        }

        public void OpenSettingsMenu(){
            form.InvokeAsyncSafe(form.OpenSettings);
        }

        public void OpenPluginsMenu(){
            form.InvokeAsyncSafe(form.OpenPlugins);
        }

        public void OnTweetPopup(string tweetHtml, string tweetUrl, int tweetCharacters){
            notification.InvokeAsyncSafe(() => {
                form.OnTweetNotification();
                notification.ShowNotification(new TweetNotification(tweetHtml, tweetUrl, tweetCharacters));
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

        public void LoadNextNotification(){
            notification.InvokeAsyncSafe(notification.FinishCurrentTweet);
        }

        public void TryPasteImage(){
            form.InvokeSafe(() => {
                if (Clipboard.ContainsImage()){
                    Image img = Clipboard.GetImage();
                    if (img == null)return;

                    try{
                        Directory.CreateDirectory(Program.TemporaryPath);

                        ClipboardImagePath = Path.Combine(Program.TemporaryPath, "TD-Img-"+DateTime.Now.Ticks+".png");
                        img.Save(ClipboardImagePath, ImageFormat.Png);

                        form.OnImagePasted();
                    }catch(Exception e){
                        Program.Reporter.HandleException("Clipboard Image Error", "Could not paste image from clipboard.", true, e);
                    }
                }
            });
        }

        public void ClickUploadImage(int offsetX, int offsetY){
            form.InvokeAsyncSafe(() => form.TriggerImageUpload(offsetX, offsetY));
        }

        public void ScreenshotTweet(string html, int width, int height){
            form.InvokeAsyncSafe(() => form.OnTweetScreenshotReady(html, width, height));
        }

        public void FixClipboard(){
            form.InvokeAsyncSafe(WindowsUtils.ClipboardStripHtmlStyles);
        }

        public void OpenBrowser(string url){
            BrowserUtils.OpenExternalBrowser(url);
        }

        public void Alert(string type, string contents){
            MessageBoxIcon icon;

            switch(type){
                case "error": icon = MessageBoxIcon.Error; break;
                case "warning": icon = MessageBoxIcon.Warning; break;
                case "info": icon = MessageBoxIcon.Information; break;
                default: icon = MessageBoxIcon.None; break;
            }

            MessageBox.Show(contents, Program.BrandName+" Browser Message", MessageBoxButtons.OK, icon);
        }

        public void Log(string data){
            System.Diagnostics.Debug.WriteLine(data);
        }
    }
}
