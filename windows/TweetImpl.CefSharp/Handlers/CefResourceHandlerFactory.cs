using CefSharp;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Handlers {
	sealed class CefResourceHandlerFactory : IResourceHandlerFactory<IResourceHandler> {
		public static CefResourceHandlerFactory Instance { get; } = new ();

		private CefResourceHandlerFactory() {}

		public IResourceHandler CreateResourceHandler(ByteArrayResource resource) {
			return new CefByteArrayResourceHandler(resource);
		}

		public string GetMimeTypeFromExtension(string extension) {
			return Cef.GetMimeType(extension);
		}
	}
}
