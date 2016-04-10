using System.Windows.Forms;
namespace TweetDick.Core{
    class TweetDeckBridge{
        private readonly FormBrowser form;

        public TweetDeckBridge(FormBrowser form){
            this.form = form;
        }

        public void Log(string data){
            System.Diagnostics.Debug.WriteLine(data);
        }
    }
}
