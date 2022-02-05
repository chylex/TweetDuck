using System;
using System.Net;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.Interfaces;
using TweetLib.Browser.Request;

namespace TweetLib.Browser.CEF.Logic {
	abstract class SchemeResourceVisitor {
		protected static readonly SchemeResource.Status FileIsEmpty = new (HttpStatusCode.NoContent, "File is empty.");
	}

	sealed class SchemeResourceVisitor<TResourceHandler> : SchemeResourceVisitor, ISchemeResourceVisitor<TResourceHandler> {
		private readonly IResourceHandlerFactory<TResourceHandler> factory;

		public SchemeResourceVisitor(IResourceHandlerFactory<TResourceHandler> factory) {
			this.factory = factory;
		}

		public TResourceHandler Status(SchemeResource.Status status) {
			return factory.CreateResourceHandler(new ByteArrayResource(Array.Empty<byte>(), statusCode: status.Code, statusText: status.Message));
		}

		public TResourceHandler File(SchemeResource.File file) {
			byte[] contents = file.Contents;
			return contents.Length == 0 ? Status(FileIsEmpty) : factory.CreateResourceHandler(new ByteArrayResource(contents, factory.GetMimeTypeFromExtension(file.Extension)));
		}
	}
}
