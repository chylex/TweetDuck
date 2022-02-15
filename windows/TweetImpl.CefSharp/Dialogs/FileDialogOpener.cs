using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Utils.Dialogs;

namespace TweetImpl.CefSharp.Dialogs {
	sealed class FileDialogOpener : IFileDialogOpener {
		public static FileDialogOpener Instance { get; } = new FileDialogOpener();

		private FileDialogOpener() {}

		public void OpenFile(string title, bool multiple, List<FileDialogFilter> filters, Action<string[]> onAccepted, Action onCancelled) {
			using OpenFileDialog dialog = new OpenFileDialog {
				AutoUpgradeEnabled = true,
				DereferenceLinks = true,
				Multiselect = multiple,
				Title = title,
				Filter = string.Join("|", filters.Select(filter => filter.JoinFullNameAndPattern("|")))
			};

			if (dialog.ShowDialog() == DialogResult.OK) {
				onAccepted(dialog.FileNames);
			}
			else {
				onCancelled();
			}
		}
	}
}
