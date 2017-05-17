namespace TweetDuck.Plugins.Enums{
    enum PluginGroup{
        Official, Custom
    }

    static class PluginGroupExtensions{
        public static string GetIdentifierPrefix(this PluginGroup group){
            switch(group){
                case PluginGroup.Official: return "official/";
                case PluginGroup.Custom: return "custom/";
                default: return "unknown/";
            }
        }
    }
}
