using System;
using Gtk;
using TweetImpl.CefGlue.Utils;
using TweetLib.Browser.CEF.Dialogs;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefGlue.Dialogs {
	sealed class JsDialogOpener : IJsDialogOpener {
		private readonly Window window;

		public JsDialogOpener(Window window) {
			this.window = window;
		}

		private MessageDialog CreateMessageDialog(MessageDialogType type, string caption, string text, ButtonsType buttons) {
			var messageType = type switch {
				MessageDialogType.Error       => MessageType.Error,
				MessageDialogType.Warning     => MessageType.Warning,
				MessageDialogType.Information => MessageType.Info,
				MessageDialogType.Question    => MessageType.Question,
				_                             => MessageType.Other
			};

			return GtkUtils.CreateMessageDialog(window, messageType, caption, text, buttons);
		}

		private void Show(MessageDialogType type, string caption, string text, ButtonsType buttons, Action<ResponseType> callback) {
			Application.Invoke(delegate {
				using var dialog = CreateMessageDialog(type, caption, text, buttons);

				ResponseType result = (ResponseType) dialog.Run();
				dialog.Hide();
				callback.Invoke(result);
			});
		}

		public void Alert(MessageDialogType type, string title, string message, Action<bool> callback) {
			Show(type, title, message, ButtonsType.Ok, response => callback(response == ResponseType.Ok));
		}

		public void Confirm(MessageDialogType type, string title, string message, Action<bool> callback) {
			Show(type, title, message, ButtonsType.YesNo, response => callback(response == ResponseType.Yes));
		}

		public void Prompt(MessageDialogType type, string title, string message, Action<bool, string> callback) {
			Application.Invoke(delegate {
				using var dialog = CreateMessageDialog(type, title, message, ButtonsType.OkCancel);

				Entry textField = new Entry();
				textField.Margin = 0;
				textField.MarginStart = 11;
				textField.MarginEnd = 11;
				textField.MarginTop = 0;
				textField.MarginBottom = 0;
				textField.ActivatesDefault = true;
				dialog.ContentArea.Add(textField);
				dialog.GetWidgetForResponse((int) ResponseType.Ok).GrabDefault();
				textField.Show();

				ResponseType result = (ResponseType) dialog.Run();
				string inputText = textField.Text ?? string.Empty;

				dialog.Hide();
				callback.Invoke(result == ResponseType.Ok, inputText);
			});
		}
	}
}
