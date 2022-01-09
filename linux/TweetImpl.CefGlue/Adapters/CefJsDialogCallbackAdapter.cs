using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefJsDialogCallbackAdapter : IJsDialogCallbackAdapter<CefJSDialogCallback> {
		public static CefJsDialogCallbackAdapter Instance { get; } = new ();

		private CefJsDialogCallbackAdapter() {}

		public void Continue(CefJSDialogCallback callback, bool success, string? userInput = null) {
			callback.Continue(success, userInput);
		}

		public void Dispose(CefJSDialogCallback callback) {
			callback.Dispose();
		}
	}
}
