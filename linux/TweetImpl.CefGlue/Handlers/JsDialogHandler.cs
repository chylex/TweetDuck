using Gtk;
using TweetImpl.CefGlue.Adapters;
using TweetImpl.CefGlue.Dialogs;
using TweetLib.Browser.CEF.Dialogs;
using TweetLib.Browser.CEF.Logic;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers {
	sealed class JsDialogHandler : CefJSDialogHandler {
		private readonly JsDialogHandlerLogic<CefJSDialogCallback> logic;

		public JsDialogHandler(Window window) {
			this.logic = new JsDialogHandlerLogic<CefJSDialogCallback>(new JsDialogOpener(window), CefJsDialogCallbackAdapter.Instance);
		}

		protected override bool OnJSDialog(CefBrowser browser, string originUrl, CefJSDialogType dialogType, string messageText, string defaultPromptText, CefJSDialogCallback callback, out bool suppressMessage) {
			return logic.OnJSDialog(ConvertDialogType(dialogType), messageText, callback, out suppressMessage);
		}

		protected override bool OnBeforeUnloadDialog(CefBrowser browser, string messageText, bool isReload, CefJSDialogCallback callback) {
			return logic.OnBeforeUnloadDialog(callback);
		}

		protected override void OnResetDialogState(CefBrowser browser) {}
		protected override void OnDialogClosed(CefBrowser browser) {}

		private static JsDialogType ConvertDialogType(CefJSDialogType type) {
			return type switch {
				CefJSDialogType.Alert   => JsDialogType.Alert,
				CefJSDialogType.Confirm => JsDialogType.Confirm,
				CefJSDialogType.Prompt  => JsDialogType.Prompt,
				_                       => JsDialogType.Unknown
			};
		}
	}
}
