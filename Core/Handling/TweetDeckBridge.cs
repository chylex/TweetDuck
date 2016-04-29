using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Handling{
    class TweetDeckBridge{
        public static string LastRightClickedLink = string.Empty;
        public static string ClipboardImagePath = string.Empty;

        private readonly FormBrowser form;

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

        public bool UpdateCheckEnabled{
            get{
                return Program.UserConfig.EnableUpdateCheck;
            }
        }

        public bool ExpandLinksOnHover{
            get{
                return Program.UserConfig.ExpandLinksOnHover;
            }
        }

        public string DismissedVersionTag{
            get{
                return Program.UserConfig.DismissedUpdate ?? string.Empty;
            }
        }

        public TweetDeckBridge(FormBrowser form){
            this.form = form;
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
            form.InvokeSafe(() => {
                LastRightClickedLink = link; 
            });
        }

        public void OpenSettingsMenu(){
            form.InvokeSafe(() => {
                form.OpenSettings();
            });
        }

        public void OnTweetPopup(string tweetHtml, int tweetCharacters){
            form.InvokeSafe(() => {
                form.OnTweetPopup(new TweetNotification(tweetHtml,tweetCharacters));
            });
        }

        public void OnTweetSound(){
            form.InvokeSafe(() => {
                form.OnTweetSound();
            });
        }

        public void OnUpdateAccepted(string versionTag, string downloadUrl){
            form.InvokeSafe(() => {
                form.BeginUpdateProcess(versionTag,downloadUrl);
            });
        }

        public void OnUpdateDismissed(string versionTag){
            form.InvokeSafe(() => {
                Program.UserConfig.DismissedUpdate = versionTag;
                Program.UserConfig.Save();
            });
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
                Program.mouse_event(0x02,Cursor.Position.X,Cursor.Position.Y,0,0); // MOUSEEVENTF_LEFTDOWN
                Program.mouse_event(0x04,Cursor.Position.X,Cursor.Position.Y,0,0); // MOUSEEVENTF_LEFTUP

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
