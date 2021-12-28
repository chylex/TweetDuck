using System;
using TweetLib.Utils.Data;

namespace TweetLib.Core.Systems.Updates {
	public sealed class UpdateCheckEventArgs : EventArgs {
		public int EventId { get; }
		public Result<UpdateInfo> Result { get; }

		internal UpdateCheckEventArgs(int eventId, Result<UpdateInfo> result) {
			this.EventId = eventId;
			this.Result = result;
		}
	}
}
