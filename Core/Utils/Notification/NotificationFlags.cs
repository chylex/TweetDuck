using System;

namespace TweetDck.Core.Utils.Notification{
    [Flags]
    public enum NotificationFlags{
        None = 0,
        AutoHide = 1,
        DisableScripts = 2,
        DisableContextMenu = 4,
        TopMost = 8
    }
}
