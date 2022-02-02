using CefSharp;
using CefSharp.Handler;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Logic;
using IResourceRequestHandler = TweetLib.Browser.Interfaces.IResourceRequestHandler;

namespace TweetDuck.Browser.Base {
	sealed class CefResourceRequestHandler : ResourceRequestHandler {
		private readonly ResourceRequestHandlerLogic<IRequest, IResponse, IResourceHandler> logic;

		public CefResourceRequestHandler(ResourceHandlerRegistry<IResourceHandler> resourceHandlerRegistry, IResourceRequestHandler resourceRequestHandler) {
			this.logic = new ResourceRequestHandlerLogic<IRequest, IResponse, IResourceHandler>(CefRequestAdapter.Instance, CefResponseAdapter.Instance, resourceHandlerRegistry, resourceRequestHandler);
		}

		protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback) {
			return logic.OnBeforeResourceLoad(request, callback) ? CefReturnValue.Continue : CefReturnValue.Cancel;
		}

		protected override IResourceHandler GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request) {
			return logic.GetResourceHandler(request);
		}

		protected override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response) {
			return CefResponseFilter.Create(logic.GetResourceResponseFilter(request, response));
		}

		protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength) {
			logic.OnResourceLoadComplete(request);
		}
	}
}
