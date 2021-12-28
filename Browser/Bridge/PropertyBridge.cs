using System.Text;
using TweetDuck.Configuration;
using TweetLib.Core;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Browser.Bridge {
	static class PropertyBridge {
		public enum Environment {
			Browser,
			Notification
		}

		public static string GenerateScript(Environment environment) {
			static string Bool(bool value) => value ? "true;" : "false;";
			static string Str(string value) => $"\"{value}\";";

			UserConfig config = Program.Config.User;
			StringBuilder build = new StringBuilder(384).Append("(function(x){");

			build.Append("x.expandLinksOnHover=").Append(Bool(config.ExpandLinksOnHover));

			if (environment == Environment.Browser) {
				build.Append("x.focusDmInput=").Append(Bool(config.FocusDmInput));
				build.Append("x.openSearchInFirstColumn=").Append(Bool(config.OpenSearchInFirstColumn));
				build.Append("x.keepLikeFollowDialogsOpen=").Append(Bool(config.KeepLikeFollowDialogsOpen));
				build.Append("x.muteNotifications=").Append(Bool(config.MuteNotifications));
				build.Append("x.notificationMediaPreviews=").Append(Bool(config.NotificationMediaPreviews));
				build.Append("x.translationTarget=").Append(Str(config.TranslationTarget));
				build.Append("x.firstDayOfWeek=").Append(config.CalendarFirstDay == -1 ? JQuery.GetDatePickerDayOfWeek(Lib.Culture.DateTimeFormat.FirstDayOfWeek) : config.CalendarFirstDay);
			}

			if (environment == Environment.Notification) {
				build.Append("x.skipOnLinkClick=").Append(Bool(config.NotificationSkipOnLinkClick));
			}

			return build.Append("})(window.$TDX=window.$TDX||{});if(window.TDGF_onPropertiesUpdated)window.TDGF_onPropertiesUpdated()").ToString();
		}
	}
}
