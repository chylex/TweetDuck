using TweetImpl.CefGlue.Adapters;
using TweetImpl.CefGlue.Utils;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Logic;
using TweetLib.Browser.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers {
	sealed class ResourceRequestHandlerFactory {
		private readonly ResourceRequestHandlerFactoryLogic<ResourceRequestHandler, CefResourceHandler, CefRequest> logic;

		public ResourceRequestHandlerFactory(IResourceRequestHandler? resourceRequestHandler, ResourceHandlerRegistry<CefResourceHandler> resourceHandlerRegistry, BridgeObjectRegistry bridgeObjectRegistry) {
			this.logic = new ResourceRequestHandlerFactoryLogic<ResourceRequestHandler, CefResourceHandler, CefRequest>(CefRequestAdapter.Instance, new ResourceRequestHandler(resourceHandlerRegistry, bridgeObjectRegistry, resourceRequestHandler), resourceHandlerRegistry);
		}

		public CefResourceRequestHandler GetResourceRequestHandler(CefRequest request, ref bool disableDefaultHandling) {
			return logic.GetResourceRequestHandler(request, ref disableDefaultHandling);
		}
	}
}
