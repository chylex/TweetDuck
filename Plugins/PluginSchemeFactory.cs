using CefSharp;
using TweetDuck.Browser.Handling;
using TweetLib.Core.Features.Plugins;

namespace TweetDuck.Plugins {
	sealed class PluginSchemeFactory : ISchemeHandlerFactory {
		public const string Name = PluginSchemeHandler<IResourceHandler>.Name;

		private readonly PluginSchemeHandler<IResourceHandler> handler = new PluginSchemeHandler<IResourceHandler>(new ResourceProvider());

		internal void Setup(PluginManager plugins) {
			handler.Setup(plugins);
		}

		public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request) {
			return handler.Process(request.Url);
		}
	}
}
