using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Adapters {
	sealed class CefResponseAdapter : IResponseAdapter<IResponse> {
		public static CefResponseAdapter Instance { get; } = new CefResponseAdapter();

		private CefResponseAdapter() {}

		public void SetCharset(IResponse response, string charset) {
			response.Charset = charset;
		}

		public void SetMimeType(IResponse response, string mimeType) {
			response.MimeType = mimeType;
		}

		public void SetStatus(IResponse response, int statusCode, string statusText) {
			response.StatusCode = statusCode;
			response.StatusText = statusText;
		}

		public void SetHeader(IResponse response, string header, string value) {
			response.SetHeaderByName(header, value, overwrite: true);
		}

		public string? GetHeader(IResponse response, string header) {
			return response.Headers[header];
		}
	}
}
