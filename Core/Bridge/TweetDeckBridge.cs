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

        private readonly FormBrowser form;
        private readonly FormNotification notification;

        public string BrandName{
            get{
                return Program.BrandName;
            }
        }

        public string VersionTag{
            get{
                return Program.VersionTag;
            }
        }

        public bool MuteNotifications{
            get{
                return Program.UserConfig.MuteNotifications;
            }
        }

        public bool HasCustomNotificationSound{
            get{
                return !string.IsNullOrEmpty(Program.UserConfig.NotificationSoundPath);
            }
        }

        public bool ExpandLinksOnHover{
            get{
                return Program.UserConfig.ExpandLinksOnHover;
            }
        }

        public bool HasCustomBrowserCSS{
            get{
                return !string.IsNullOrEmpty(Program.UserConfig.CustomBrowserCSS);
            }
        }

        public string CustomBrowserCSS{
            get{
                return Program.UserConfig.CustomBrowserCSS;
            }
        }

        public TweetDeckBridge(FormBrowser form, FormNotification notification){
            this.form = form;
            this.notification = notification;
        }

        public void LoadFontSizeClass(string fsClass){
            form.InvokeSafe(() => {
               TweetNotification.SetFontSizeClass(fsClass);
            });
        }

        public void LoadNotificationHeadContents(string headContents){
            form.InvokeSafe(() => {
               TweetNotification.SetHeadTag(headContents); 
            });
        }

        public void SetLastRightClickedLink(string link){
            form.InvokeSafe(() => LastRightClickedLink = link);
        }

        public void SetLastHighlightedTweet(string link, string quotedLink){
            form.InvokeSafe(() => {
                LastHighlightedTweet = link;
                LastHighlightedQuotedTweet = quotedLink;
            });
        }

        public void SetNotificationQuotedTweet(string link){
            notification.InvokeSafe(() => notification.CurrentQuotedTweetUrl = link);
        }

        public void OpenSettingsMenu(){
            form.InvokeSafe(form.OpenSettings);
        }

        public void OpenPluginsMenu(){
            form.InvokeSafe(form.OpenPlugins);
        }

        public void OnTweetPopup(string tweetHtml, string tweetUrl, int tweetCharacters){
            notification.InvokeSafe(() => {
                form.OnTweetNotification();
                notification.ShowNotification(new TweetNotification(tweetHtml, tweetUrl, tweetCharacters));
            });
        }

        public void OnTweetSound(){
            form.InvokeSafe(() => {
                form.OnTweetNotification();
                form.PlayNotificationSound();
            });
        }

        public void OnNotificationReady(){
            if (!Program.UserConfig.NotificationLegacyLoad){
                notification.InvokeSafe(notification.OnNotificationReady);
            }
        }

        public void DisplayTooltip(string text, bool showInNotification){
            if (showInNotification){
                notification.InvokeSafe(() => notification.DisplayTooltip(text));
            }
            else{
                form.InvokeSafe(() => form.DisplayTooltip(text));
            }
        }

        public void LoadNextNotification(){
            notification.InvokeSafe(notification.FinishCurrentTweet);
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
            form.InvokeSafe(() => {
                Point prevPos = Cursor.Position;

                Cursor.Position = form.PointToScreen(new Point(offsetX, offsetY));
                NativeMethods.SimulateMouseClick(NativeMethods.MouseButton.Left);
                Cursor.Position = prevPos;

                form.OnImagePastedFinish();
            });
        }

        public void ScreenshotTweet(string html, int width, int height){
            form.InvokeSafe(() => form.OnTweetScreenshotReady(html, width, height));
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
