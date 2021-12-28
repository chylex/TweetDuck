using System;
using System.Collections.Concurrent;
using System.Text;
using CefSharp;

namespace TweetDuck.Browser.Adapters {
	internal sealed class CefResourceHandlerRegistry {
		private readonly ConcurrentDictionary<string, Func<IResourceHandler>> resourceHandlers = new ConcurrentDictionary<string, Func<IResourceHandler>>(StringComparer.OrdinalIgnoreCase);

		public bool HasHandler(string url) {
			return resourceHandlers.ContainsKey(url);
		}

		public IResourceHandler GetHandler(string url) {
			return resourceHandlers.TryGetValue(url, out var handler) ? handler() : null;
		}

		private void Register(string url, Func<IResourceHandler> factory) {
			if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri)) {
				throw new ArgumentException("Resource handler URL must be absolute!");
			}

			resourceHandlers.AddOrUpdate(uri.AbsoluteUri, factory, (key, prev) => factory);
		}

		public void RegisterStatic(string url, byte[] staticData, string mimeType = ResourceHandler.DefaultMimeType) {
			Register(url, () => ResourceHandler.FromByteArray(staticData, mimeType));
		}

		public void RegisterStatic(string url, string staticData, string mimeType = ResourceHandler.DefaultMimeType) {
			Register(url, () => ResourceHandler.FromString(staticData, Encoding.UTF8, mimeType: mimeType));
		}

		public void RegisterDynamic(string url, IResourceHandler handler) {
			Register(url, () => handler);
		}

		public void Unregister(string url) {
			resourceHandlers.TryRemove(url, out _);
		}
	}
}
