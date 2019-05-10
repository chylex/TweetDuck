namespace TweetLib.Core.Features.Plugins.Enums{
    public enum PluginGroup{
        Official, Custom
    }

    public static class PluginGroupExtensions{
        public static string GetIdentifierPrefix(this PluginGroup group){
            switch(group){
                case PluginGroup.Official: return "official/";
                case PluginGroup.Custom: return "custom/";
                default: return "unknown/";
            }
        }

        public static string GetIdentifierPrefixShort(this PluginGroup group){
            switch(group){
                case PluginGroup.Official: return "o/";
                case PluginGroup.Custom: return "c/";
                default: return "?/";
            }
        }
    }
}
