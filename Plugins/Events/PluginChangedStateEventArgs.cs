using System;

namespace TweetDck.Plugins.Events{
    class PluginChangedStateEventArgs : EventArgs{
        public Plugin Plugin { get; }
        public bool IsEnabled { get; }

        public PluginChangedStateEventArgs(Plugin plugin, bool isEnabled){
            this.Plugin = plugin;
            this.IsEnabled = isEnabled;
        }
    }
}
