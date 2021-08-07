using System;
using System.Collections.Generic;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetLib.Core.Features.Plugins.Config {
	public interface IPluginConfig {
		event EventHandler<PluginChangedStateEventArgs> PluginChangedState;

		IEnumerable<string> DisabledPlugins { get; }
		void Reset(IEnumerable<string> newDisabledPlugins);

		void SetEnabled(Plugin plugin, bool enabled);
		bool IsEnabled(Plugin plugin);
	}
}
