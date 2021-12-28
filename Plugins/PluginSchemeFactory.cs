using CefSharp;
using TweetDuck.Resources;
using TweetLib.Core.Features.Plugins;

namespace TweetDuck.Plugins {
	sealed class PluginSchemeFactory : ISchemeHandlerFactory {
		public const string Name = PluginSchemeHandler<IResourceHandler>.Name;

		private readonly PluginSchemeHandler<IResourceHandler> handler;

		public PluginSchemeFactory(ResourceProvider resourceProvider) {
			handler = new PluginSchemeHandler<IResourceHandler>(resourceProvider);
		}

		internal void Setup(PluginManager plugins) {
			handler.Setup(plugins);
		}

		public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request) {
			return handler.Process(request.Url);
		}
	}
}
