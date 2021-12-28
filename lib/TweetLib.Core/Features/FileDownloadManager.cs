using System;
using System.IO;
using System.Linq;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Dialogs;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features {
	public sealed class FileDownloadManager {
		private readonly IBrowserComponent browser;

		internal FileDownloadManager(IBrowserComponent browser) {
			this.browser = browser;
		}

		private void DownloadTempImage(string url, Action<string> process) {
			string? staticFileName = TwitterUrls.GetImageFileName(url);
			string file = Path.Combine(browser.FileDownloader.CacheFolder, staticFileName ?? Path.GetRandomFileName());

			if (staticFileName != null && FileUtils.FileExistsAndNotEmpty(file)) {
				process(file);
			}
			else {
				void OnSuccess() {
					process(file);
				}

				static void OnFailure(Exception ex) {
					App.DialogHandler.Error("Image Download", "An error occurred while downloading the image: " + ex.Message, Dialogs.OK);
				}

				browser.FileDownloader.DownloadFile(url, file, OnSuccess, OnFailure);
			}
		}

		public void ViewImage(string url) {
			DownloadTempImage(url, static path => {
				string ext = Path.GetExtension(path);

				if (ImageUrl.ValidExtensions.Contains(ext)) {
					App.SystemHandler.OpenAssociatedProgram(path);
				}
				else {
					App.DialogHandler.Error("Image Download", "Unknown image file extension: " + ext, Dialogs.OK);
				}
			});
		}

		public void CopyImage(string url) {
			DownloadTempImage(url, App.SystemHandler.CopyImageFromFile);
		}

		public void SaveImages(string[] urls, string? author) {
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

			App.DialogHandler.SaveFile(settings, path => {
				static void OnFailure(Exception ex) {
					App.DialogHandler.Error("Image Download", "An error occurred while downloading the image: " + ex.Message, Dialogs.OK);
				}

				if (oneImage) {
					browser.FileDownloader.DownloadFile(firstImageLink, path, null, OnFailure);
				}
				else {
					string pathBase = Path.ChangeExtension(path, null);
					string pathExt = Path.GetExtension(path);

					for (int index = 0; index < urls.Length; index++) {
						browser.FileDownloader.DownloadFile(urls[index], $"{pathBase} {index + 1}{pathExt}", null, OnFailure);
					}
				}
			});
		}

		public void SaveVideo(string url, string? author) {
			string? filename = TwitterUrls.GetFileNameFromUrl(url);
			string? ext = Path.GetExtension(filename);

			var settings = new SaveFileDialogSettings {
				DialogTitle = "Save Video",
				OverwritePrompt = true,
				FileName = string.IsNullOrEmpty(author) ? filename : $"{author} {filename}".TrimStart(),
				Filters = new [] { new FileDialogFilter("Video", string.IsNullOrEmpty(ext) ? Array.Empty<string>() : new [] { ext }) }
			};

			App.DialogHandler.SaveFile(settings, path => {
				static void OnError(Exception ex) {
					App.DialogHandler.Error("Video Download", "An error occurred while downloading the video: " + ex.Message, Dialogs.OK);
				}

				browser.FileDownloader.DownloadFile(url, path, null, OnError);
			});
		}
	}
}
