using CefSharp;
using CefSharp.WinForms;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetDuck.Browser.Base {
	sealed class CefBrowserAdapter : IBrowserWrapper<IFrame> {
		public string Url => browser.Address;
		public IFrame MainFrame => browser.GetMainFrame();

		private readonly ChromiumWebBrowser browser;

		public CefBrowserAdapter(ChromiumWebBrowser browser) {
			this.browser = browser;
		}

		public void AddWordToDictionary(string word) {
			browser.AddWordToDictionary(word);
		}
	}
}
