using System;

namespace TweetDuck.Updates.Events{
    class UpdateCheckEventArgs : EventArgs{
        public int EventId { get; }
        public UpdateInfo UpdateInfo { get; }

        public bool UpdateAvailable => UpdateInfo != null;

        public UpdateCheckEventArgs(int eventId, UpdateInfo updateInfo){
            EventId = eventId;
            UpdateInfo = updateInfo;
        }
    }
}
