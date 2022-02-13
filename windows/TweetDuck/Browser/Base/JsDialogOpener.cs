using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Utils;
using TweetLib.Browser.CEF.Dialogs;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetDuck.Browser.Base {
	sealed class JsDialogOpener : IJsDialogOpener {
		private static FormMessage CreateMessageForm(MessageDialogType type, string title, string text) {
			return new FormMessage(title, text, type switch {
				MessageDialogType.Error       => MessageBoxIcon.Error,
				MessageDialogType.Warning     => MessageBoxIcon.Warning,
				MessageDialogType.Information => MessageBoxIcon.Information,
				MessageDialogType.Question    => MessageBoxIcon.Question,
				_                             => MessageBoxIcon.None
			});
		}

		private readonly Control control;

		public JsDialogOpener(Control control) {
			this.control = control;
		}

		public void Alert(MessageDialogType type, string title, string message, Action<bool> callback) {
			control.InvokeSafe(() => {
				using FormMessage form = CreateMessageForm(type, title, message);
				form.AddButton(FormMessage.OK, ControlType.Accept | ControlType.Focused);

				bool success = form.ShowDialog() == DialogResult.OK;
				callback(success);
			});
		}

		public void Confirm(MessageDialogType type, string title, string message, Action<bool> callback) {
			control.InvokeSafe(() => {
				using FormMessage form = CreateMessageForm(type, title, message);
				form.AddButton(FormMessage.No, DialogResult.No, ControlType.Cancel);
				form.AddButton(FormMessage.Yes, ControlType.Focused);

				bool success = form.ShowDialog() == DialogResult.OK;
				callback(success);
			});
		}

		public void Prompt(MessageDialogType type, string title, string message, Action<bool, string> callback) {
			control.InvokeSafe(() => {
				using FormMessage form = CreateMessageForm(type, title, message);
				form.AddButton(FormMessage.Cancel, DialogResult.Cancel, ControlType.Cancel);
				form.AddButton(FormMessage.OK, ControlType.Accept | ControlType.Focused);

				float dpiScale = form.GetDPIScale();
				int inputPad = form.HasIcon ? 43 : 0;

				using var input = new TextBox {
					Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
					Font = SystemFonts.MessageBoxFont,
					Location = new Point(BrowserUtils.Scale(22 + inputPad, dpiScale), form.ActionPanelY - BrowserUtils.Scale(46, dpiScale)),
					Size = new Size(form.ClientSize.Width - BrowserUtils.Scale(44 + inputPad, dpiScale), BrowserUtils.Scale(23, dpiScale))
				};

				form.Controls.Add(input);
				form.ActiveControl = input;
				form.Height += input.Size.Height + input.Margin.Vertical;

				bool success = form.ShowDialog() == DialogResult.OK;
				callback(success, input.Text);
			});
		}
	}
}
