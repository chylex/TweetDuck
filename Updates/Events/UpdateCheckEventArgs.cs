using System;

namespace TweetDck.Updates.Events{
    class UpdateCheckEventArgs : EventArgs{
        public int EventId { get; private set; }
        public bool UpdateAvailable { get; private set; }
        public string LatestVersion { get; private set; }

        public UpdateCheckEventArgs(int eventId, bool updateAvailable, string latestVersion){
            EventId = eventId;
            UpdateAvailable = updateAvailable;
            LatestVersion = latestVersion;
        }
    }
}
