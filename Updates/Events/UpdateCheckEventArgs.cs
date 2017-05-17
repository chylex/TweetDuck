using System;

namespace TweetDuck.Updates.Events{
    class UpdateCheckEventArgs : EventArgs{
        public int EventId { get; }
        public bool UpdateAvailable { get; }
        public string LatestVersion { get; }

        public UpdateCheckEventArgs(int eventId, bool updateAvailable, string latestVersion){
            EventId = eventId;
            UpdateAvailable = updateAvailable;
            LatestVersion = latestVersion;
        }
    }
}
