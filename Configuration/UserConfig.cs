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
        
        // CONFIGURATION DATA

        public WindowState BrowserWindow { get; set; } = new WindowState();
        public WindowState PluginsWindow { get; set; } = new WindowState();

        public bool ExpandLinksOnHover     { get; set; } = true;
        public bool SwitchAccountSelectors { get; set; } = true;
        public bool EnableSpellCheck       { get; set; } = false;
        private int _zoomLevel                           = 100;
        private bool _muteNotifications;
        
        private TrayIcon.Behavior _trayBehavior          = TrayIcon.Behavior.Disabled;
        public bool EnableTrayHighlight    { get; set; } = true;

        public bool EnableUpdateCheck { get; set; } = true;
        public string DismissedUpdate { get; set; } = null;

        public bool DisplayNotificationColumn    { get; set; } = false;
        public bool NotificationSkipOnLinkClick  { get; set; } = false;
        public bool NotificationNonIntrusiveMode { get; set; } = true;
        public int NotificationIdlePauseSeconds  { get; set; } = 0;

        public bool DisplayNotificationTimer     { get; set; } = true;
        public bool NotificationTimerCountDown   { get; set; } = false;
        public int NotificationDurationValue     { get; set; } = 25;

        public TweetNotification.Position NotificationPosition { get; set; } = TweetNotification.Position.TopRight;
        public Point CustomNotificationPosition                { get; set; } = ControlExtensions.InvisibleLocation;
        public int NotificationDisplay                         { get; set; } = 0;
        public int NotificationEdgeDistance                    { get; set; } = 8;

        public TweetNotification.Size NotificationSize { get; set; } = TweetNotification.Size.Auto;
        public Size CustomNotificationSize             { get; set; } = Size.Empty;
        public int NotificationScrollSpeed             { get; set; } = 10;

        private string _notificationSoundPath;

        public string CustomCefArgs         { get; set; } = null;
        public string CustomBrowserCSS      { get; set; } = null;
        public string CustomNotificationCSS { get; set; } = null;

        public bool EnableBrowserGCReload { get; set; } = false;
        public int BrowserMemoryThreshold { get; set; } = 350;
        
        // SPECIAL PROPERTIES

        public bool IsCustomNotificationPositionSet => CustomNotificationPosition != ControlExtensions.InvisibleLocation;
        public bool IsCustomNotificationSizeSet => CustomNotificationSize != Size.Empty;

        public string NotificationSoundPath{
            get => string.IsNullOrEmpty(_notificationSoundPath) ? string.Empty : _notificationSoundPath;
            set => _notificationSoundPath = value;
        }

        public bool MuteNotifications{
            get => _muteNotifications;

            set{
                if (_muteNotifications != value){
                    _muteNotifications = value;
                    MuteToggled?.Invoke(this, new EventArgs());
                }
            }
        }

        public int ZoomLevel{
            get => _zoomLevel;

            set{
                if (_zoomLevel != value){
                    _zoomLevel = value;
                    ZoomLevelChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public double ZoomMultiplier => _zoomLevel/100.0;

        public TrayIcon.Behavior TrayBehavior{
            get => _trayBehavior;

            set{
                if (_trayBehavior != value){
                    _trayBehavior = value;
                    TrayBehaviorChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        // EVENTS
        
        public event EventHandler MuteToggled;
        public event EventHandler ZoomLevelChanged;
        public event EventHandler TrayBehaviorChanged;
        
        private readonly string file;

        public UserConfig(string file){ // TODO make private after removing UserConfigLegacy
            this.file = file;
        }

        bool ISerializedObject.OnReadUnknownProperty(string property, string value){
            return false;
        }

        public bool Save(){
            try{
                WindowsUtils.CreateDirectoryForFile(file);

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
