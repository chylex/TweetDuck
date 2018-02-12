using System;

namespace TweetDuck.Plugins.Events{
    class PluginEventArgs : EventArgs{
        public Plugin Plugin { get; }

        public PluginEventArgs(Plugin plugin){
            this.Plugin = plugin;
        }
    }
}
