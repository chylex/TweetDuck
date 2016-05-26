using System;

namespace TweetDck.Updates{
    class UpdateAcceptedEventArgs : EventArgs{
        public readonly UpdateInfo UpdateInfo;

        public UpdateAcceptedEventArgs(UpdateInfo info){
            this.UpdateInfo = info;
        }
    }
}
