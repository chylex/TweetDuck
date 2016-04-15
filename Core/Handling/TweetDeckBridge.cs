using TweetDck.Core.Utils;

namespace TweetDck.Core.Handling{
    class TweetDeckBridge{
        private readonly FormBrowser form;

        public string BrandName{
            get{
                return Program.BrandName;
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

        public void OpenBrowser(string url){
            BrowserUtils.OpenExternalBrowser(url);
        }

        public void Log(string data){
            System.Diagnostics.Debug.WriteLine(data);
        }
    }
}
