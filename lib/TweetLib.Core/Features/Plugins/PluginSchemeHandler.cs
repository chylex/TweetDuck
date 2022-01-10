using System;
using System.Linq;
using System.Net;
using TweetLib.Browser.Interfaces;
using TweetLib.Browser.Request;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Resources;

namespace TweetLib.Core.Features.Plugins {
	public sealed class PluginSchemeHandler : ICustomSchemeHandler {
		private static readonly SchemeResource PathMustBeRelativeToRoot = new SchemeResource.Status(HttpStatusCode.Forbidden, "File path has to be relative to the plugin root folder.");

		public string Protocol => "tdp";

		private readonly ResourceCache resourceCache;
		private readonly PluginBridge bridge;

		public PluginSchemeHandler(ResourceCache resourceCache, PluginManager pluginManager) {
			this.resourceCache = resourceCache;
			this.bridge = pluginManager.bridge;
		}

		public SchemeResource? Resolve(Uri uri) {
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

			return new SchemeResource.Status(HttpStatusCode.BadRequest, "Bad URL path: " + uri.AbsolutePath);
		}

		private SchemeResource DoReadRootFile(int identifier, string[] segments) {
			string path = string.Join("/", segments, 1, segments.Length - 1);

			Plugin? plugin = bridge.GetPluginFromToken(identifier);
			string fullPath = plugin == null ? string.Empty : plugin.GetFullPathIfSafe(PluginFolder.Root, path);
			return fullPath.Length == 0 ? PathMustBeRelativeToRoot : resourceCache.ReadFile(fullPath);
		}
	}
}
