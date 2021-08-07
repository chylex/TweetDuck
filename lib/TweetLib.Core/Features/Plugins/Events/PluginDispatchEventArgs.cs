using System;
using TweetLib.Core.Browser;

namespace TweetLib.Core.Features.Plugins.Events {
	public sealed class PluginDispatchEventArgs : EventArgs {
		public IScriptExecutor Executor { get; }

		public PluginDispatchEventArgs(IScriptExecutor executor) {
			this.Executor = executor;
		}
	}
}
