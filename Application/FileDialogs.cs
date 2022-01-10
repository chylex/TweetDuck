using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TweetDuck.Management;
using TweetLib.Core.Application;
using TweetLib.Core.Systems.Dialogs;

namespace TweetDuck.Application {
	sealed class FileDialogs : IAppFileDialogs {
		public void SaveFile(SaveFileDialogSettings settings, Action<string> onAccepted) {
			static string FormatFilter(FileDialogFilter filter) {
				var builder = new StringBuilder();
				builder.Append(filter.Name);

				var extensions = string.Join(";", filter.Extensions.Select(ext => "*" + ext));
				if (extensions.Length > 0) {
					builder.Append(" (");
					builder.Append(extensions);
					builder.Append(")");
				}

				builder.Append('|');
				builder.Append(extensions.Length == 0 ? "*.*" : extensions);
				return builder.ToString();
			}

			FormManager.RunOnUIThreadAsync(() => {
				using SaveFileDialog dialog = new SaveFileDialog {
					AutoUpgradeEnabled = true,
					OverwritePrompt = settings.OverwritePrompt,
					Title = settings.DialogTitle,
					FileName = settings.FileName,
					Filter = settings.Filters == null ? null : string.Join("|", settings.Filters.Select(FormatFilter))
				};

				if (dialog.ShowDialog() == DialogResult.OK) {
					onAccepted(dialog.FileName);
				}
			});
		}
	}
}
