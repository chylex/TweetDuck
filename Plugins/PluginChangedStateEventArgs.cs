using System;

namespace TweetDck.Plugins{
    class PluginChangedStateEventArgs : EventArgs{
        public Plugin Plugin { get; private set; }
        public bool IsEnabled { get; private set; }

        public PluginChangedStateEventArgs(Plugin plugin, bool isEnabled){
            this.Plugin = plugin;
            this.IsEnabled = isEnabled;
        }
    }
}
