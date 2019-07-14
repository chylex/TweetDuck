using System;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetLib.Core.Features.Plugins{
    public interface IPluginDispatcher{
        event EventHandler<PluginDispatchEventArgs> Ready;
        void AttachBridge(string name, object bridge);
    }
}
