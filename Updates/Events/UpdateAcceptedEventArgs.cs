using System;

namespace TweetDck.Updates.Events{
    class UpdateAcceptedEventArgs : EventArgs{
        public readonly UpdateInfo UpdateInfo;

        public UpdateAcceptedEventArgs(UpdateInfo info){
            this.UpdateInfo = info;
        }
    }
}
