using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Dialogs {
	sealed class FileDialogOpener : IFileDialogOpener {
		public static FileDialogOpener Instance { get; } = new FileDialogOpener();

		private FileDialogOpener() {}

		public void OpenFile(string title, bool multiple, List<string> supportedExtensions, Action<string[]> onAccepted, Action onCancelled) {
			string supportedFormatsFilter = string.Join(";", supportedExtensions.Select(filter => "*" + filter));
			
			using OpenFileDialog dialog = new OpenFileDialog {
				AutoUpgradeEnabled = true,
				DereferenceLinks = true,
				Multiselect = multiple,
				Title = title,
				Filter = $"All Supported Formats ({supportedFormatsFilter})|{supportedFormatsFilter}|All Files (*.*)|*.*"
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
