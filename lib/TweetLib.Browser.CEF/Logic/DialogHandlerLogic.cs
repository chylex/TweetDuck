using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TweetLib.Browser.CEF.Dialogs;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Utils.Dialogs;
using TweetLib.Utils.Static;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class DialogHandlerLogic<TCallback> {
		private readonly IFileDialogOpener fileDialogOpener;
		private readonly IFileDialogCallbackAdapter<TCallback> callbackAdapter;

		public DialogHandlerLogic(IFileDialogOpener fileDialogOpener, IFileDialogCallbackAdapter<TCallback> callbackAdapter) {
			this.fileDialogOpener = fileDialogOpener;
			this.callbackAdapter = callbackAdapter;
		}

		public bool OnFileDialog(FileDialogType type, IEnumerable<string> acceptFilters, TCallback callback) {
			if (type is FileDialogType.Open or FileDialogType.OpenMultiple) {
				var multiple = type == FileDialogType.OpenMultiple;
				var supportedExtensions = acceptFilters.SelectMany(ParseFileType).Where(static filter => !string.IsNullOrEmpty(filter)).ToArray();
				
				var filters = new List<FileDialogFilter> {
					new ("All Supported Formats", supportedExtensions),
					new ("All Files", ".*")
				};

				fileDialogOpener.OpenFile("Open Files", multiple, filters, files => {
					string ext = Path.GetExtension(files[0]).ToLower();
					callbackAdapter.Continue(callback, Array.FindIndex(supportedExtensions, filter => ParseFileType(filter).Contains(ext)), files);
					callbackAdapter.Dispose(callback);
				}, () => {
					callbackAdapter.Cancel(callback);
					callbackAdapter.Dispose(callback);
				});

				return true;
			}
			else {
				callbackAdapter.Dispose(callback);
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

			string[] extensions = type switch {
				"image/jpeg"      => new string[] { ".jpg", ".jpeg" },
				"image/png"       => new string[] { ".png" },
				"image/gif"       => new string[] { ".gif" },
				"image/webp"      => new string[] { ".webp" },
				"video/mp4"       => new string[] { ".mp4" },
				"video/quicktime" => new string[] { ".mov", ".qt" },
				_                 => StringUtils.EmptyArray
			};

			if (extensions.Length == 0) {
				Debug.WriteLine("Unknown file type: " + type);
				Debugger.Break();
			}

			return extensions;
		}
	}
}
