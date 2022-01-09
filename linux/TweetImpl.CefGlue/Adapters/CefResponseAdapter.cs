using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefResponseAdapter : IResponseAdapter<CefResponse> {
		public static CefResponseAdapter Instance { get; } = new ();

		private CefResponseAdapter() {}

		public void SetCharset(CefResponse response, string charset) {
			response.Charset = charset;
		}

		public void SetMimeType(CefResponse response, string mimeType) {
			response.MimeType = mimeType;
		}

		public void SetStatus(CefResponse response, int statusCode, string statusText) {
			response.Status = statusCode;
			response.StatusText = statusText;
		}

		public void SetHeader(CefResponse response, string header, string value) {
			response.SetHeaderByName(header, value, overwrite: true);
		}

		public string GetHeader(CefResponse response, string header) {
			return response.GetHeaderMap()[header] ?? string.Empty;
		}
	}
}
