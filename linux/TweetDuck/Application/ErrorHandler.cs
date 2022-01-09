using System;
using System.Diagnostics;
using System.Threading;
using Gtk;
using TweetDuck.Utils;
using TweetImpl.CefGlue.Utils;
using TweetLib.Core;
using TweetLib.Core.Application;

namespace TweetDuck.Application {
	sealed class ErrorHandler : IAppErrorHandler {
		private readonly int originalThread = Thread.CurrentThread.ManagedThreadId;

		public void HandleException(string caption, string message, bool canIgnore, Exception e) {
			App.Logger.Error(e.ToString());

			Window? window = WindowManager.MainWindow;
			bool isWindowOwned = false;

			if (window == null) {
				window = WindowManager.CreateWindow(Lib.BrandName);
				isWindowOwned = true;
			}

			if (Thread.CurrentThread.ManagedThreadId == originalThread) {
				DoShow();
			}
			else {
				Gtk.Application.Invoke((_, _) => DoShow());
			}

			void DoShow() {
				using var dialog = GtkUtils.CreateMessageDialog(window, MessageType.Error, caption, message, ButtonsType.None);
				dialog.AddButton("Exit", ResponseType.Close);

				if (canIgnore) {
					dialog.AddButton("Ignore", ResponseType.Ok);
				}

				ResponseType response = (ResponseType) dialog.Run();
				dialog.Hide();

				if (isWindowOwned) {
					window.Dispose();
				}

				if (response == ResponseType.Close) {
					try {
						Process.GetCurrentProcess().Kill();
					} catch {
						Environment.FailFast(message, e);
					}
				}
			}
		}
	}
}
