using System;
using System.Net;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Resources;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features.TweetDeck {
	public sealed class TweetDuckSchemeHandler<T> : ICustomSchemeHandler<T> where T : class {
		public string Protocol => "td";

		private readonly CachingResourceProvider<T> resourceProvider;

		public TweetDuckSchemeHandler(CachingResourceProvider<T> resourceProvider) {
			this.resourceProvider = resourceProvider;
		}

		public T Resolve(Uri uri) {
			string? rootPath = uri.Authority switch {
				"resources" => App.ResourcesPath,
				"guide"     => App.GuidePath,
				_           => null
			};

			if (rootPath == null) {
				return resourceProvider.Status(HttpStatusCode.NotFound, "Invalid URL.");
			}

			string filePath = FileUtils.ResolveRelativePathSafely(rootPath, uri.AbsolutePath.TrimStart('/'));
			return filePath.Length == 0 ? resourceProvider.Status(HttpStatusCode.Forbidden, "File path has to be relative to the root folder.") : resourceProvider.CachedFile(filePath);
		}
	}
}
