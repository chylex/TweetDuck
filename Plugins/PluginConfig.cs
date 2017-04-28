using System;
using System.Collections.Generic;
using TweetDck.Plugins.Events;

namespace TweetDck.Plugins{
    [Serializable]
    sealed class PluginConfig{
        [field:NonSerialized]
        public event EventHandler<PluginChangedStateEventArgs> InternalPluginChangedState; // should only be accessed from PluginManager

        public IEnumerable<string> DisabledPlugins => Disabled;
        public bool AnyDisabled => Disabled.Count > 0;

        private readonly HashSet<string> Disabled = new HashSet<string>();

        public void SetEnabled(Plugin plugin, bool enabled){
            if ((enabled && Disabled.Remove(plugin.Identifier)) || (!enabled && Disabled.Add(plugin.Identifier))){
                InternalPluginChangedState?.Invoke(this, new PluginChangedStateEventArgs(plugin, enabled));
            }
        }

        public bool IsEnabled(Plugin plugin){
            return !Disabled.Contains(plugin.Identifier) && plugin.CanRun;
        }

        public void DisableOfficialFromConfig(string pluginName){
            Disabled.Add("official/"+pluginName);
        }
    }
}
