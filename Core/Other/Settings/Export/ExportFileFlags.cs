using System;

namespace TweetDck.Core.Other.Settings.Export{
    [Flags]
    enum ExportFileFlags{
        None = 0,
        Config = 1,
        Session = 2,
        PluginData = 4,
        All = Config|Session|PluginData
    }
}
