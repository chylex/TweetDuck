using System;

namespace TweetDuck.Updates{
    sealed class UpdateEventArgs : EventArgs{
        public int EventId { get; }
        public UpdateInfo UpdateInfo { get; }

        public bool IsUpdateAvailable => UpdateInfo != null;

        public UpdateEventArgs(int eventId, UpdateInfo updateInfo){
            this.EventId = eventId;
            this.UpdateInfo = updateInfo;
        }

        public UpdateEventArgs(UpdateInfo updateInfo){
            this.EventId = updateInfo.EventId;
            this.UpdateInfo = updateInfo;
        }
    }
}
