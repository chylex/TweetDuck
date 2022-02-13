using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Adapters {
	sealed class CefJsDialogCallbackAdapter : IJsDialogCallbackAdapter<IJsDialogCallback> {
		public static CefJsDialogCallbackAdapter Instance { get; } = new CefJsDialogCallbackAdapter();

		private CefJsDialogCallbackAdapter() {}

		public void Continue(IJsDialogCallback callback, bool success, string userInput = null) {
			if (userInput == null) {
				callback.Continue(success);
			}
			else {
				callback.Continue(success, userInput);
			}
		}

		public void Dispose(IJsDialogCallback callback) {
			callback.Dispose();
		}
	}
}
