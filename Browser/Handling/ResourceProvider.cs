using System;
using System.IO;
using System.Net;
using System.Text;
using CefSharp;
using TweetLib.Core.Browser;
using IOFile = System.IO.File;

namespace TweetDuck.Browser.Handling {
	internal sealed class ResourceProvider : IResourceProvider<IResourceHandler> {
		private static ResourceHandler CreateHandler(byte[] bytes) {
			var handler = ResourceHandler.FromStream(new MemoryStream(bytes), autoDisposeStream: true);
			handler.Headers.Set("Access-Control-Allow-Origin", "*");
			return handler;
		}

		public IResourceHandler Status(HttpStatusCode code, string message) {
			var handler = CreateHandler(Encoding.UTF8.GetBytes(message));
			handler.StatusCode = (int) code;
			return handler;
		}

		public IResourceHandler File(string path) {
			try {
				return FileContents(System.IO.File.ReadAllBytes(path), Path.GetExtension(path));
			} catch (FileNotFoundException) {
				return Status(HttpStatusCode.NotFound, "File not found.");
			} catch (DirectoryNotFoundException) {
				return Status(HttpStatusCode.NotFound, "Directory not found.");
			} catch (Exception e) {
				return Status(HttpStatusCode.InternalServerError, e.Message);
			}
		}

		private IResourceHandler FileContents(byte[] bytes, string extension) {
			if (bytes.Length == 0) {
				return Status(HttpStatusCode.NoContent, "File is empty."); // FromByteArray crashes CEF internals with no contents
			}
			else {
				var handler = CreateHandler(bytes);
				handler.MimeType = Cef.GetMimeType(extension);
				return handler;
			}
		}
	}
}
