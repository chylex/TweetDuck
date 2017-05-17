using System;

namespace TweetDuck.Updates.Events{
    class UpdateAcceptedEventArgs : EventArgs{
        public readonly UpdateInfo UpdateInfo;

        public UpdateAcceptedEventArgs(UpdateInfo info){
            this.UpdateInfo = info;
        }
    }
}
