using System.Linq;
using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Adapters {
	sealed class CefFileDialogCallbackAdapter : IFileDialogCallbackAdapter<IFileDialogCallback> {
		public static CefFileDialogCallbackAdapter Instance { get; } = new CefFileDialogCallbackAdapter();

		private CefFileDialogCallbackAdapter() {}

		public void Continue(IFileDialogCallback callback, int selectedAcceptFilter, string[] filePaths) {
			callback.Continue(filePaths.ToList());
		}

		public void Cancel(IFileDialogCallback callback) {
			callback.Cancel();
		}

		public void Dispose(IFileDialogCallback callback) {
			callback.Dispose();
		}
	}
}
