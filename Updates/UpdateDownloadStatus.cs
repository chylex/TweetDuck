namespace TweetDuck.Updates{
    public enum UpdateDownloadStatus{
        None = 0,
        InProgress,
        AssetMissing,
        Done,
        Failed
    }

    public static class UpdateDownloadStatusExtensions{
        public static bool IsFinished(this UpdateDownloadStatus status){
            return status == UpdateDownloadStatus.AssetMissing || status == UpdateDownloadStatus.Done || status == UpdateDownloadStatus.Failed;
        }
    }
}
