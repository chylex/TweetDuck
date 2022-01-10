using System;
using System.Collections.Generic;
using TweetLib.Core.Features.Plugins.Events;
using TweetLib.Core.Systems.Configuration;

namespace TweetLib.Core.Features.Plugins.Config {
	public sealed class PluginConfig : IConfigObject<PluginConfig> {
		internal IEnumerable<string> DisabledPlugins => disabled;

		public event EventHandler<PluginChangedStateEventArgs>? PluginChangedState;

		private readonly HashSet<string> defaultDisabled;
		private readonly HashSet<string> disabled;

		public PluginConfig(IEnumerable<string> defaultDisabled) {
			this.defaultDisabled = new HashSet<string>(defaultDisabled);
			this.disabled = new HashSet<string>(this.defaultDisabled);
		}

		public PluginConfig ConstructWithDefaults() {
			return new PluginConfig(defaultDisabled);
		}

		internal void Reset(IEnumerable<string> newDisabledPlugins) {
			disabled.Clear();
			disabled.UnionWith(newDisabledPlugins);
		}

		internal void ResetToDefault() {
			Reset(defaultDisabled);
		}

		public void SetEnabled(Plugin plugin, bool enabled) {
			if ((enabled && disabled.Remove(plugin.Identifier)) || (!enabled && disabled.Add(plugin.Identifier))) {
				PluginChangedState?.Invoke(this, new PluginChangedStateEventArgs(plugin, enabled));
			}
		}

		public bool IsEnabled(Plugin plugin) {
			return !disabled.Contains(plugin.Identifier);
		}
	}
}
