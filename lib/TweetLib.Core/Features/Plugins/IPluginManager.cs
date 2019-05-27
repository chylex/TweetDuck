using System;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetLib.Core.Features.Plugins{
    public interface IPluginManager{
        IPluginConfig Config { get; }

        event EventHandler<PluginErrorEventArgs> Reloaded;

        int GetTokenFromPlugin(Plugin plugin);
        Plugin GetPluginFromToken(int token);
    }
}
