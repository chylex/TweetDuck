using System.Text;

namespace TweetDuck.Core.Bridge{
    static class PropertyBridge{
        public enum Environment{
            Browser, Notification
        }

        public static string GenerateScript(Environment environment){
            string Bool(bool value){
                return value ? "true," : "false,";
            }

            StringBuilder build = new StringBuilder().Append("window.$TDX={");

            build.Append("expandLinksOnHover:").Append(Bool(Program.UserConfig.ExpandLinksOnHover));
            
            if (environment == Environment.Browser){
                build.Append("switchAccountSelectors:").Append(Bool(Program.UserConfig.SwitchAccountSelectors));
                build.Append("muteNotifications:").Append(Bool(Program.UserConfig.MuteNotifications));
                build.Append("hasCustomNotificationSound:").Append(Bool(Program.UserConfig.NotificationSoundPath.Length > 0));
                build.Append("notificationMediaPreviews:").Append(Bool(Program.UserConfig.NotificationMediaPreviews));
            }

            if (environment == Environment.Notification){
                build.Append("skipOnLinkClick:").Append(Bool(Program.UserConfig.NotificationSkipOnLinkClick));
            }
            
            return build.Append("}").ToString();
        }
    }
}
