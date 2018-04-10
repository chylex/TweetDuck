using System;
using System.IO;
using System.Net;
using TweetDuck.Core.Utils;

namespace TweetDuck.Updates{
    sealed class UpdateInfo{
        public string VersionTag { get; }
        public string ReleaseNotes { get; }
        public string InstallerPath { get; }

        public bool IsUpdateNew => VersionTag != Program.VersionTag;
        public bool IsUpdateDismissed => VersionTag == settings.DismissedUpdate;

        public UpdateDownloadStatus DownloadStatus { get; private set; }
        public Exception DownloadError { get; private set; }

        private readonly UpdaterSettings settings;
        private readonly string downloadUrl;
        private WebClient currentDownload;

        public UpdateInfo(UpdaterSettings settings, string versionTag, string releaseNotes, string downloadUrl){
            this.settings = settings;
            this.downloadUrl = downloadUrl;
            
            this.VersionTag = versionTag;
            this.ReleaseNotes = releaseNotes;
            this.InstallerPath = Path.Combine(settings.InstallerDownloadFolder, "TweetDuck."+versionTag+".exe");
        }

        public void BeginSilentDownload(){
            if (DownloadStatus == UpdateDownloadStatus.None || DownloadStatus == UpdateDownloadStatus.Failed){
                DownloadStatus = UpdateDownloadStatus.InProgress;

                if (string.IsNullOrEmpty(downloadUrl)){
                    DownloadError = new InvalidDataException("Missing installer asset.");
                    DownloadStatus = UpdateDownloadStatus.AssetMissing;
                    return;
                }

                try{
                    Directory.CreateDirectory(settings.InstallerDownloadFolder);
                }catch(Exception e){
                    DownloadError = e;
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

        public override bool Equals(object obj){
            return obj is UpdateInfo info && VersionTag == info.VersionTag;
        }

        public override int GetHashCode(){
            return VersionTag.GetHashCode();
        }
    }
}
