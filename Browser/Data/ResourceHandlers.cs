using System;
using System.Collections.Concurrent;
using CefSharp;

namespace TweetDuck.Browser.Data {
	sealed class ResourceHandlers {
		private readonly ConcurrentDictionary<string, Func<IResourceHandler>> handlers = new ConcurrentDictionary<string, Func<IResourceHandler>>(StringComparer.OrdinalIgnoreCase);

		public bool HasHandler(IRequest request) {
			return handlers.ContainsKey(request.Url);
		}

		public IResourceHandler GetHandler(IRequest request) {
			return handlers.TryGetValue(request.Url, out var factory) ? factory() : null;
		}

		public bool Register(string url, Func<IResourceHandler> factory) {
			if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri)) {
				handlers.AddOrUpdate(uri.AbsoluteUri, factory, (key, prev) => factory);
				return true;
			}

			return false;
		}

		public bool Register(ResourceLink link) {
			return Register(link.Url, link.Factory);
		}

		public bool Unregister(string url) {
			return handlers.TryRemove(url, out _);
		}

		public bool Unregister(ResourceLink link) {
			return Unregister(link.Url);
		}

		public static Func<IResourceHandler> ForString(string str) {
			return () => ResourceHandler.FromString(str);
		}

		public static Func<IResourceHandler> ForString(string str, string mimeType) {
			return () => ResourceHandler.FromString(str, mimeType: mimeType);
		}

		public static Func<IResourceHandler> ForBytes(byte[] bytes, string mimeType) {
			return () => ResourceHandler.FromByteArray(bytes, mimeType);
		}
	}
}
