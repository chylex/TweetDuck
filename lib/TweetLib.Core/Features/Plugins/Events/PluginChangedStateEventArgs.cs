using System;

namespace TweetLib.Core.Features.Plugins.Events{
    public sealed class PluginChangedStateEventArgs : EventArgs{
        public Plugin Plugin { get; }
        public bool IsEnabled { get; }

        public PluginChangedStateEventArgs(Plugin plugin, bool isEnabled){
            this.Plugin = plugin;
            this.IsEnabled = isEnabled;
        }
    }
}
