using System;
using System.IO;
using System.Net;
using TweetDuck.Core.Utils;

namespace TweetDuck.Updates{
    sealed class UpdateInfo{
        public string VersionTag { get; }
        public string InstallerPath { get; }

        public UpdateDownloadStatus DownloadStatus { get; private set; }
        public Exception DownloadError { get; private set; }

        private readonly string installerFolder;
        private readonly string downloadUrl;
        private WebClient currentDownload;

        public UpdateInfo(UpdaterSettings settings, string versionTag, string downloadUrl){
            this.installerFolder = settings.InstallerDownloadFolder;
            this.downloadUrl = downloadUrl;

            this.VersionTag = versionTag;
            this.InstallerPath = Path.Combine(installerFolder, "TweetDuck."+versionTag+".exe");
        }

        public void BeginSilentDownload(){
            if (DownloadStatus == UpdateDownloadStatus.None || DownloadStatus == UpdateDownloadStatus.Failed){
                DownloadStatus = UpdateDownloadStatus.InProgress;

                try{
                    Directory.CreateDirectory(installerFolder);
                }catch(Exception e){
                    DownloadError = e;
                    DownloadStatus = UpdateDownloadStatus.Failed;
                    return;
                }

                if (string.IsNullOrEmpty(downloadUrl)){
                    DownloadError = new UriFormatException("Could not determine URL of the update installer");
                    DownloadStatus = UpdateDownloadStatus.Failed;
                    return;
                }

                currentDownload = BrowserUtils.DownloadFileAsync(downloadUrl, InstallerPath, () => {
                    DownloadStatus = UpdateDownloadStatus.Done;
                    currentDownload = null;
                }, e => {
                    DownloadError = e;
                    DownloadStatus = UpdateDownloadStatus.Failed;
                    currentDownload = null;
                });
            }
        }

        public void DeleteInstaller(){
            DownloadStatus = UpdateDownloadStatus.None;

            if (currentDownload != null && currentDownload.IsBusy){
                currentDownload.CancelAsync(); // deletes file when cancelled
                return;
            }

            try{
                File.Delete(InstallerPath);
            }catch{
                // rip
            }
        }
    }
}
