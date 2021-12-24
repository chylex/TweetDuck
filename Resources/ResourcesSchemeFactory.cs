using System;
using System.Net;
using CefSharp;
using TweetLib.Core.Browser;
using TweetLib.Core.Utils;

namespace TweetDuck.Resources {
	public class ResourceSchemeFactory : ISchemeHandlerFactory {
		public const string Name = "td";

		private readonly IResourceProvider<IResourceHandler> resourceProvider;

		public ResourceSchemeFactory(IResourceProvider<IResourceHandler> resourceProvider) {
			this.resourceProvider = resourceProvider;
		}

		public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request) {
			if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) || uri.Scheme != Name) {
				return null;
			}

			string rootPath = uri.Authority switch {
				"resources" => Program.ResourcesPath,
				"guide"     => Program.GuidePath,
				_           => null
			};

			if (rootPath == null) {
				return resourceProvider.Status(HttpStatusCode.NotFound, "Invalid URL.");
			}

			string filePath = FileUtils.ResolveRelativePathSafely(rootPath, uri.AbsolutePath.TrimStart('/'));
			return filePath.Length == 0 ? resourceProvider.Status(HttpStatusCode.Forbidden, "File path has to be relative to the root folder.") : resourceProvider.File(filePath);
		}
	}
}
