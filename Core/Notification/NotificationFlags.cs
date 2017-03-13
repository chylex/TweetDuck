using System;

namespace TweetDck.Core.Notification{
    [Flags]
    public enum NotificationFlags{
        None = 0,
        DisableContextMenu = 1,
        TopMost = 2
    }
}
