using System;
using System.Diagnostics.CodeAnalysis;
using TweetLib.Core.Application;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Configuration {
	[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
	sealed class UserConfiguration : IConfigObject<UserConfiguration>, IAppUserConfiguration {
		public string CustomBrowserCSS           => string.Empty;
		public string CustomNotificationCSS      => string.Empty;
		public string? DismissedUpdate           => null;
		public bool ExpandLinksOnHover           => true;
		public bool FirstRun                     => false;
		public bool IsCustomSoundNotificationSet => false;
		public bool MuteNotifications            => false;
		public bool NotificationMediaPreviews    => false;
		public bool NotificationSkipOnLinkClick  => false;
		public string NotificationSoundPath      => string.Empty;
		public int NotificationSoundVolume       => 0;

		public bool FocusDmInput                { get; set; } = false;
		public bool HideTweetsByNftUsers        { get; set; } = false;
		public bool KeepLikeFollowDialogsOpen   { get; set; } = false;
		public bool OpenSearchInFirstColumn     { get; set; } = false;
		public int CalendarFirstDay             { get; set; } = JQuery.GetDatePickerDayOfWeek(DayOfWeek.Monday);
		public string TranslationTarget         { get; set; } = "en";
		public ImageQuality TwitterImageQuality { get; set; } = ImageQuality.Best;

		public event EventHandler? MuteToggled;
		public event EventHandler? OptionsDialogClosed;
		public event EventHandler? SoundNotificationChanged;

		public UserConfiguration ConstructWithDefaults() {
			return new UserConfiguration();
		}
	}
}
