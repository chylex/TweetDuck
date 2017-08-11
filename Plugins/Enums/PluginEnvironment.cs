using System;
using System.Collections.Generic;

namespace TweetDuck.Plugins.Enums{
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

        public static bool IncludesDisabledPlugins(this PluginEnvironment environment){
            return environment == PluginEnvironment.Browser;
        }

        public static string GetScriptIdentifier(this PluginEnvironment environment){
            switch(environment){
                case PluginEnvironment.None: return "root:plugins";
                case PluginEnvironment.Browser: return "root:plugins.browser";
                case PluginEnvironment.Notification: return "root:plugins.notification";
                default: return null;
            }
        }

        public static string GetPluginScriptFile(this PluginEnvironment environment){
            switch(environment){
                case PluginEnvironment.Browser: return "browser.js";
                case PluginEnvironment.Notification: return "notification.js";
                default: return null;
            }
        }

        public static string GetPluginScriptVariables(this PluginEnvironment environment){
            switch(environment){
                case PluginEnvironment.Browser: return "$,$TD,$TDP,TD";
                case PluginEnvironment.Notification: return "$TD,$TDP";
                default: return string.Empty;
            }
        }
    }
}
