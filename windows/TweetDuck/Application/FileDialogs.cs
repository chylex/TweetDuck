using System;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Management;
using TweetLib.Core.Application;
using TweetLib.Utils.Dialogs;

namespace TweetDuck.Application {
	sealed class FileDialogs : IAppFileDialogs {
		public void SaveFile(SaveFileDialogSettings settings, Action<string> onAccepted) {
			FormManager.RunOnUIThreadAsync(() => {
				using SaveFileDialog dialog = new SaveFileDialog {
					AutoUpgradeEnabled = true,
					OverwritePrompt = settings.OverwritePrompt,
					Title = settings.DialogTitle,
					FileName = settings.FileName,
					Filter = settings.Filters == null ? null : string.Join("|", settings.Filters.Select(filter => filter.JoinFullNameAndPattern("|")))
				};

				if (dialog.ShowDialog() == DialogResult.OK) {
					onAccepted(dialog.FileName);
				}
			});
		}
	}
}
