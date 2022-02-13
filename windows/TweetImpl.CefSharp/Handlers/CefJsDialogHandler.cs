using System.Diagnostics.CodeAnalysis;
using CefSharp;
using TweetImpl.CefSharp.Adapters;
using TweetLib.Browser.CEF.Dialogs;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.CEF.Logic;

namespace TweetImpl.CefSharp.Handlers {
	sealed class CefJsDialogHandler : IJsDialogHandler {
		private readonly JsDialogHandlerLogic<IJsDialogCallback> logic;

		public CefJsDialogHandler(IJsDialogOpener jsDialogOpener) {
			this.logic = new JsDialogHandlerLogic<IJsDialogCallback>(jsDialogOpener, CefJsDialogCallbackAdapter.Instance);
		}

		[SuppressMessage("ReSharper", "RedundantAssignment")]
		public bool OnJSDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage) {
			return logic.OnJSDialog(ConvertDialogType(dialogType), messageText, callback, out suppressMessage);
		}

		public bool OnBeforeUnloadDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string messageText, bool isReload, IJsDialogCallback callback) {
			return logic.OnBeforeUnloadDialog(callback);
		}

		public void OnResetDialogState(IWebBrowser chromiumWebBrowser, IBrowser browser) {}
		public void OnDialogClosed(IWebBrowser chromiumWebBrowser, IBrowser browser) {}

		private static JsDialogType ConvertDialogType(CefJsDialogType type) {
			return type switch {
				CefJsDialogType.Alert   => JsDialogType.Alert,
				CefJsDialogType.Confirm => JsDialogType.Confirm,
				CefJsDialogType.Prompt  => JsDialogType.Prompt,
				_                       => JsDialogType.Unknown
			};
		}
	}
}
