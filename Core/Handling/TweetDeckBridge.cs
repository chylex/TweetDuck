namespace TweetDick.Core.Handling{
    class TweetDeckBridge{
        private readonly FormBrowser form;

        public TweetDeckBridge(FormBrowser form){
            this.form = form;
        }

        public void OpenSettingsMenu(){
            form.OpenSettings();
        }

        public void Log(string data){
            System.Diagnostics.Debug.WriteLine(data);
        }
    }
}
