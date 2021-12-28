using System.IO;
using System.Net;
using System.Text;
using CefSharp;
using TweetLib.Browser.Interfaces;

namespace TweetDuck.Browser.Adapters {
	internal sealed class ResourceProvider : IResourceProvider<IResourceHandler> {
		public IResourceHandler Status(HttpStatusCode code, string message) {
			var handler = CreateHandler(Encoding.UTF8.GetBytes(message));
			handler.StatusCode = (int) code;
			return handler;
		}

		public IResourceHandler File(byte[] contents, string extension) {
			if (contents.Length == 0) {
				return Status(HttpStatusCode.NoContent, "File is empty."); // FromByteArray crashes CEF internals with no contents
			}

			var handler = CreateHandler(contents);
			handler.MimeType = Cef.GetMimeType(extension);
			return handler;
		}

		private static ResourceHandler CreateHandler(byte[] bytes) {
			var handler = ResourceHandler.FromStream(new MemoryStream(bytes), autoDisposeStream: true);
			handler.Headers.Set("Access-Control-Allow-Origin", "*");
			return handler;
		}
	}
}
