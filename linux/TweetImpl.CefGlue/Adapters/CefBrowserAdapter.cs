using Lunixo.ChromiumGtk.Core;
using TweetImpl.CefGlue.Handlers;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefBrowserAdapter : IBrowserWrapper<CefFrame, CefRequest> {
		public string Url { get; private set; } = "";
		public CefFrame MainFrame => browser.CefBrowser.GetMainFrame();

		private readonly WebBrowser browser;

		public CefBrowserAdapter(WebBrowser browser) {
			this.browser = browser;
			this.browser.Client.DisplayHandler = new DisplayHandler(this);
		}

		public void AddWordToDictionary(string word) {
			using var host = browser.CefBrowser.GetHost();
			host.AddWordToDictionary(word);
		}

		public CefRequest CreateGetRequest() {
			return CefRequest.Create();
		}

		public void RequestDownload(CefFrame frame, CefRequest request, DownloadCallbacks callbacks) {
			frame.CreateUrlRequest(request, new DownloadRequestClient(callbacks));
		}

		private sealed class DisplayHandler : CefDisplayHandler {
			private readonly CefBrowserAdapter browser;

			public DisplayHandler(CefBrowserAdapter browser) {
				this.browser = browser;
			}

			protected override void OnAddressChange(CefBrowser browser, CefFrame frame, string url) {
				this.browser.Url = url;
			}
		}
	}
}
