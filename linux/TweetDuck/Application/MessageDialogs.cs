using Gtk;
using TweetDuck.Utils;
using TweetImpl.CefGlue.Utils;
using TweetLib.Core.Application;

namespace TweetDuck.Application {
	sealed class MessageDialogs : IAppMessageDialogs {
		public static void Show(MessageType type, string caption, string text) {
			Gtk.Application.Invoke(delegate {
				using var dialog = GtkUtils.CreateMessageDialog(WindowManager.MainWindow!, type, caption, text, ButtonsType.Ok);
				dialog.Run();
				dialog.Hide();
			});
		}

		public void Information(string caption, string text) {
			Show(MessageType.Info, caption, text);
		}

		public void Error(string caption, string text) {
			Show(MessageType.Error, caption, text);
		}
	}
}
