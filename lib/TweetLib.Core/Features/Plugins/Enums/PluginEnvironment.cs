using System;
using System.Collections.Generic;

namespace TweetLib.Core.Features.Plugins.Enums{
    [Flags]
    public enum PluginEnvironment{
        None = 0,
        Browser = 1,
        Notification = 2
    }

    public static class PluginEnvironmentExtensions{
        public static IEnumerable<PluginEnvironment> Values { get; } = new PluginEnvironment[]{
            PluginEnvironment.Browser,
            PluginEnvironment.Notification
        };

        public static bool IncludesDisabledPlugins(this PluginEnvironment environment){
            return environment == PluginEnvironment.Browser;
        }

        public static string? GetPluginScriptFile(this PluginEnvironment environment){
            return environment switch{
                PluginEnvironment.Browser      => "browser.js",
                PluginEnvironment.Notification => "notification.js",
                _                              => throw new InvalidOperationException($"Invalid plugin environment: {environment}")
            };
        }

        public static string GetPluginScriptVariables(this PluginEnvironment environment){
            return environment switch{
                PluginEnvironment.Browser      => "$,$TD,$TDP,TD",
                PluginEnvironment.Notification => "$TD,$TDP",
                _                              => string.Empty
            };
        }
    }
}
