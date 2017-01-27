using System;

namespace TweetDck.Updates.Events{
    class UpdateDismissedEventArgs : EventArgs{
        public readonly string VersionTag;

        public UpdateDismissedEventArgs(string versionTag){
            this.VersionTag = versionTag;
        }
    }
}
