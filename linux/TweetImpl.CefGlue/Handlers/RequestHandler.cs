using TweetImpl.CefGlue.Adapters;
using TweetLib.Browser.CEF.Logic;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers {
	sealed class RequestHandler : CefRequestHandler {
		public RequestHandlerLogic<CefRequest> Logic { get; }

		private readonly ResourceRequestHandlerFactory resourceRequestHandlerFactory;
		private readonly bool autoReload;

		public RequestHandler(LifeSpanHandler lifeSpanHandler, ResourceRequestHandlerFactory resourceRequestHandlerFactory, bool autoReload) {
			this.Logic = new RequestHandlerLogic<CefRequest>(CefRequestAdapter.Instance, lifeSpanHandler.Logic);
			this.resourceRequestHandlerFactory = resourceRequestHandlerFactory;
			this.autoReload = autoReload;
		}

		protected override bool OnBeforeBrowse(CefBrowser browser, CefFrame frame, CefRequest request, bool userGesture, bool isRedirect) {
			return Logic.OnBeforeBrowse(request, userGesture);
		}

		protected override bool OnOpenUrlFromTab(CefBrowser browser, CefFrame frame, string targetUrl, CefWindowOpenDisposition targetDisposition, bool userGesture) {
			return Logic.OnOpenUrlFromTab(targetUrl, userGesture, LifeSpanHandler.ConvertTargetDisposition(targetDisposition));
		}

		protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) {
			return resourceRequestHandlerFactory.GetResourceRequestHandler(request, ref disableDefaultHandling);
		}

		protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status) {
			if (autoReload) {
				browser.Reload();
			}
		}
	}
}
