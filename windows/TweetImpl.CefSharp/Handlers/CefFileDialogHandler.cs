using System.Collections.Generic;
using CefSharp;
using TweetImpl.CefSharp.Adapters;
using TweetImpl.CefSharp.Dialogs;
using TweetLib.Browser.CEF.Dialogs;
using TweetLib.Browser.CEF.Logic;
using static TweetLib.Browser.CEF.Dialogs.FileDialogType;

namespace TweetImpl.CefSharp.Handlers {
	sealed class CefFileDialogHandler : IDialogHandler {
		private readonly DialogHandlerLogic<IFileDialogCallback> logic;

		public CefFileDialogHandler() {
			this.logic = new DialogHandlerLogic<IFileDialogCallback>(FileDialogOpener.Instance, CefFileDialogCallbackAdapter.Instance);
		}

		public bool OnFileDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, IFileDialogCallback callback) {
			return logic.OnFileDialog(ConvertDialogType(mode), acceptFilters, callback);
		}

		private static FileDialogType ConvertDialogType(CefFileDialogMode mode) {
			return mode switch {
				CefFileDialogMode.Open         => Open,
				CefFileDialogMode.OpenMultiple => OpenMultiple,
				_                              => Other
			};
		}
	}
}
