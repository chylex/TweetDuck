using TweetDck.Core.Utils;

namespace TweetDck.Core.Handling{
    class TweetDeckBridge{
        public static string LastRightClickedLink = string.Empty;

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

        public bool UpdateCheckEnabled{
            get{
                return Program.UserConfig.EnableUpdateCheck;
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

        public void OpenBrowser(string url){
            BrowserUtils.OpenExternalBrowser(url);
        }

        public void Log(string data){
            System.Diagnostics.Debug.WriteLine(data);
        }
    }
}
