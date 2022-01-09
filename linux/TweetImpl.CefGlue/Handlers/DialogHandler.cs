using Gtk;
using TweetImpl.CefGlue.Adapters;
using TweetImpl.CefGlue.Dialogs;
using TweetLib.Browser.CEF.Dialogs;
using TweetLib.Browser.CEF.Logic;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers {
	sealed class DialogHandler : CefDialogHandler {
		private readonly DialogHandlerLogic<CefFileDialogCallback> logic;

		public DialogHandler(Window window) {
			this.logic = new DialogHandlerLogic<CefFileDialogCallback>(new FileDialogOpener(window), CefFileDialogCallbackAdapter.Instance);
		}

		protected override bool OnFileDialog(CefBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, string[] acceptFilters, int selectedAcceptFilter, CefFileDialogCallback callback) {
			return logic.OnFileDialog(ConvertDialogType(mode & CefFileDialogMode.TypeMask), acceptFilters, callback);
		}

		private static FileDialogType ConvertDialogType(CefFileDialogMode mode) {
			return mode switch {
				CefFileDialogMode.Open         => FileDialogType.Open,
				CefFileDialogMode.OpenMultiple => FileDialogType.OpenMultiple,
				_                              => FileDialogType.Other
			};
		}
	}
}
