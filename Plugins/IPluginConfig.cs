using System;
using System.Collections.Generic;
using TweetDuck.Plugins.Events;

namespace TweetDuck.Plugins{
    interface IPluginConfig{
        IEnumerable<string> DisabledPlugins { get; }

        event EventHandler<PluginChangedStateEventArgs> PluginChangedState;
        
        void SetEnabled(Plugin plugin, bool enabled);
        bool IsEnabled(Plugin plugin);
    }
}
