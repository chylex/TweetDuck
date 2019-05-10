using System;
using System.Collections.Generic;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetDuck.Configuration{
    sealed class PluginConfig : ConfigManager.BaseConfig, IPluginConfig{
        private static readonly string[] DefaultDisabled = {
            "official/clear-columns",
            "official/reply-account"
        };

        // CONFIGURATION

        public IEnumerable<string> DisabledPlugins => disabled;
        
        public event EventHandler<PluginChangedStateEventArgs> PluginChangedState;

        public void SetEnabled(Plugin plugin, bool enabled){
            if ((enabled && disabled.Remove(plugin.Identifier)) || (!enabled && disabled.Add(plugin.Identifier))){
                PluginChangedState?.Invoke(this, new PluginChangedStateEventArgs(plugin, enabled));
                Save();
            }
        }

        public bool IsEnabled(Plugin plugin){
            return !disabled.Contains(plugin.Identifier);
        }

        public void ReloadSilently(IEnumerable<string> newDisabled){
            disabled.Clear();
            disabled.UnionWith(newDisabled);
        }

        private readonly HashSet<string> disabled = new HashSet<string>(DefaultDisabled);
        
        // END OF CONFIG

        public PluginConfig(ConfigManager configManager) : base(configManager){}

        protected override ConfigManager.BaseConfig ConstructWithDefaults(ConfigManager configManager){
            return new PluginConfig(configManager);
        }
    }
}
