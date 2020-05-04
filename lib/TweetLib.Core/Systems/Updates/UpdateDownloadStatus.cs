namespace TweetLib.Core.Systems.Updates{
    public enum UpdateDownloadStatus{
        None = 0,
        InProgress,
        Canceled,
        AssetMissing,
        Failed,
        Done
    }

    public static class UpdateDownloadStatusExtensions{
        public static bool IsFinished(this UpdateDownloadStatus status, bool canRetry){
            return status == UpdateDownloadStatus.AssetMissing || status == UpdateDownloadStatus.Done || (status == UpdateDownloadStatus.Failed && !canRetry);
        }
    }
}
