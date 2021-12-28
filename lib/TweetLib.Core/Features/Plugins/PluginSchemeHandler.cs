using System;
using System.Linq;
using System.Net;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Resources;

namespace TweetLib.Core.Features.Plugins {
	public sealed class PluginSchemeHandler<T> : ICustomSchemeHandler<T> where T : class {
		public string Protocol => "tdp";

		private readonly CachingResourceProvider<T> resourceProvider;
		private readonly PluginBridge bridge;

		public PluginSchemeHandler(CachingResourceProvider<T> resourceProvider, PluginManager pluginManager) {
			this.resourceProvider = resourceProvider;
			this.bridge = pluginManager.bridge;
		}

		public T? Resolve(Uri uri) {
			if (!uri.IsAbsoluteUri || uri.Scheme != Protocol || !int.TryParse(uri.Authority, out var identifier)) {
				return null;
			}

			var segments = uri.Segments.Select(static segment => segment.TrimEnd('/')).Where(static segment => !string.IsNullOrEmpty(segment)).ToArray();

			if (segments.Length > 0) {
				var handler = segments[0] switch {
					"root" => DoReadRootFile(identifier, segments),
					_      => null
				};

				if (handler != null) {
					return handler;
				}
			}

			return resourceProvider.Status(HttpStatusCode.BadRequest, "Bad URL path: " + uri.AbsolutePath);
		}

		private T DoReadRootFile(int identifier, string[] segments) {
			string path = string.Join("/", segments, 1, segments.Length - 1);

			Plugin? plugin = bridge.GetPluginFromToken(identifier);
			string fullPath = plugin == null ? string.Empty : plugin.GetFullPathIfSafe(PluginFolder.Root, path);
			return fullPath.Length == 0 ? resourceProvider.Status(HttpStatusCode.Forbidden, "File path has to be relative to the plugin root folder.") : resourceProvider.CachedFile(fullPath);
		}
	}
}
