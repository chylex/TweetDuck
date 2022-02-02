using CefSharp;
using CefSharp.WinForms;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetDuck.Browser.Base {
	sealed class CefBrowserAdapter : IBrowserWrapper<IFrame, IRequest> {
		public string Url => browser.Address;
		public IFrame MainFrame => browser.GetMainFrame();

		private readonly ChromiumWebBrowser browser;

		public CefBrowserAdapter(ChromiumWebBrowser browser) {
			this.browser = browser;
		}

		public void AddWordToDictionary(string word) {
			browser.AddWordToDictionary(word);
		}

		public IRequest CreateGetRequest() {
			using var frame = MainFrame;
			return frame.CreateRequest(false);
		}

		public void RequestDownload(IFrame frame, IRequest request, DownloadCallbacks callbacks) {
			frame.CreateUrlRequest(request, new CefDownloadRequestClient(callbacks));
		}
	}
}
