using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using TweetDuck.Browser;
using TweetDuck.Controls;
using TweetLib.Core;
using TweetLib.Core.Application;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Configuration;
using TweetLib.Utils.Data;

namespace TweetDuck.Configuration {
	[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
	sealed class UserConfig : BaseConfig<UserConfig>, IAppUserConfiguration {
		public bool FirstRun { get; set; } = true;

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public bool AllowDataCollection { get; set; } = false;

		public WindowState BrowserWindow { get; set; } = new WindowState();
		public Size PluginsWindowSize    { get; set; } = Size.Empty;

		public bool ExpandLinksOnHover        { get; set; } = true;
		public bool FocusDmInput              { get; set; } = true;
		public bool OpenSearchInFirstColumn   { get; set; } = true;
		public bool KeepLikeFollowDialogsOpen { get; set; } = true;
		public bool BestImageQuality          { get; set; } = true;
		public bool EnableAnimatedImages      { get; set; } = true;
		public bool HideTweetsByNftUsers      { get; set; } = false;

		private bool _enableSmoothScrolling  = true;
		private string? _customCefArgs       = null;

		public string? BrowserPath           { get; set; } = null;
		public string? BrowserPathArgs       { get; set; } = null;
		public bool IgnoreTrackingUrlWarning { get; set; } = false;
		public string? SearchEngineUrl       { get; set; } = null;
		private int _zoomLevel                             = 100;

		public string? VideoPlayerPath     { get; set; } = null;
		public string? VideoPlayerPathArgs { get; set; } = null;
		public int VideoPlayerVolume       { get; set; } = 50;

		public bool EnableSpellCheck { get; set; } = false;
		private string _spellCheckLanguage         = "en-US";

		public string TranslationTarget { get; set; } = "en";
		public int CalendarFirstDay     { get; set; } = -1;

		private TrayIcon.Behavior _trayBehavior       = TrayIcon.Behavior.Disabled;
		public bool EnableTrayHighlight { get; set; } = true;

		public bool EnableUpdateCheck  { get; set; } = true;
		public string? DismissedUpdate { get; set; } = null;

		public bool DisplayNotificationColumn    { get; set; } = false;
		public bool NotificationMediaPreviews    { get; set; } = true;
		public bool NotificationSkipOnLinkClick  { get; set; } = false;
		public bool NotificationNonIntrusiveMode { get; set; } = true;
		public int NotificationIdlePauseSeconds  { get; set; } = 0;

		public bool DisplayNotificationTimer   { get; set; } = true;
		public bool NotificationTimerCountDown { get; set; } = false;
		public int NotificationDurationValue   { get; set; } = 25;

		public DesktopNotification.Position NotificationPosition { get; set; } = DesktopNotification.Position.TopRight;
		public Point CustomNotificationPosition                  { get; set; } = ControlExtensions.InvisibleLocation;
		public int NotificationDisplay                           { get; set; } = 0;
		public int NotificationEdgeDistance                      { get; set; } = 8;
		public int NotificationWindowOpacity                     { get; set; } = 100;

		public DesktopNotification.Size NotificationSize { get; set; } = DesktopNotification.Size.Auto;
		public Size CustomNotificationSize               { get; set; } = Size.Empty;
		public int NotificationScrollSpeed               { get; set; } = 100;

		private string? _notificationSoundPath;
		private int _notificationSoundVolume = 100;

		private bool _muteNotifications;

		public string? CustomBrowserCSS      { get; set; } = null;
		public string? CustomNotificationCSS { get; set; } = null;

		public bool DevToolsInContextMenu { get; set; } = false;
		public bool DevToolsWindowOnTop   { get; set; } = true;

		// SPECIAL PROPERTIES

		public bool IsCustomNotificationPositionSet => CustomNotificationPosition != ControlExtensions.InvisibleLocation;
		public bool IsCustomNotificationSizeSet => CustomNotificationSize != Size.Empty;
		public bool IsCustomSoundNotificationSet => NotificationSoundPath != string.Empty;

		public ImageQuality TwitterImageQuality => BestImageQuality ? ImageQuality.Best : ImageQuality.Default;

		public string NotificationSoundPath {
			get => _notificationSoundPath ?? string.Empty;
			set => UpdatePropertyWithEvent(ref _notificationSoundPath, value, SoundNotificationChanged);
		}

		public int NotificationSoundVolume {
			get => _notificationSoundVolume;
			set => UpdatePropertyWithEvent(ref _notificationSoundVolume, value, SoundNotificationChanged);
		}

		public bool MuteNotifications {
			get => _muteNotifications;
			set => UpdatePropertyWithEvent(ref _muteNotifications, value, MuteToggled);
		}

		public int ZoomLevel {
			get => _zoomLevel;
			set => UpdatePropertyWithEvent(ref _zoomLevel, value, ZoomLevelChanged);
		}

		public TrayIcon.Behavior TrayBehavior {
			get => _trayBehavior;
			set => UpdatePropertyWithEvent(ref _trayBehavior, value, TrayBehaviorChanged);
		}

		public bool EnableSmoothScrolling {
			get => _enableSmoothScrolling;
			set => UpdatePropertyWithCallback(ref _enableSmoothScrolling, value, App.ConfigManager.TriggerProgramRestartRequested);
		}

		public string? CustomCefArgs {
			get => _customCefArgs;
			set => UpdatePropertyWithCallback(ref _customCefArgs, value, App.ConfigManager.TriggerProgramRestartRequested);
		}

		public string SpellCheckLanguage {
			get => _spellCheckLanguage;
			set => UpdatePropertyWithCallback(ref _spellCheckLanguage, value, App.ConfigManager.TriggerProgramRestartRequested);
		}

		// DEPRECATED

		[Obsolete("Moved to SystemConfig")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public bool EnableTouchAdjustment { get; set; }

		[Obsolete("Moved to SystemConfig")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public bool EnableColorProfileDetection { get; set; }

		[Obsolete("Moved to SystemConfig")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public bool UseSystemProxyForAllConnections { get; set; }

		// EVENTS

		public event EventHandler? MuteToggled;
		public event EventHandler? ZoomLevelChanged;
		public event EventHandler? TrayBehaviorChanged;
		public event EventHandler? SoundNotificationChanged;
		public event EventHandler? OptionsDialogClosed;

		public void TriggerOptionsDialogClosed() {
			OptionsDialogClosed?.Invoke(this, EventArgs.Empty);
		}

		// END OF CONFIG

		public override UserConfig ConstructWithDefaults() {
			return new UserConfig();
		}
	}
}
