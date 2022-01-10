using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Dialogs;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features {
	[SuppressMessage("ReSharper", "MemberCanBeInternal")]
	public sealed class FileDownloadManager {
		public bool SupportsViewingImage => App.SystemHandler.OpenAssociatedProgram != null;
		public bool SupportsCopyingImage => App.SystemHandler.CopyImageFromFile != null;
		public bool SupportsFileSaving => App.FileDialogs != null;

		private readonly IFileDownloader fileDownloader;

		internal FileDownloadManager(IFileDownloader fileDownloader) {
			this.fileDownloader = fileDownloader;
		}

		private void DownloadTempImage(string url, Action<string> process) {
			string? staticFileName = TwitterUrls.GetImageFileName(url);
			string file = Path.Combine(fileDownloader.CacheFolder, staticFileName ?? Path.GetRandomFileName());

			if (staticFileName != null && FileUtils.FileExistsAndNotEmpty(file)) {
				process(file);
			}
			else {
				void OnSuccess() {
					process(file);
				}

				static void OnFailure(Exception ex) {
					App.MessageDialogs.Error("Image Download", "An error occurred while downloading the image: " + ex.Message);
				}

				fileDownloader.DownloadFile(url, file, OnSuccess, OnFailure);
			}
		}

		public void ViewImage(string url) {
			if (App.SystemHandler.OpenAssociatedProgram == null) {
				return;
			}

			DownloadTempImage(url, static path => {
				string ext = Path.GetExtension(path);

				if (ImageUrl.ValidExtensions.Contains(ext)) {
					App.SystemHandler.OpenAssociatedProgram(path);
				}
				else {
					App.MessageDialogs.Error("Image Download", "Unknown image file extension: " + ext);
				}
			});
		}

		public void CopyImage(string url) {
			if (App.SystemHandler.CopyImageFromFile is {} copyImageFromFile) {
				DownloadTempImage(url, new Action<string>(copyImageFromFile));
			}
		}

		public void SaveImages(string[] urls, string? author) {
			var fileDialogs = App.FileDialogs;
			if (fileDialogs == null) {
				App.MessageDialogs.Error("Image Download", "Saving files is not supported!");
				return;
			}

			if (urls.Length == 0) {
				return;
			}

			bool oneImage = urls.Length == 1;
			string firstImageLink = urls[0];
			int qualityIndex = firstImageLink.IndexOf(':', firstImageLink.LastIndexOf('/'));

			string? filename = TwitterUrls.GetImageFileName(firstImageLink);
			string? ext = Path.GetExtension(filename); // includes dot

			var settings = new SaveFileDialogSettings {
				DialogTitle = oneImage ? "Save Image" : "Save Images",
				OverwritePrompt = oneImage,
				FileName = qualityIndex == -1 ? filename : $"{author} {Path.ChangeExtension(filename, null)} {firstImageLink.Substring(qualityIndex + 1)}".Trim() + ext,
				Filters = new [] { new FileDialogFilter(oneImage ? "Image" : "Images", string.IsNullOrEmpty(ext) ? Array.Empty<string>() : new [] { ext }) }
			};

			fileDialogs.SaveFile(settings, path => {
				static void OnFailure(Exception ex) {
					App.MessageDialogs.Error("Image Download", "An error occurred while downloading the image: " + ex.Message);
				}

				if (oneImage) {
					fileDownloader.DownloadFile(firstImageLink, path, null, OnFailure);
				}
				else {
					string pathBase = Path.ChangeExtension(path, null);
					string pathExt = Path.GetExtension(path);

					for (int index = 0; index < urls.Length; index++) {
						fileDownloader.DownloadFile(urls[index], $"{pathBase} {index + 1}{pathExt}", null, OnFailure);
					}
				}
			});
		}

		public void SaveVideo(string url, string? author) {
			var fileDialogs = App.FileDialogs;
			if (fileDialogs == null) {
				App.MessageDialogs.Error("Video Download", "Saving files is not supported!");
				return;
			}

			string? filename = TwitterUrls.GetFileNameFromUrl(url);
			string? ext = Path.GetExtension(filename);

			var settings = new SaveFileDialogSettings {
				DialogTitle = "Save Video",
				OverwritePrompt = true,
				FileName = string.IsNullOrEmpty(author) ? filename : $"{author} {filename}".TrimStart(),
				Filters = new [] { new FileDialogFilter("Video", string.IsNullOrEmpty(ext) ? Array.Empty<string>() : new [] { ext }) }
			};

			fileDialogs.SaveFile(settings, path => {
				static void OnError(Exception ex) {
					App.MessageDialogs.Error("Video Download", "An error occurred while downloading the video: " + ex.Message);
				}

				fileDownloader.DownloadFile(url, path, null, OnError);
			});
		}
	}
}
