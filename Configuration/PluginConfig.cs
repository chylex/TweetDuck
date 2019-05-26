﻿using System;
using System.Collections.Generic;
using TweetLib.Core.Features.Configuration;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetDuck.Configuration{
    sealed class PluginConfig : BaseConfig, IPluginConfig{
        private static readonly string[] DefaultDisabled = {
            "official/clear-columns",
            "official/reply-account"
        };

        // CONFIGURATION DATA

        private readonly HashSet<string> disabled = new HashSet<string>(DefaultDisabled);

        // EVENTS
        
        public event EventHandler<PluginChangedStateEventArgs> PluginChangedState;
        
        // END OF CONFIG

        public PluginConfig(IConfigManager configManager) : base(configManager){}

        protected override BaseConfig ConstructWithDefaults(IConfigManager configManager){
            return new PluginConfig(configManager);
        }

        // INTERFACE IMPLEMENTATION

        IEnumerable<string> IPluginConfig.DisabledPlugins => disabled;

        void IPluginConfig.Reset(IEnumerable<string> newDisabledPlugins){
            disabled.Clear();
            disabled.UnionWith(newDisabledPlugins);
        }

        public void SetEnabled(Plugin plugin, bool enabled){
            if ((enabled && disabled.Remove(plugin.Identifier)) || (!enabled && disabled.Add(plugin.Identifier))){
                PluginChangedState?.Invoke(this, new PluginChangedStateEventArgs(plugin, enabled));
                Save();
            }
        }

        public bool IsEnabled(Plugin plugin){
            return !disabled.Contains(plugin.Identifier);
        }
    }
}
