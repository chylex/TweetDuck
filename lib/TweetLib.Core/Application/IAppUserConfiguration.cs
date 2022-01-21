using System;
using TweetLib.Core.Features.Twitter;

namespace TweetLib.Core.Application {
	public interface IAppUserConfiguration {
		string? CustomBrowserCSS { get; }
		string? CustomNotificationCSS { get; }
		string? DismissedUpdate { get; }
		bool ExpandLinksOnHover { get; }
		bool FirstRun { get; }
		bool FocusDmInput { get; }
		bool HideTweetsByNftUsers { get; }
		bool IsCustomSoundNotificationSet { get; }
		bool KeepLikeFollowDialogsOpen { get; }
		bool MuteNotifications { get; }
		bool NotificationMediaPreviews { get; }
		bool NotificationSkipOnLinkClick { get; }
		string NotificationSoundPath { get; }
		int NotificationSoundVolume { get; }
		bool OpenSearchInFirstColumn { get; }
		int CalendarFirstDay { get; }
		string TranslationTarget { get; }
		ImageQuality TwitterImageQuality { get; }

		event EventHandler MuteToggled;
		event EventHandler OptionsDialogClosed;
		event EventHandler SoundNotificationChanged;
	}
}
