using System;
using System.Drawing;
using System.IO;
using TweetDuck.Core;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Utils;
using TweetDuck.Data;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Configuration{
    sealed class UserConfig : ISerializedObject{
        private static readonly FileSerializer<UserConfig> Serializer = new FileSerializer<UserConfig>();

        static UserConfig(){
            Serializer.RegisterTypeConverter(typeof(WindowState), WindowState.Converter);

            Serializer.RegisterTypeConverter(typeof(Point), new SingleTypeConverter<Point>{
                ConvertToString = value => $"{value.X} {value.Y}",
                ConvertToObject = value => {
                    int[] elements = StringUtils.ParseInts(value, ' ');
                    return new Point(elements[0], elements[1]);
                }
            });
            
            Serializer.RegisterTypeConverter(typeof(Size), new SingleTypeConverter<Size>{
                ConvertToString = value => $"{value.Width} {value.Height}",
                ConvertToObject = value => {
                    int[] elements = StringUtils.ParseInts(value, ' ');
                    return new Size(elements[0], elements[1]);
                }
            });
        }
        
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

        public bool IsCustomNotificationPositionSet => CustomNotificationPosition != ControlExtensions.InvisibleLocation;
        public bool IsCustomNotificationSizeSet => CustomNotificationSize != Size.Empty;

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
        
        public event EventHandler MuteToggled;
        public event EventHandler ZoomLevelChanged;
        public event EventHandler TrayBehaviorChanged;
        
        private readonly string file;
        
        private bool muteNotifications;
        private int zoomLevel;
        private string notificationSoundPath;
        private TrayIcon.Behavior trayBehavior;

        public UserConfig(string file){ // TODO make private after removing UserConfigLegacy
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

        bool ISerializedObject.OnReadUnknownProperty(string property, string value){
            return false;
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

                Serializer.Write(file, this);
                return true;
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not save the configuration file.", true, e);
                return false;
            }
        }
        
        public static UserConfig Load(string file){
            Exception firstException = null;

            for(int attempt = 0; attempt < 2; attempt++){
                try{
                    UserConfig config = new UserConfig(file);
                    Serializer.Read(attempt == 0 ? file : GetBackupFile(file), config);
                    return config;
                }catch(FileNotFoundException){
                }catch(DirectoryNotFoundException){
                    break;
                }catch(FormatException){
                    UserConfig config = UserConfigLegacy.Load(file);
                    config.Save();
                    return config;
                }catch(Exception e){
                    if (attempt == 0){
                        firstException = e;
                        Program.Reporter.Log(e.ToString());
                    }
                    else if (firstException != null){
                        Program.Reporter.HandleException("Configuration Error", "Could not open the backup configuration file. If you continue, your program options will be reset.", true, e);
                    }
                }
            }

            if (firstException != null){
                Program.Reporter.HandleException("Configuration Error", "Could not open the configuration file.", true, firstException);
            }

            return new UserConfig(file);
        }

        public static string GetBackupFile(string file){
            return file+".bak";
        }
    }
}
