using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TweetDck.Core;
using TweetDck.Core.Controls;
using TweetDck.Core.Notification;
using TweetDck.Core.Utils;
using TweetDck.Plugins;

namespace TweetDck.Configuration{
    [Serializable]
    sealed class UserConfig{
        private static readonly IFormatter Formatter = new BinaryFormatter();

        private const int CurrentFileVersion = 9;

        // START OF CONFIGURATION

        public WindowState BrowserWindow { get; set; }
        public bool DisplayNotificationColumn { get; set; }
        public bool DisplayNotificationTimer { get; set; }
        public bool NotificationTimerCountDown { get; set; }
        public bool NotificationSkipOnLinkClick { get; set; }
        public bool NotificationNonIntrusiveMode { get; set; }

        public TweetNotification.Position NotificationPosition { get; set; }
        public Point CustomNotificationPosition { get; set; }
        public int NotificationEdgeDistance { get; set; }
        public int NotificationDisplay { get; set; }
        public int NotificationIdlePauseSeconds { get; set; }
        public int NotificationDurationValue { get; set; }

        public bool EnableSpellCheck { get; set; }
        public bool ExpandLinksOnHover { get; set; }
        public bool SwitchAccountSelectors { get; set; }
        public bool EnableTrayHighlight { get; set; }

        public bool EnableUpdateCheck { get; set; }
        public string DismissedUpdate { get; set; }

        [Obsolete] public PluginConfig Plugins { get; set; } // TODO remove eventually
        public WindowState PluginsWindow { get; set; }

        public string CustomCefArgs { get; set; }
        public string CustomBrowserCSS { get; set; }
        public string CustomNotificationCSS { get; set; }

        public bool IsCustomNotificationPositionSet => CustomNotificationPosition != ControlExtensions.InvisibleLocation;

        public string NotificationSoundPath{
            get => string.IsNullOrEmpty(notificationSoundPath) ? string.Empty : notificationSoundPath;
            set => notificationSoundPath = value;
        }

        public bool MuteNotifications{
            get => muteNotifications;

            set{
                if (muteNotifications != value){
                    muteNotifications = value;
                    MuteToggled?.Invoke(this, new EventArgs());
                }
            }
        }

        public int ZoomLevel{
            get => zoomLevel;

            set{
                if (zoomLevel != value){
                    zoomLevel = value;
                    ZoomLevelChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public double ZoomMultiplier => zoomLevel/100.0;

        public TrayIcon.Behavior TrayBehavior{
            get => trayBehavior;

            set{
                if (trayBehavior != value){
                    trayBehavior = value;
                    TrayBehaviorChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        // END OF CONFIGURATION
        
        [field:NonSerialized]
        public event EventHandler MuteToggled;
        
        [field:NonSerialized]
        public event EventHandler ZoomLevelChanged;
        
        [field:NonSerialized]
        public event EventHandler TrayBehaviorChanged;

        [NonSerialized]
        private string file;

        private int fileVersion;
        private bool muteNotifications;
        private int zoomLevel;
        private string notificationSoundPath;
        private TrayIcon.Behavior trayBehavior;

        private UserConfig(string file){
            this.file = file;

            BrowserWindow = new WindowState();
            ZoomLevel = 100;
            DisplayNotificationTimer = true;
            NotificationNonIntrusiveMode = true;
            NotificationPosition = TweetNotification.Position.TopRight;
            CustomNotificationPosition = ControlExtensions.InvisibleLocation;
            NotificationEdgeDistance = 8;
            NotificationDurationValue = 25;
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
            if (fileVersion == 0){
                DisplayNotificationTimer = true;
                EnableUpdateCheck = true;
                ++fileVersion;
            }

            if (fileVersion == 1){
                ExpandLinksOnHover = true;
                ++fileVersion;
            }

            if (fileVersion == 2){
                BrowserWindow = new WindowState();
                PluginsWindow = new WindowState();
                ++fileVersion;
            }

            if (fileVersion == 3){
                EnableTrayHighlight = true;
                NotificationDurationValue = 25;
                ++fileVersion;
            }

            if (fileVersion == 4){
                ++fileVersion;
            }

            if (fileVersion == 5){
                ++fileVersion;
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

            // update the version
            fileVersion = CurrentFileVersion;
            Save();
        }

        public bool Save(){
            try{
                string directory = Path.GetDirectoryName(file);
                if (directory == null)return false;

                Directory.CreateDirectory(directory);

                if (File.Exists(file)){
                    string backupFile = GetBackupFile(file);
                    File.Delete(backupFile);
                    File.Move(file, backupFile);
                }

                using(Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None)){
                    Formatter.Serialize(stream, this);
                }

                return true;
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not save the configuration file.", true, e);
                return false;
            }
        }
        
        public static UserConfig Load(string file){
            UserConfig config = null;
            Exception firstException = null;

            for(int attempt = 0; attempt < 2; attempt++){
                try{
                    using(Stream stream = new FileStream(attempt == 0 ? file : GetBackupFile(file), FileMode.Open, FileAccess.Read, FileShare.Read)){
                        if ((config = Formatter.Deserialize(stream) as UserConfig) != null){
                            config.file = file;
                        }
                    }

                    config?.UpgradeFile();
                    break;
                }catch(FileNotFoundException){
                }catch(DirectoryNotFoundException){
                    break;
                }catch(Exception e){
                    if (attempt == 0){
                        firstException = e;
                        Program.Reporter.Log(e.ToString());
                    }
                    else if (firstException != null){
                        Program.Reporter.HandleException("Configuration Error", "Could not open the backup configuration file. If you continue, you may lose your settings and list of enabled plugins.", true, e);
                    }
                }
            }

            if (firstException != null && config == null){
                Program.Reporter.HandleException("Configuration Error", "Could not open the configuration file.", true, firstException);
            }

            return config ?? new UserConfig(file);
        }

        public static string GetBackupFile(string file){
            return file+".bak";
        }
    }
}
