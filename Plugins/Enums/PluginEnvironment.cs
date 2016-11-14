using System;
using System.Collections.Generic;

namespace TweetDck.Plugins.Enums{
    [Flags]
    enum PluginEnvironment{
        None = 0,
        Browser = 1,
        Notification = 2
    }

    static class PluginEnvironmentExtensions{
        public static IEnumerable<PluginEnvironment> Values{
            get{
                yield return PluginEnvironment.Browser;
                yield return PluginEnvironment.Notification;
            }
        }

        public static string GetScriptFile(this PluginEnvironment environment){
            switch(environment){
                case PluginEnvironment.Browser: return "browser.js";
                case PluginEnvironment.Notification: return "notification.js";
                default: return null;
            }
        }

        public static string GetScriptVariables(this PluginEnvironment environment){
            switch(environment){
                case PluginEnvironment.Browser: return "$,$TD,$TDP,TD";
                case PluginEnvironment.Notification: return "$TD,$TDP";
                default: return string.Empty;
            }
        }
    }
}
