using System.IO;
using System.Net;
using System.Text;
using CefSharp;
using TweetLib.Browser.Interfaces;
using TweetLib.Browser.Request;

namespace TweetDuck.Browser.Adapters {
	internal sealed class CefSchemeResourceVisitor : ISchemeResourceVisitor<IResourceHandler> {
		public static CefSchemeResourceVisitor Instance { get; } = new CefSchemeResourceVisitor();

		private static readonly SchemeResource.Status FileIsEmpty = new SchemeResource.Status(HttpStatusCode.NoContent, "File is empty.");

		private CefSchemeResourceVisitor() {}

		public IResourceHandler Status(SchemeResource.Status status) {
			var handler = CreateHandler(Encoding.UTF8.GetBytes(status.Message));
			handler.StatusCode = (int) status.Code;
			return handler;
		}

		public IResourceHandler File(SchemeResource.File file) {
			byte[] contents = file.Contents;
			if (contents.Length == 0) {
				return Status(FileIsEmpty); // FromByteArray crashes CEF internals with no contents
			}

			var handler = CreateHandler(contents);
			handler.MimeType = Cef.GetMimeType(file.Extension);
			return handler;
		}

		private static ResourceHandler CreateHandler(byte[] bytes) {
			var handler = ResourceHandler.FromStream(new MemoryStream(bytes), autoDisposeStream: true);
			handler.Headers.Set("Access-Control-Allow-Origin", "*");
			return handler;
		}
	}
}
