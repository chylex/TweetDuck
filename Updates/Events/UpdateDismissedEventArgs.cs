using System;

namespace TweetDuck.Updates.Events{
    class UpdateDismissedEventArgs : EventArgs{
        public readonly string VersionTag;

        public UpdateDismissedEventArgs(string versionTag){
            this.VersionTag = versionTag;
        }
    }
}
