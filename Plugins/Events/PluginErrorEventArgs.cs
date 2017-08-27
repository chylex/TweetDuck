using System;
using System.Collections.Generic;

namespace TweetDuck.Plugins.Events{
    class PluginErrorEventArgs : EventArgs{
        public bool HasErrors => Errors.Count > 0;

        public IList<string> Errors { get; }

        public PluginErrorEventArgs(IList<string> errors){
            this.Errors = errors;
        }
    }
}
