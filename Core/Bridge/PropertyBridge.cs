using System.Text;

namespace TweetDuck.Core.Bridge{
    static class PropertyBridge{
        public enum Environment{
            Browser, Notification
        }

        public static string GenerateScript(Environment environment){
            string Bool(bool value) => value ? "true;" : "false;";
            string Str(string value) => '"'+value+"\";";

            StringBuilder build = new StringBuilder().Append("(function(x){");

            build.Append("x.expandLinksOnHover=").Append(Bool(Program.UserConfig.ExpandLinksOnHover));
            
            if (environment == Environment.Browser){
                build.Append("x.openSearchInFirstColumn=").Append(Bool(Program.UserConfig.OpenSearchInFirstColumn));
                build.Append("x.keepLikeFollowDialogsOpen=").Append(Bool(Program.UserConfig.KeepLikeFollowDialogsOpen));
                build.Append("x.muteNotifications=").Append(Bool(Program.UserConfig.MuteNotifications));
                build.Append("x.notificationMediaPreviews=").Append(Bool(Program.UserConfig.NotificationMediaPreviews));
                build.Append("x.translationTarget=").Append(Str(Program.UserConfig.TranslationTarget));
            }

            if (environment == Environment.Notification){
                build.Append("x.skipOnLinkClick=").Append(Bool(Program.UserConfig.NotificationSkipOnLinkClick));
            }
            
            return build.Append("})(window.$TDX=window.$TDX||{})").ToString();
        }
    }
}
