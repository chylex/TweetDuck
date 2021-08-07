using System.Diagnostics.CodeAnalysis;
using CefSharp;
using TweetDuck.Browser.Data;

namespace TweetDuck.Browser.Handling {
	abstract class ResourceRequestHandler : CefSharp.Handler.ResourceRequestHandler {
		private class SelfFactoryImpl : IResourceRequestHandlerFactory {
			private readonly ResourceRequestHandler me;

			public SelfFactoryImpl(ResourceRequestHandler me) {
				this.me = me;
			}

			bool IResourceRequestHandlerFactory.HasHandlers => true;

			[SuppressMessage("ReSharper", "RedundantAssignment")]
			IResourceRequestHandler IResourceRequestHandlerFactory.GetResourceRequestHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) {
				disableDefaultHandling = me.ResourceHandlers.HasHandler(request);
				return me;
			}
		}

		public IResourceRequestHandlerFactory SelfFactory { get; }
		public ResourceHandlers ResourceHandlers { get; }

		protected ResourceRequestHandler() {
			this.SelfFactory = new SelfFactoryImpl(this);
			this.ResourceHandlers = new ResourceHandlers();
		}

		protected override IResourceHandler GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request) {
			return ResourceHandlers.GetHandler(request);
		}
	}
}
