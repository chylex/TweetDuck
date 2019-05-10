using System;
using System.Collections.Generic;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetLib.Core.Features.Plugins{
    public interface IPluginConfig{
        IEnumerable<string> DisabledPlugins { get; }

        event EventHandler<PluginChangedStateEventArgs> PluginChangedState;
        
        void SetEnabled(Plugin plugin, bool enabled);
        bool IsEnabled(Plugin plugin);
    }
}
