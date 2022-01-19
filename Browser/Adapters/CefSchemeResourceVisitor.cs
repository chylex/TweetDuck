using System;
using System.IO;
using System.Net;
using CefSharp;
using TweetLib.Browser.Interfaces;
using TweetLib.Browser.Request;

namespace TweetDuck.Browser.Adapters {
	sealed class CefSchemeResourceVisitor : ISchemeResourceVisitor<IResourceHandler> {
		public static CefSchemeResourceVisitor Instance { get; } = new CefSchemeResourceVisitor();

		private static readonly SchemeResource.Status FileIsEmpty = new SchemeResource.Status(HttpStatusCode.NoContent, "File is empty.");

		private CefSchemeResourceVisitor() {}

		public IResourceHandler Status(SchemeResource.Status status) {
			var handler = CreateHandler(Array.Empty<byte>());
			handler.StatusCode = (int) status.Code;
			handler.StatusText = status.Message;
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
			return ResourceHandler.FromStream(new MemoryStream(bytes), autoDisposeStream: true);
		}
	}
}
