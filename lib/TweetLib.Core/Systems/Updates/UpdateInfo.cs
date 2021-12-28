using System;
using System.IO;
using System.Net;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Systems.Updates {
	public sealed class UpdateInfo {
		public string VersionTag { get; }
		public string ReleaseNotes { get; }
		public string InstallerPath { get; }

		public UpdateDownloadStatus DownloadStatus { get; private set; }
		public Exception? DownloadError { get; private set; }

		private readonly string downloadUrl;
		private readonly string installerFolder;
		private WebClient? currentDownload;

		public UpdateInfo(string versionTag, string releaseNotes, string downloadUrl, string installerFolder) {
			this.downloadUrl = downloadUrl;
			this.installerFolder = installerFolder;

			this.VersionTag = versionTag;
			this.ReleaseNotes = releaseNotes;
			this.InstallerPath = Path.Combine(installerFolder, $"{Lib.BrandName}.{versionTag}.exe");
		}

		public void BeginSilentDownload() {
			if (FileUtils.FileExistsAndNotEmpty(InstallerPath)) {
				DownloadStatus = UpdateDownloadStatus.Done;
				return;
			}

			if (DownloadStatus == UpdateDownloadStatus.None || DownloadStatus == UpdateDownloadStatus.Failed) {
				DownloadStatus = UpdateDownloadStatus.InProgress;

				if (string.IsNullOrEmpty(downloadUrl)) {
					DownloadError = new InvalidDataException("Missing installer asset.");
					DownloadStatus = UpdateDownloadStatus.AssetMissing;
					return;
				}

				try {
					Directory.CreateDirectory(installerFolder);
				} catch (Exception e) {
					DownloadError = e;
					DownloadStatus = UpdateDownloadStatus.Failed;
					return;
				}

				WebClient client = WebUtils.NewClient($"{Lib.BrandName} {TweetDuck.Version.Tag}");

				client.DownloadFileCompleted += WebUtils.FileDownloadCallback(InstallerPath, () => {
					DownloadStatus = UpdateDownloadStatus.Done;
					currentDownload = null;
				}, e => {
					DownloadError = e;
					DownloadStatus = UpdateDownloadStatus.Failed;
					currentDownload = null;
				});

				client.DownloadFileAsync(new Uri(downloadUrl), InstallerPath);
			}
		}

		public void DeleteInstaller() {
			DownloadStatus = UpdateDownloadStatus.None;

			if (currentDownload != null && currentDownload.IsBusy) {
				currentDownload.CancelAsync(); // deletes file when cancelled
				return;
			}

			try {
				File.Delete(InstallerPath);
			} catch {
				// rip
			}
		}

		public void CancelDownload() {
			DeleteInstaller();
			DownloadStatus = UpdateDownloadStatus.Canceled;
		}

		public override bool Equals(object obj) {
			return obj is UpdateInfo info && VersionTag == info.VersionTag;
		}

		public override int GetHashCode() {
			return VersionTag.GetHashCode();
		}
	}
}
