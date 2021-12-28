using System.Diagnostics.CodeAnalysis;
using CefSharp;
using IResourceRequestHandler = TweetLib.Browser.Interfaces.IResourceRequestHandler;

namespace TweetDuck.Browser.Adapters {
	sealed class CefResourceHandlerFactory : IResourceRequestHandlerFactory {
		bool IResourceRequestHandlerFactory.HasHandlers => registry != null;

		private readonly CefResourceRequestHandler resourceRequestHandler;
		private readonly CefResourceHandlerRegistry registry;

		public CefResourceHandlerFactory(IResourceRequestHandler resourceRequestHandler, CefResourceHandlerRegistry registry) {
			this.resourceRequestHandler = new CefResourceRequestHandler(registry, resourceRequestHandler);
			this.registry = registry;
		}

		[SuppressMessage("ReSharper", "RedundantAssignment")]
		CefSharp.IResourceRequestHandler IResourceRequestHandlerFactory.GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) {
			disableDefaultHandling = registry != null && registry.HasHandler(request.Url);
			return resourceRequestHandler;
		}
	}
}
