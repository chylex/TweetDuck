using System;
using System.Drawing;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other;
using TweetDuck.Data;
using TweetLib.Core.Features.Configuration;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Configuration{
    sealed class UserConfig : BaseConfig{
        
        // CONFIGURATION DATA

        public bool FirstRun            { get; set; } = true;
        public bool AllowDataCollection { get; set; } = false;

        public WindowState BrowserWindow { get; set; } = new WindowState();
        public Size PluginsWindowSize    { get; set; } = Size.Empty;

        public bool ExpandLinksOnHover        { get; set; } = true;
        public bool FocusDmInput              { get; set; } = true;
        public bool OpenSearchInFirstColumn   { get; set; } = true;
        public bool KeepLikeFollowDialogsOpen { get; set; } = true;
        public bool BestImageQuality          { get; set; } = true;
        public bool EnableAnimatedImages      { get; set; } = true;

        private bool _enableSmoothScrolling = true;
        private bool _enableTouchAdjustment = false;
        private string _customCefArgs       = null;

        public string BrowserPath            { get; set; } = null;
        public bool IgnoreTrackingUrlWarning { get; set; } = false;
        public string SearchEngineUrl        { get; set; } = null;
        private int _zoomLevel                             = 100;

        public int VideoPlayerVolume { get; set; } = 50;
        
        public bool EnableSpellCheck { get; set; } = false;
        private string _spellCheckLanguage         = "en-US";

        public string TranslationTarget { get; set; } = "en";
        
        private TrayIcon.Behavior _trayBehavior       = TrayIcon.Behavior.Disabled;
        public bool EnableTrayHighlight { get; set; } = true;

        public bool EnableUpdateCheck { get; set; } = true;
        public string DismissedUpdate { get; set; } = null;

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

        public DesktopNotification.Size NotificationSize { get; set; } = DesktopNotification.Size.Auto;
        public Size CustomNotificationSize               { get; set; } = Size.Empty;
        public int NotificationScrollSpeed               { get; set; } = 100;
        
        private string _notificationSoundPath;
        private int _notificationSoundVolume = 100;

        private bool _muteNotifications;

        public string CustomBrowserCSS      { get; set; } = null;
        public string CustomNotificationCSS { get; set; } = null;
        
        // SPECIAL PROPERTIES

        public bool IsCustomNotificationPositionSet => CustomNotificationPosition != ControlExtensions.InvisibleLocation;
        public bool IsCustomNotificationSizeSet => CustomNotificationSize != Size.Empty;
        public bool IsCustomSoundNotificationSet => NotificationSoundPath != string.Empty;

        public ImageQuality TwitterImageQuality => BestImageQuality ? ImageQuality.Best : ImageQuality.Default;
        
        public string NotificationSoundPath{
            get => _notificationSoundPath ?? string.Empty;
            set => UpdatePropertyWithEvent(ref _notificationSoundPath, value, SoundNotificationChanged);
        }
        
        public int NotificationSoundVolume{
            get => _notificationSoundVolume;
            set => UpdatePropertyWithEvent(ref _notificationSoundVolume, value, SoundNotificationChanged);
        }

        public bool MuteNotifications{
            get => _muteNotifications;
            set => UpdatePropertyWithEvent(ref _muteNotifications, value, MuteToggled);
        }

        public int ZoomLevel{
            get => _zoomLevel;
            set => UpdatePropertyWithEvent(ref _zoomLevel, value, ZoomLevelChanged);
        }
        
        public TrayIcon.Behavior TrayBehavior{
            get => _trayBehavior;
            set => UpdatePropertyWithEvent(ref _trayBehavior, value, TrayBehaviorChanged);
        }
        
        public bool EnableSmoothScrolling{
            get => _enableSmoothScrolling;
            set => UpdatePropertyWithRestartRequest(ref _enableSmoothScrolling, value);
        }

        public bool EnableTouchAdjustment{
            get => _enableTouchAdjustment;
            set => UpdatePropertyWithRestartRequest(ref _enableTouchAdjustment, value);
        }

        public string CustomCefArgs{
            get => _customCefArgs;
            set => UpdatePropertyWithRestartRequest(ref _customCefArgs, value);
        }

        public string SpellCheckLanguage{
            get => _spellCheckLanguage;
            set => UpdatePropertyWithRestartRequest(ref _spellCheckLanguage, value);
        }

        // EVENTS
        
        public event EventHandler MuteToggled;
        public event EventHandler ZoomLevelChanged;
        public event EventHandler TrayBehaviorChanged;
        public event EventHandler SoundNotificationChanged;

        // END OF CONFIG
        
        public UserConfig(IConfigManager configManager) : base(configManager){}

        protected override BaseConfig ConstructWithDefaults(IConfigManager configManager){
            return new UserConfig(configManager);
        }
    }
}
