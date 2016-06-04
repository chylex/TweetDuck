using System;
using System.Collections.Generic;

namespace TweetDck.Plugins{
    class PluginLoadErrorEventArgs : EventArgs{
        public IEnumerable<string> Errors;

        public PluginLoadErrorEventArgs(IEnumerable<string> errors){
            this.Errors = errors;
        }
    }
}
