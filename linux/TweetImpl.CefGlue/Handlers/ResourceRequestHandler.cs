using TweetImpl.CefGlue.Adapters;
using TweetImpl.CefGlue.Handlers.Resource;
using TweetImpl.CefGlue.Utils;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Logic;
using TweetLib.Browser.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers {
	sealed class ResourceRequestHandler : CefResourceRequestHandler {
		private readonly ResourceRequestHandlerLogic<CefRequest, CefResponse, CefResourceHandler> logic;
		private readonly BridgeObjectRegistry bridgeObjectRegistry;

		public ResourceRequestHandler(ResourceHandlerRegistry<CefResourceHandler> resourceHandlerRegistry, BridgeObjectRegistry bridgeObjectRegistry, IResourceRequestHandler? resourceRequestHandler) {
			this.logic = new ResourceRequestHandlerLogic<CefRequest, CefResponse, CefResourceHandler>(CefRequestAdapter.Instance, CefResponseAdapter.Instance, resourceHandlerRegistry, resourceRequestHandler);
			this.bridgeObjectRegistry = bridgeObjectRegistry;
		}

		protected override CefCookieAccessFilter? GetCookieAccessFilter(CefBrowser browser, CefFrame frame, CefRequest request) {
			return null;
		}

		protected override CefReturnValue OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request, CefCallback callback) {
			return logic.OnBeforeResourceLoad(request, callback) ? CefReturnValue.Continue : CefReturnValue.Cancel;
		}

		protected override CefResourceHandler? GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request) {
			if (request.ResourceType == CefResourceType.MainFrame) {
				return new DisableCspResourceHandler(frame);
			}

			if (SchemeHandlerFactory.TryGetHandler(request) is {} schemeHandler) {
				return schemeHandler;
			}

			if (bridgeObjectRegistry.TryGetHandler(request) is {} bridgeHandler) {
				return bridgeHandler;
			}

			return logic.GetResourceHandler(request);
		}

		protected override ResponseFilter? GetResourceResponseFilter(CefBrowser browser, CefFrame frame, CefRequest request, CefResponse response) {
			var filter = logic.GetResourceResponseFilter(request, response);
			return filter == null ? null : new ResponseFilter(filter);
		}

		protected override void OnResourceLoadComplete(CefBrowser browser, CefFrame frame, CefRequest request, CefResponse response, CefUrlRequestStatus status, long receivedContentLength) {
			logic.OnResourceLoadComplete(request);
		}
	}
}
