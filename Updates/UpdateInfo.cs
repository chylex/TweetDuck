using TweetDuck.Core.Utils;

namespace TweetDuck.Updates{
    class UpdateInfo{
        public readonly string VersionTag;
        public readonly string DownloadUrl;

        public string FileName => BrowserUtils.GetFileNameFromUrl(DownloadUrl) ?? Program.BrandName+".Update.exe";

        public UpdateInfo(string versionTag, string downloadUrl){
            this.VersionTag = versionTag;
            this.DownloadUrl = downloadUrl;
        }
    }
}
