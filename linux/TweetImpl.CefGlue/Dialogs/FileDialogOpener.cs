using System;
using System.Collections.Generic;
using Gtk;
using TweetImpl.CefGlue.Utils;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Utils.Dialogs;
using Action = System.Action;

namespace TweetImpl.CefGlue.Dialogs {
	sealed class FileDialogOpener : IFileDialogOpener {
		private readonly Window window;

		public FileDialogOpener(Window window) {
			this.window = window;
		}

		public void OpenFile(string title, bool multiple, List<FileDialogFilter> filters, Action<string[]> onAccepted, Action onCancelled) {
			Application.Invoke(delegate {
				using FileChooserDialog dialog = new FileChooserDialog(title, window, FileChooserAction.Open);
				dialog.SelectMultiple = multiple;
				dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
				dialog.AddButton(Stock.Save, ResponseType.Accept);

				foreach (var filter in filters) {
					dialog.AddFilter(GtkUtils.CreateFileFilter(filter));
				}

				if (dialog.Run() == (int) ResponseType.Accept) {
					onAccepted(dialog.Filenames);
				}
				else {
					onCancelled();
				}
			});
		}
	}
}
