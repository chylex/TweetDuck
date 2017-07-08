using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TweetDuck.Core;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Notification;
using TweetDuck.Data;

namespace TweetDuck.Configuration{
    [Serializable]
    sealed class UserConfigLegacy{ // TODO remove eventually
        private static readonly IFormatter Formatter = new BinaryFormatter{ Binder = new LegacyBinder() };

        private class LegacyBinder : SerializationBinder{
            public override Type BindToType(string assemblyName, string typeName){
                return Type.GetType(string.Format("{0}, {1}", typeName.Replace("TweetDck", "TweetDuck").Replace(".UserConfig", ".UserConfigLegacy").Replace("Core.Utils.WindowState", "Data.WindowState"), assemblyName.Replace("TweetDck", "TweetDuck")));
            }
        }
        
        private const int CurrentFileVersion = 11;

        // START OF CONFIGURATION

        public WindowState BrowserWindow { get; set; }
        public WindowState PluginsWindow { get; set; }

        public bool DisplayNotificationColumn { get; set; }
        public bool DisplayNotificationTimer { get; set; }
        public bool NotificationTimerCountDown { get; set; }
        public bool NotificationSkipOnLinkClick { get; set; }
        public bool NotificationNonIntrusiveMode { get; set; }

        public int NotificationIdlePauseSeconds { get; set; }
        public int NotificationDurationValue { get; set; }
        public int NotificationScrollSpeed { get; set; }

        public TweetNotification.Position NotificationPosition { get; set; }
        public Point CustomNotificationPosition { get; set; }
        public int NotificationEdgeDistance { get; set; }
        public int NotificationDisplay { get; set; }

        public TweetNotification.Size NotificationSize { get; set; }
        public Size CustomNotificationSize { get; set; }

        public bool EnableSpellCheck { get; set; }
        public bool ExpandLinksOnHover { get; set; }
        public bool SwitchAccountSelectors { get; set; }
        public bool EnableTrayHighlight { get; set; }

        public bool EnableUpdateCheck { get; set; }
        public string DismissedUpdate { get; set; }

        public string CustomCefArgs { get; set; }
        public string CustomBrowserCSS { get; set; }
        public string CustomNotificationCSS { get; set; }
        
        public string NotificationSoundPath{
            get => string.IsNullOrEmpty(notificationSoundPath) ? string.Empty : notificationSoundPath;
            set => notificationSoundPath = value;
        }

        public bool MuteNotifications{
            get => muteNotifications;
            set => muteNotifications = value;
        }

        public int ZoomLevel{
            get => zoomLevel;
            set => zoomLevel = value;
        }

        public TrayIcon.Behavior TrayBehavior{
            get => trayBehavior;
            set => trayBehavior = value;
        }

        // END OF CONFIGURATION
        
        [NonSerialized]
        private string file;

        private int fileVersion;
        private bool muteNotifications;
        private int zoomLevel;
        private string notificationSoundPath;
        private TrayIcon.Behavior trayBehavior;

        private UserConfigLegacy(string file){
            this.file = file;

            BrowserWindow = new WindowState();
            ZoomLevel = 100;
            DisplayNotificationTimer = true;
            NotificationNonIntrusiveMode = true;
            NotificationPosition = TweetNotification.Position.TopRight;
            CustomNotificationPosition = ControlExtensions.InvisibleLocation;
            NotificationSize = TweetNotification.Size.Auto;
            NotificationEdgeDistance = 8;
            NotificationDurationValue = 25;
            NotificationScrollSpeed = 100;
            EnableUpdateCheck = true;
            ExpandLinksOnHover = true;
            SwitchAccountSelectors = true;
            EnableTrayHighlight = true;
            PluginsWindow = new WindowState();
        }

        private void UpgradeFile(){
            if (fileVersion == CurrentFileVersion){
                return;
            }

            // if outdated, cycle through all versions
            if (fileVersion <= 5){
                DisplayNotificationTimer = true;
                EnableUpdateCheck = true;
                ExpandLinksOnHover = true;
                BrowserWindow = new WindowState();
                PluginsWindow = new WindowState();
                EnableTrayHighlight = true;
                NotificationDurationValue = 25;
                fileVersion = 6;
            }

            if (fileVersion == 6){
                NotificationNonIntrusiveMode = true;
                ++fileVersion;
            }

            if (fileVersion == 7){
                ZoomLevel = 100;
                ++fileVersion;
            }

            if (fileVersion == 8){
                SwitchAccountSelectors = true;
                ++fileVersion;
            }

            if (fileVersion == 9){
                NotificationScrollSpeed = 100;
                ++fileVersion;
            }

            if (fileVersion == 10){
                NotificationSize = TweetNotification.Size.Auto;
                ++fileVersion;
            }

            // update the version
            fileVersion = CurrentFileVersion;
        }

        public UserConfig ConvertLegacy(){
            return new UserConfig(file){
                BrowserWindow = BrowserWindow,
                PluginsWindow = PluginsWindow,
                DisplayNotificationColumn = DisplayNotificationColumn,
                DisplayNotificationTimer = DisplayNotificationTimer,
                NotificationTimerCountDown = NotificationTimerCountDown,
                NotificationSkipOnLinkClick = NotificationSkipOnLinkClick,
                NotificationNonIntrusiveMode = NotificationNonIntrusiveMode,
                NotificationIdlePauseSeconds = NotificationIdlePauseSeconds,
                NotificationDurationValue = NotificationDurationValue,
                NotificationScrollSpeed = NotificationScrollSpeed,
                NotificationPosition = NotificationPosition,
                CustomNotificationPosition = CustomNotificationPosition,
                NotificationEdgeDistance = NotificationEdgeDistance,
                NotificationDisplay = NotificationDisplay,
                NotificationSize = NotificationSize,
                CustomNotificationSize = CustomNotificationSize,
                EnableSpellCheck = EnableSpellCheck,
                ExpandLinksOnHover = ExpandLinksOnHover,
                SwitchAccountSelectors = SwitchAccountSelectors,
                EnableTrayHighlight = EnableTrayHighlight,
                EnableUpdateCheck = EnableUpdateCheck,
                DismissedUpdate = DismissedUpdate,
                CustomCefArgs = CustomCefArgs,
                CustomBrowserCSS = CustomBrowserCSS,
                CustomNotificationCSS = CustomNotificationCSS,
                NotificationSoundPath = NotificationSoundPath,
                MuteNotifications = MuteNotifications,
                ZoomLevel = ZoomLevel,
                TrayBehavior = TrayBehavior
            };
        }
        
        public static UserConfig Load(string file){
            UserConfigLegacy config = null;
            
            try{
                using(Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)){
                    if ((config = Formatter.Deserialize(stream) as UserConfigLegacy) != null){
                        config.file = file;
                    }
                }

                config?.UpgradeFile();
            }catch(FileNotFoundException){
            }catch(DirectoryNotFoundException){
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not open the configuration file.", true, e);
            }

            return (config ?? new UserConfigLegacy(file)).ConvertLegacy();
        }
    }
}
