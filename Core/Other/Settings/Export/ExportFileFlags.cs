using System;

namespace TweetDuck.Core.Other.Settings.Export{
    [Flags]
    enum ExportFileFlags{
        None = 0,
        UserConfig = 1,
        SystemConfig = 2, // TODO implement later
        Session = 4,
        PluginData = 8,
        All = UserConfig|SystemConfig|Session|PluginData
    }
}
