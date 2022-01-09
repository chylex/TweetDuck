using System;
using Gtk;
using TweetDuck.Utils;
using TweetImpl.CefGlue.Utils;
using TweetLib.Core.Application;
using TweetLib.Utils.Dialogs;

namespace TweetDuck.Application {
	sealed class FileDialogs : IAppFileDialogs {
		public void SaveFile(SaveFileDialogSettings settings, Action<string> onAccepted) {
			Gtk.Application.Invoke(delegate {
				using FileChooserDialog dialog = new FileChooserDialog(settings.DialogTitle, WindowManager.MainWindow, FileChooserAction.Save);
				dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
				dialog.AddButton(Stock.Save, ResponseType.Accept);
				dialog.DoOverwriteConfirmation = settings.OverwritePrompt;
				dialog.CurrentName = settings.FileName;

				if (settings.Filters is {} filters) {
					foreach (var filter in filters) {
						dialog.AddFilter(GtkUtils.CreateFileFilter(filter));
					}
				}

				if (dialog.Run() == (int) ResponseType.Accept) {
					onAccepted.Invoke(dialog.Filename);
				}
			});
		}
	}
}
