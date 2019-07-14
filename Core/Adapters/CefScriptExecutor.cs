using System.Windows.Forms;
using CefSharp;
using TweetDuck.Resources;
using TweetLib.Core.Browser;

namespace TweetDuck.Core.Adapters{
    sealed class CefScriptExecutor : IScriptExecutor{
        private readonly Control sync;
        private readonly IWebBrowser browser;

        public CefScriptExecutor(Control sync, IWebBrowser browser){
            this.sync = sync;
            this.browser = browser;
        }

        public void RunFunction(string name, params object[] args){
            browser.ExecuteScriptAsync(name, args);
        }

        public void RunScript(string identifier, string script){
            using IFrame frame = browser.GetMainFrame();
            ScriptLoader.ExecuteScript(frame, script, identifier);
        }

        public bool RunFile(string file){
            using IFrame frame = browser.GetMainFrame();
            return ScriptLoader.ExecuteFile(frame, file, sync);
        }
    }
}
