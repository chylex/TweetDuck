using System;
using System.Collections.Generic;

namespace TweetDck.Plugins{
    [Serializable]
    class PluginConfig{
        [field:NonSerialized]
        public event EventHandler<PluginChangedStateEventArgs> PluginChangedState;

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
                if (PluginChangedState != null){
                    PluginChangedState(this,new PluginChangedStateEventArgs(plugin,enabled));
                }
            }
        }

        public bool IsEnabled(Plugin plugin){
            return !Disabled.Contains(plugin.Identifier);
        }
    }
}
