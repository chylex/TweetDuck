using System;
using System.Net;
using TweetLib.Browser.Interfaces;
using TweetLib.Browser.Request;
using TweetLib.Core.Resources;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features.TweetDeck {
	public sealed class TweetDuckSchemeHandler : ICustomSchemeHandler {
		private static readonly SchemeResource InvalidUrl = new SchemeResource.Status(HttpStatusCode.NotFound, "Invalid URL.");
		private static readonly SchemeResource PathMustBeRelativeToRoot = new SchemeResource.Status(HttpStatusCode.Forbidden, "File path has to be relative to the root folder.");

		public string Protocol => "td";

		private readonly ResourceCache resourceCache;

		public TweetDuckSchemeHandler(ResourceCache resourceCache) {
			this.resourceCache = resourceCache;
		}

		public SchemeResource Resolve(Uri uri) {
			string? rootPath = uri.Authority switch {
				"resources" => App.ResourcesPath,
				"guide"     => App.GuidePath,
				_           => null
			};

			if (rootPath == null) {
				return InvalidUrl;
			}

			string filePath = FileUtils.ResolveRelativePathSafely(rootPath, uri.AbsolutePath.TrimStart('/'));
			return filePath.Length == 0 ? PathMustBeRelativeToRoot : resourceCache.ReadFile(filePath);
		}
	}
}
