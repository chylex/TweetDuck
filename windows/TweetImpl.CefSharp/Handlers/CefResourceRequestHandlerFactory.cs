using System.Diagnostics.CodeAnalysis;
using CefSharp;
using TweetImpl.CefSharp.Adapters;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Logic;
using IResourceRequestHandler = TweetLib.Browser.Interfaces.IResourceRequestHandler;

namespace TweetImpl.CefSharp.Handlers {
	sealed class CefResourceRequestHandlerFactory : IResourceRequestHandlerFactory {
		bool IResourceRequestHandlerFactory.HasHandlers => true;

		private readonly ResourceRequestHandlerFactoryLogic<CefResourceRequestHandler, IResourceHandler, IRequest> logic;

		public CefResourceRequestHandlerFactory(IResourceRequestHandler? resourceRequestHandler, ResourceHandlerRegistry<IResourceHandler> registry) {
			this.logic = new ResourceRequestHandlerFactoryLogic<CefResourceRequestHandler, IResourceHandler, IRequest>(CefRequestAdapter.Instance, new CefResourceRequestHandler(registry, resourceRequestHandler), registry);
		}

		[SuppressMessage("ReSharper", "RedundantAssignment")]
		global::CefSharp.IResourceRequestHandler IResourceRequestHandlerFactory.GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) {
			return logic.GetResourceRequestHandler(request, ref disableDefaultHandling);
		}
	}
}
