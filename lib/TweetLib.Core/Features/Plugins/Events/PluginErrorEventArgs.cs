using System;
using System.Collections.Generic;

namespace TweetLib.Core.Features.Plugins.Events {
	public sealed class PluginErrorEventArgs : EventArgs {
		public bool HasErrors => Errors.Count > 0;

		public IList<string> Errors { get; }

		internal PluginErrorEventArgs(IList<string> errors) {
			this.Errors = errors;
		}
	}
}
