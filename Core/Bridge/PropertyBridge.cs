using System;
using System.Text;

namespace TweetDck.Core.Bridge{
    static class PropertyBridge{
        [Flags]
        public enum Properties{
            ExpandLinksOnHover = 1,
            MuteNotifications = 2,
            HasCustomNotificationSound = 4, // TODO changes if the file is deleted
            All = ExpandLinksOnHover | MuteNotifications | HasCustomNotificationSound
        }

        public static string GenerateScript(Properties properties = Properties.All){
            StringBuilder build = new StringBuilder();
            build.Append("(function(c){");

            if (properties.HasFlag(Properties.ExpandLinksOnHover)){
                build.Append("c.expandLinksOnHover=").Append(Program.UserConfig.ExpandLinksOnHover ? "true;" : "false;");
            }

            if (properties.HasFlag(Properties.MuteNotifications)){
                build.Append("c.muteNotifications=").Append(Program.UserConfig.MuteNotifications ? "true;" : "false;");
            }

            if (properties.HasFlag(Properties.HasCustomNotificationSound)){
                build.Append("c.hasCustomNotificationSound=").Append(!string.IsNullOrEmpty(Program.UserConfig.NotificationSoundPath) ? "true;" : "false;");
            }

            build.Append("})(window.$TDX=window.$TDX||{})");
            return build.ToString();
        }
    }
}
