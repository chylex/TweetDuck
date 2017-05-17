using System;
using System.Text;

namespace TweetDuck.Core.Bridge{
    static class PropertyBridge{
        [Flags]
        public enum Properties{
            ExpandLinksOnHover = 1,
            MuteNotifications = 2,
            HasCustomNotificationSound = 4,
            SkipOnLinkClick = 8,
            SwitchAccountSelectors = 16,
            AllBrowser = ExpandLinksOnHover | SwitchAccountSelectors | MuteNotifications | HasCustomNotificationSound,
            AllNotification = ExpandLinksOnHover | SkipOnLinkClick
        }

        public static string GenerateScript(Properties properties){
            StringBuilder build = new StringBuilder();
            build.Append("(function(c){");

            if (properties.HasFlag(Properties.ExpandLinksOnHover)){
                build.Append("c.expandLinksOnHover=").Append(Program.UserConfig.ExpandLinksOnHover ? "true;" : "false;");
            }

            if (properties.HasFlag(Properties.SwitchAccountSelectors)){
                build.Append("c.switchAccountSelectors=").Append(Program.UserConfig.SwitchAccountSelectors ? "true;" : "false;");
            }

            if (properties.HasFlag(Properties.MuteNotifications)){
                build.Append("c.muteNotifications=").Append(Program.UserConfig.MuteNotifications ? "true;" : "false;");
            }

            if (properties.HasFlag(Properties.HasCustomNotificationSound)){
                build.Append("c.hasCustomNotificationSound=").Append(Program.UserConfig.NotificationSoundPath.Length > 0 ? "true;" : "false;");
            }

            if (properties.HasFlag(Properties.SkipOnLinkClick)){
                build.Append("c.skipOnLinkClick=").Append(Program.UserConfig.NotificationSkipOnLinkClick ? "true;" : "false;");
            }

            build.Append("})(window.$TDX=window.$TDX||{})");
            return build.ToString();
        }
    }
}
