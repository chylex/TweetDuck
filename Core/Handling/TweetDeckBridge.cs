using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using TweetDck.Core.Utils;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Handling{
    class TweetDeckBridge{
        public static string LastRightClickedLink = string.Empty;
        public static string LastHighlightedTweet = string.Empty;
        public static string LastHighlightedTweetEmbedded = string.Empty;
        public static string NotificationTweetEmbedded = string.Empty;
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

        public bool ExpandLinksOnHover{
            get{
                return Program.UserConfig.ExpandLinksOnHover;
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

        public void SetLastHighlightedTweet(string link, string embeddedLink){
            form.InvokeSafe(() => {
                LastHighlightedTweet = link;
                LastHighlightedTweetEmbedded = embeddedLink;
            });
        }

        public void SetNotificationTweetEmbedded(string link){
            form.InvokeSafe(() => NotificationTweetEmbedded = link);
        }

        public void OpenSettingsMenu(){
            form.InvokeSafe(form.OpenSettings);
        }

        public void OpenPluginsMenu(){
            form.InvokeSafe(form.OpenPlugins);
        }

        public void OnTweetPopup(string tweetHtml, string tweetUrl, int tweetCharacters){
            notification.InvokeSafe(() => {
                notification.ShowNotification(new TweetNotification(tweetHtml,tweetUrl,tweetCharacters));
            });
        }

        public void OnTweetSound(){
            form.InvokeSafe(form.OnTweetSound);
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

        public void TryPasteImage(){
            form.InvokeSafe(() => {
                if (Clipboard.ContainsImage()){
                    Image img = Clipboard.GetImage();
                    if (img == null)return;

                    try{
                        Directory.CreateDirectory(Program.TemporaryPath);

                        ClipboardImagePath = Path.Combine(Program.TemporaryPath,"TD-Img-"+DateTime.Now.Ticks+".png");
                        img.Save(ClipboardImagePath,ImageFormat.Png);

                        form.OnImagePasted();
                    }catch(Exception e){
                        Program.HandleException("Could not paste image from clipboard.",e);
                    }
                }
            });
        }

        public void ClickUploadImage(int offsetX, int offsetY){
            form.InvokeSafe(() => {
                Point prevPos = Cursor.Position;

                Cursor.Position = form.PointToScreen(new Point(offsetX,offsetY));
                NativeMethods.SimulateMouseClick(NativeMethods.MouseButton.Left);
                Cursor.Position = prevPos;

                form.OnImagePastedFinish();
            });
        }

        public void OpenBrowser(string url){
            BrowserUtils.OpenExternalBrowser(url);
        }

        public void Log(string data){
            System.Diagnostics.Debug.WriteLine(data);
        }
    }
}
