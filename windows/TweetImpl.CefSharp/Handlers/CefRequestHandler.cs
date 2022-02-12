using CefSharp;
using CefSharp.Handler;
using TweetImpl.CefSharp.Adapters;
using TweetLib.Browser.CEF.Logic;

namespace TweetImpl.CefSharp.Handlers {
	sealed class CefRequestHandler : RequestHandler {
		public RequestHandlerLogic<IRequest> Logic { get; }

		private readonly bool autoReload;

		public CefRequestHandler(CefLifeSpanHandler lifeSpanHandler, bool autoReload) {
			this.Logic = new RequestHandlerLogic<IRequest>(CefRequestAdapter.Instance, lifeSpanHandler.Logic);
			this.autoReload = autoReload;
		}

		protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect) {
			return Logic.OnBeforeBrowse(request, userGesture);
		}

		protected override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture) {
			return Logic.OnOpenUrlFromTab(targetUrl, userGesture, CefLifeSpanHandler.ConvertTargetDisposition(targetDisposition));
		}

		protected override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status) {
			if (autoReload) {
				browser.Reload();
			}
		}
	}
}
