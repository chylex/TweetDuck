using System.Windows.Forms;
using CefSharp;

namespace TweetDuck.Core.Handling{
    sealed class KeyboardHandlerBrowser : KeyboardHandlerBase{
        private readonly FormBrowser form;

        public KeyboardHandlerBrowser(FormBrowser form){
            this.form = form;
        }

        protected override bool HandleRawKey(IWebBrowser browserControl, IBrowser browser, Keys key, CefEventFlags modifiers){
            if (base.HandleRawKey(browserControl, browser, key, modifiers)){
                return true;
            }

            return form.ProcessBrowserKey(key);
        }
    }
}
