using System;
using TweetLib.Browser.CEF.Dialogs;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class JsDialogHandlerLogic<TCallback> {
		private static (MessageDialogType, string) GetMessageDialogProperties(string text) {
			MessageDialogType type = MessageDialogType.None;
			
			int pipe = text.IndexOf('|');
			if (pipe != -1) {
				type = text[..pipe] switch {
					"error"    => MessageDialogType.Error,
					"warning"  => MessageDialogType.Warning,
					"info"     => MessageDialogType.Information,
					"question" => MessageDialogType.Question,
					_          => MessageDialogType.None
				};

				if (type != MessageDialogType.None) {
					text = text[(pipe + 1)..];
				}
			}

			return (type, text);
		}
		
		private readonly IJsDialogOpener jsDialogOpener;
		private readonly IJsDialogCallbackAdapter<TCallback> callbackAdapter;

		public JsDialogHandlerLogic(IJsDialogOpener jsDialogOpener, IJsDialogCallbackAdapter<TCallback> callbackAdapter) {
			this.jsDialogOpener = jsDialogOpener;
			this.callbackAdapter = callbackAdapter;
		}

		public bool OnJSDialog(JsDialogType dialogType, string messageText, TCallback callback, out bool suppressMessage) {
			suppressMessage = false;

			var (type, text) = GetMessageDialogProperties(messageText);
			
			if (dialogType == JsDialogType.Alert) {
				jsDialogOpener.Alert(type, "Browser Message", text, success => {
					callbackAdapter.Continue(callback, success);
					callbackAdapter.Dispose(callback);
				});
			}
			else if (dialogType == JsDialogType.Confirm) {
				jsDialogOpener.Confirm(type, "Browser Confirmation", text, success => {
					callbackAdapter.Continue(callback, success);
					callbackAdapter.Dispose(callback);
				});
			}
			else if (dialogType == JsDialogType.Prompt) {
				jsDialogOpener.Prompt(type, "Browser Prompt", text, (success, input) => {
					callbackAdapter.Continue(callback, success, input);
					callbackAdapter.Dispose(callback);
				});
			}
			else {
				return false;
			}
			
			return true;
		}

		public bool OnBeforeUnloadDialog(IDisposable callback) {
			callback.Dispose();
			return false;
		}
	}
}
