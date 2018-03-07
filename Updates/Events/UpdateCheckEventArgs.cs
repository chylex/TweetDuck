﻿using System;

namespace TweetDuck.Updates{
    sealed class UpdateCheckEventArgs : EventArgs{
        public int EventId { get; }
        public bool IsUpdateAvailable { get; }

        public UpdateCheckEventArgs(int eventId, bool isUpdateAvailable){
            this.EventId = eventId;
            this.IsUpdateAvailable = isUpdateAvailable;
        }
    }
}
