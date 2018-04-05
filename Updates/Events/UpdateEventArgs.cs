using System;

namespace TweetDuck.Updates.Events{
    sealed class UpdateEventArgs : EventArgs{
        public UpdateInfo UpdateInfo { get; }
        
        public UpdateEventArgs(UpdateInfo updateInfo){
            this.UpdateInfo = updateInfo;
        }
    }
}
