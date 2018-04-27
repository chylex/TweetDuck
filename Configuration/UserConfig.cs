using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Other;
using TweetDuck.Core.Utils;
using TweetDuck.Data;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Configuration{
    sealed class UserConfig{
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

        public bool FirstRun            { get; set; } = true;
        public bool AllowDataCollection { get; set; } = false;

        public WindowState BrowserWindow { get; set; } = new WindowState();
        public Size PluginsWindowSize    { get; set; } = Size.Empty;

        public bool ExpandLinksOnHover        { get; set; } = true;
        public bool OpenSearchInFirstColumn   { get; set; } = true;
        public bool KeepLikeFollowDialogsOpen { get; set; } = true;
        public bool BestImageQuality          { get; set; } = true;
        public bool EnableAnimatedImages      { get; set; } = true;

        public bool IgnoreTrackingUrlWarning { get; set; } = false;
        public bool EnableSmoothScrolling    { get; set; } = true;
        public string BrowserPath            { get; set; } = null;
        public string SearchEngineUrl        { get; set; } = null;
        private int _zoomLevel                             = 100;
        private bool _muteNotifications;

        public int VideoPlayerVolume { get; set; } = 50;
        
        public bool EnableSpellCheck     { get; set; } = false;
        public string SpellCheckLanguage { get; set; } = "en-US";
        public string TranslationTarget  { get; set; } = "en";
        
        private TrayIcon.Behavior _trayBehavior       = TrayIcon.Behavior.Disabled;
        public bool EnableTrayHighlight { get; set; } = true;

        public bool EnableUpdateCheck { get; set; } = true;
        public string DismissedUpdate { get; set; } = null;

        public bool DisplayNotificationColumn    { get; set; } = false;
        public bool NotificationMediaPreviews    { get; set; } = true;
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
        public int NotificationScrollSpeed             { get; set; } = 100;

        private string _notificationSoundPath;
        private int _notificationSoundVolume = 100;

        public string CustomCefArgs         { get; set; } = null;
        public string CustomBrowserCSS      { get; set; } = null;
        public string CustomNotificationCSS { get; set; } = null;
        
        // SPECIAL PROPERTIES

        public bool IsCustomNotificationPositionSet => CustomNotificationPosition != ControlExtensions.InvisibleLocation;
        public bool IsCustomNotificationSizeSet => CustomNotificationSize != Size.Empty;
        public bool IsCustomSoundNotificationSet => NotificationSoundPath != string.Empty;

        public TwitterUtils.ImageQuality TwitterImageQuality => BestImageQuality ? TwitterUtils.ImageQuality.Orig : TwitterUtils.ImageQuality.Default;
        
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

        // EVENTS
        
        public event EventHandler MuteToggled;
        public event EventHandler ZoomLevelChanged;
        public event EventHandler TrayBehaviorChanged;
        public event EventHandler SoundNotificationChanged;

        // END OF CONFIG
        
        private readonly string file;

        private UserConfig(string file){
            this.file = file;
        }

        private void UpdatePropertyWithEvent<T>(ref T field, T value, EventHandler eventHandler){
            if (!EqualityComparer<T>.Default.Equals(field, value)){
                field = value;
                eventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Save(){
            try{
                if (File.Exists(file)){
                    string backupFile = GetBackupFile(file);
                    File.Delete(backupFile);
                    File.Move(file, backupFile);
                }

                Serializer.Write(file, this);
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not save the configuration file.", true, e);
            }
        }

        public void Reload(){
            try{
                LoadInternal(false);
            }catch(FileNotFoundException){
                try{
                    Serializer.Write(file, new UserConfig(file));
                    LoadInternal(false);
                }catch(Exception e){
                    Program.Reporter.HandleException("Configuration Error", "Could not regenerate configuration file.", true, e);
                }
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not reload configuration file.", true, e);
            }
        }

        public void Reset(){
            try{
                File.Delete(file);
                File.Delete(GetBackupFile(file));
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not delete configuration files to reset the options.", true, e);
                return;
            }
            
            Reload();
        }

        private void LoadInternal(bool backup){
            Serializer.Read(backup ? GetBackupFile(file) : file, this);

            if (NotificationScrollSpeed == 10){ // incorrect initial value
                NotificationScrollSpeed = 100;
                Save();
            }
        }
        
        public static UserConfig Load(string file){
            Exception firstException = null;

            for(int attempt = 0; attempt < 2; attempt++){
                try{
                    UserConfig config = new UserConfig(file);
                    config.LoadInternal(attempt > 0);
                    return config;
                }catch(FileNotFoundException){
                }catch(DirectoryNotFoundException){
                    break;
                }catch(Exception e){
                    if (attempt == 0){
                        firstException = e;
                        Program.Reporter.Log(e.ToString());
                    }
                    else if (firstException is FormatException){
                        Program.Reporter.HandleException("Configuration Error", "The configuration file is outdated or corrupted. If you continue, your program options will be reset.", true, e);
                        return new UserConfig(file);
                    }
                    else if (firstException != null){
                        Program.Reporter.HandleException("Configuration Error", "Could not open the backup configuration file. If you continue, your program options will be reset.", true, e);
                        return new UserConfig(file);
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
