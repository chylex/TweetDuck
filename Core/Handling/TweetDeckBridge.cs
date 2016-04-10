namespace TweetDick.Core.Handling{
    class TweetDeckBridge{
        private readonly FormBrowser form;

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

        public void OnTweetPopup(string tweetHtml, string tweetColumn){
            form.InvokeSafe(() => {
                form.OnTweetPopup(tweetHtml,tweetColumn);
            });
        }

        public void Log(string data){
            System.Diagnostics.Debug.WriteLine(data);
        }
    }
}
