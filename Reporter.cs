using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Dialogs;
using TweetLib.Core;
using TweetLib.Core.Application;

namespace TweetDuck {
	sealed class Reporter : IAppErrorHandler {
		private readonly string logFile;

		public Reporter(string logFile) {
			this.logFile = logFile;
		}

		public void SetupUnhandledExceptionHandler(string caption) {
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
				if (args.ExceptionObject is Exception ex) {
					HandleException(caption, "An unhandled exception has occurred.", false, ex);
				}
			};
		}

		public bool LogVerbose(string data) {
			return Arguments.HasFlag(Arguments.ArgLogging) && LogImportant(data);
		}

		public bool LogImportant(string data) {
			return ((IAppErrorHandler) this).Log(data);
		}

		bool IAppErrorHandler.Log(string text) {
			#if DEBUG
			Debug.WriteLine(text);
			#endif

			StringBuilder build = new StringBuilder();

			if (!File.Exists(logFile)) {
				build.Append("Please, report all issues to: https://github.com/chylex/TweetDuck/issues\r\n\r\n");
			}

			build.Append("[").Append(DateTime.Now.ToString("G", Lib.Culture)).Append("]\r\n");
			build.Append(text).Append("\r\n\r\n");

			try {
				File.AppendAllText(logFile, build.ToString(), Encoding.UTF8);
				return true;
			} catch {
				return false;
			}
		}

		public void HandleException(string caption, string message, bool canIgnore, Exception e) {
			bool loggedSuccessfully = LogImportant(e.ToString());

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
				using (Process.Start(logFile)) {}
			};

			form.AddActionControl(btnOpenLog);

			if (form.ShowDialog() == DialogResult.Ignore) {
				return;
			}

			try {
				Process.GetCurrentProcess().Kill();
			} catch {
				Environment.FailFast(message, e);
			}
		}

		public static void HandleEarlyFailure(string caption, string message) {
			Program.SetupWinForms();
			FormMessage.Error(caption, message, "Exit");

			try {
				Process.GetCurrentProcess().Kill();
			} catch {
				Environment.FailFast(message, new Exception(message));
			}
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
