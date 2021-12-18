using System;
using System.IO;
using System.Net;
using CefSharp;
using TweetDuck.Browser.Handling;
using TweetLib.Core.Utils;

namespace TweetDuck.Resources {
	public class ResourceSchemeFactory : ISchemeHandlerFactory {
		public const string Name = "td";

		private static readonly string RootPath = Path.Combine(Program.ResourcesPath);
		private static readonly ResourceProvider ResourceProvider = new ResourceProvider();

		public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request) {
			if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) || uri.Scheme != Name) {
				return null;
			}

			if (uri.Authority != "resources") {
				return null;
			}

			string filePath = FileUtils.ResolveRelativePathSafely(RootPath, uri.AbsolutePath.TrimStart('/'));
			return filePath.Length == 0 ? ResourceProvider.Status(HttpStatusCode.Forbidden, "File path has to be relative to the resources root folder.") : ResourceProvider.File(filePath);
		}
	}
}
