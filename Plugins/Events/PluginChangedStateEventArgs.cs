namespace TweetDuck.Plugins.Events{
    sealed class PluginChangedStateEventArgs : PluginEventArgs{
        public bool IsEnabled { get; }

        public PluginChangedStateEventArgs(Plugin plugin, bool isEnabled) : base(plugin){
            this.IsEnabled = isEnabled;
        }
    }
}
