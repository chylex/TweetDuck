using System;
using System.Collections.Concurrent;
using System.Text;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetLib.Browser.CEF.Data {
	public sealed class ResourceHandlerRegistry<TResourceHandler> where TResourceHandler : class {
		private readonly IResourceHandlerFactory<TResourceHandler> factory;
		private readonly ConcurrentDictionary<string, Func<TResourceHandler>> resourceHandlers = new (StringComparer.OrdinalIgnoreCase);

		public ResourceHandlerRegistry(IResourceHandlerFactory<TResourceHandler> factory) {
			this.factory = factory;
		}

		internal bool HasHandler(string url) {
			return resourceHandlers.ContainsKey(url);
		}

		internal TResourceHandler? GetHandler(string url) {
			return resourceHandlers.TryGetValue(url, out var handler) ? handler() : null;
		}

		private void Register(string url, Func<TResourceHandler> factory) {
			if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) {
				throw new ArgumentException("Resource handler URL must be absolute!");
			}

			resourceHandlers.AddOrUpdate(uri.AbsoluteUri, factory, (_, _) => factory);
		}

		public void RegisterStatic(string url, byte[] staticData, string mimeType = "text/html") {
			Register(url, () => factory.CreateResourceHandler(new ByteArrayResource(staticData, mimeType)));
		}

		public void RegisterStatic(string url, string staticData, string mimeType = "text/html") {
			Register(url, () => factory.CreateResourceHandler(new ByteArrayResource(staticData, Encoding.UTF8, mimeType)));
		}

		public void RegisterDynamic(string url, TResourceHandler handler) {
			Register(url, () => handler);
		}

		public void Unregister(string url) {
			resourceHandlers.TryRemove(url, out _);
		}
	}
}
