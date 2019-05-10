﻿using System;
using TweetLib.Core.Data;

namespace TweetLib.Core.Features.Updates{
    public sealed class UpdateCheckEventArgs : EventArgs{
        public int EventId { get; }
        public Result<UpdateInfo> Result { get; }
        
        public UpdateCheckEventArgs(int eventId, Result<UpdateInfo> result){
            this.EventId = eventId;
            this.Result = result;
        }
    }
}
