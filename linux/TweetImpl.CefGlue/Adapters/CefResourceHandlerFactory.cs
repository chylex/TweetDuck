using TweetImpl.CefGlue.Handlers.Resource;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefResourceHandlerFactory : IResourceHandlerFactory<CefResourceHandler> {
		public static CefResourceHandlerFactory Instance { get; } = new ();

		private CefResourceHandlerFactory() {}

		public CefResourceHandler CreateResourceHandler(ByteArrayResource resource) {
			return new ByteArrayResourceHandler(resource);
		}

		public string GetMimeTypeFromExtension(string extension) {
			return CefRuntime.GetMimeType(extension);
		}
	}
}
