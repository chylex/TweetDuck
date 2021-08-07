using System.Windows.Forms;
using CefSharp;

namespace TweetDuck.Browser.Handling {
	sealed class KeyboardHandlerBrowser : KeyboardHandlerBase {
		private readonly FormBrowser form;

		public KeyboardHandlerBrowser(FormBrowser form) {
			this.form = form;
		}

		protected override bool HandleRawKey(IWebBrowser browserControl, Keys key, CefEventFlags modifiers) {
			return base.HandleRawKey(browserControl, key, modifiers) || form.ProcessBrowserKey(key);
		}
	}
}
