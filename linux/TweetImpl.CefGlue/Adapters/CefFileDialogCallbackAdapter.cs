using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefFileDialogCallbackAdapter : IFileDialogCallbackAdapter<CefFileDialogCallback> {
		public static CefFileDialogCallbackAdapter Instance { get; } = new ();

		private CefFileDialogCallbackAdapter() {}

		public void Continue(CefFileDialogCallback callback, int selectedAcceptFilter, string[] filePaths) {
			callback.Continue(selectedAcceptFilter, filePaths);
		}

		public void Cancel(CefFileDialogCallback callback) {
			callback.Cancel();
		}

		public void Dispose(CefFileDialogCallback callback) {
			callback.Dispose();
		}
	}
}
