using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Dialogs;
using TweetDuck.Management;
using TweetLib.Core;
using TweetLib.Core.Application;

namespace TweetDuck {
	sealed class Reporter : IAppErrorHandler {
		private static void Exit(string message, Exception ex = null) {
			try {
				Process.GetCurrentProcess().Kill();
			} catch {
				Environment.FailFast(message, ex ?? new Exception(message));
			}
		}

		public static void HandleEarlyFailure(string caption, string message) {
			Program.SetupWinForms();
			FormMessage.Error(caption, message, "Exit");
			Exit(message);
		}

		public void HandleException(string caption, string message, bool canIgnore, Exception e) {
			bool loggedSuccessfully = App.Logger.Error(e.ToString());

			FormManager.RunOnUIThread(() => {
				string exceptionText = e is ExpandedLogException ? e.Message + "\n\nDetails with potentially sensitive information are in the Error Log." : e.Message;
				FormMessage form = new FormMessage(caption, message + "\nError: " + exceptionText, canIgnore ? MessageBoxIcon.Warning : MessageBoxIcon.Error);

				Button btnExit = form.AddButton(FormMessage.Exit);
				Button btnIgnore = form.AddButton(FormMessage.Ignore, DialogResult.Ignore, ControlType.Cancel);

				btnIgnore.Enabled = canIgnore;
				form.ActiveControl = canIgnore ? btnIgnore : btnExit;

				Button btnOpenLog = new Button {
					Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
					Enabled = loggedSuccessfully,
					Font = SystemFonts.MessageBoxFont,
					Location = new Point(9, 12),
					Margin = new Padding(0, 0, 48, 0),
					Size = new Size(106, 26),
					Text = "Show Error Log",
					UseVisualStyleBackColor = true
				};

				btnOpenLog.Click += (sender, args) => {
					if (!App.Logger.OpenLogFile()) {
						FormMessage.Error("Error Log", "Cannot open error log.", FormMessage.OK);
					}
				};

				form.AddActionControl(btnOpenLog);

				if (form.ShowDialog() != DialogResult.Ignore) {
					Exit(message, e);
				}
			});
		}

		public sealed class ExpandedLogException : Exception {
			private readonly string details;

			public ExpandedLogException(Exception source, string details) : base(source.Message, source) {
				this.details = details;
			}

			public override string ToString() => base.ToString() + "\r\n" + details;
		}
	}
}
