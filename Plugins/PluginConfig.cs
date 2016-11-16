using System;
using System.Collections.Generic;
using TweetDck.Plugins.Events;

namespace TweetDck.Plugins{
    [Serializable]
    sealed class PluginConfig{
        [field:NonSerialized]
        public event EventHandler<PluginChangedStateEventArgs> InternalPluginChangedState; // should only be accessed from PluginManager

        public IEnumerable<string> DisabledPlugins{
            get{
                return Disabled;
            }
        }

        public bool AnyDisabled{
            get{
                return Disabled.Count > 0;
            }
        }

        private readonly HashSet<string> Disabled = new HashSet<string>();

        public void SetEnabled(Plugin plugin, bool enabled){
            if ((enabled && Disabled.Remove(plugin.Identifier)) || (!enabled && Disabled.Add(plugin.Identifier))){
                if (InternalPluginChangedState != null){
                    InternalPluginChangedState(this, new PluginChangedStateEventArgs(plugin, enabled));
                }
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
