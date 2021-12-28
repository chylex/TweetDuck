using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CefSharp;
using TweetLib.Utils.Static;

namespace TweetDuck.Browser.Handling {
	sealed class FileDialogHandler : IDialogHandler {
		public bool OnFileDialog(IWebBrowser browserControl, IBrowser browser, CefFileDialogMode mode, CefFileDialogFlags flags, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback) {
			if (mode == CefFileDialogMode.Open || mode == CefFileDialogMode.OpenMultiple) {
				string allFilters = string.Join(";", acceptFilters.SelectMany(ParseFileType).Where(filter => !string.IsNullOrEmpty(filter)).Select(filter => "*" + filter));

				using OpenFileDialog dialog = new OpenFileDialog {
					AutoUpgradeEnabled = true,
					DereferenceLinks = true,
					Multiselect = mode == CefFileDialogMode.OpenMultiple,
					Title = "Open Files",
					Filter = $"All Supported Formats ({allFilters})|{allFilters}|All Files (*.*)|*.*"
				};

				if (dialog.ShowDialog() == DialogResult.OK) {
					string ext = Path.GetExtension(dialog.FileName)?.ToLower();
					callback.Continue(acceptFilters.FindIndex(filter => ParseFileType(filter).Contains(ext)), dialog.FileNames.ToList());
				}
				else {
					callback.Cancel();
				}

				callback.Dispose();
				return true;
			}
			else {
				callback.Dispose();
				return false;
			}
		}

		private static IEnumerable<string> ParseFileType(string type) {
			if (string.IsNullOrEmpty(type)) {
				return StringUtils.EmptyArray;
			}

			if (type[0] == '.') {
				return new string[] { type };
			}

			switch (type) {
				case "image/jpeg": return new string[] { ".jpg", ".jpeg" };
				case "image/png": return new string[] { ".png" };
				case "image/gif": return new string[] { ".gif" };
				case "image/webp": return new string[] { ".webp" };
				case "video/mp4": return new string[] { ".mp4" };
				case "video/quicktime": return new string[] { ".mov", ".qt" };
			}

			Debugger.Break();
			return StringUtils.EmptyArray;
		}
	}
}
