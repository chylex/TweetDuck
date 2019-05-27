namespace TweetLib.Core.Features.Plugins.Enums{
    public enum PluginGroup{
        Official, Custom
    }

    public static class PluginGroupExtensions{
        public static string GetIdentifierPrefix(this PluginGroup group){
            return group switch{
                PluginGroup.Official => "official/",
                PluginGroup.Custom   => "custom/",
                _                    => "unknown/"
            };
        }

        public static string GetIdentifierPrefixShort(this PluginGroup group){
            return group switch{
                PluginGroup.Official => "o/",
                PluginGroup.Custom   => "c/",
                _ =>                    "?/"
            };
        }
    }
}
