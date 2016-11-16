using System;

namespace TweetDck.Core.Utils{
    [Flags]
    public enum NotificationFlags{
        None = 0,
        AutoHide = 1,
        DisableScripts = 2,
        DisableContextMenu = 4
    }
}
