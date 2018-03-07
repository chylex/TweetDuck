using System;

namespace TweetDuck.Updates{
    sealed class UpdateEventArgs : EventArgs{
        public UpdateInfo UpdateInfo { get; }
        
        public UpdateEventArgs(UpdateInfo updateInfo){
            this.UpdateInfo = updateInfo;
        }
    }
}
