using System;
using System.Collections.Generic;

namespace TweetDck.Plugins.Events{
    class PluginLoadEventArgs : EventArgs{
        public bool Success{
            get{
                return Errors.Count == 0;
            }
        }

        public IList<string> Errors;

        public PluginLoadEventArgs(IList<string> errors){
            this.Errors = errors;
        }
    }
}
