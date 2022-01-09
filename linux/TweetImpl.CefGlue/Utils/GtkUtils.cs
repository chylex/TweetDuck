using Gtk;
using TweetLib.Utils.Dialogs;

namespace TweetImpl.CefGlue.Utils {
	public static class GtkUtils {
		public static MessageDialog CreateMessageDialog(Window window, MessageType type, string title, string message, ButtonsType buttons) {
			static string PrefixTitle(MessageType type, string title) {
				return type switch {
					MessageType.Error    => "Error | " + title,
					MessageType.Warning  => "Warning | " + title,
					_                    => title
				};
			}

			var dialog = new MessageDialog(window, DialogFlags.Modal | DialogFlags.DestroyWithParent, type, buttons, message) {
				Title = PrefixTitle(type, title),
				WidthRequest = 350
			};

			dialog.ContentArea.MarginTop = 8;
			dialog.ContentArea.Spacing = 4;
			return dialog;
		}

		public static FileFilter CreateFileFilter(FileDialogFilter filter) {
			var fileFilter = new FileFilter {
				Name = filter.FullName
			};

			foreach (var extension in filter.Extensions) {
				fileFilter.AddPattern("*" + extension);
			}

			return fileFilter;
		}
	}
}
