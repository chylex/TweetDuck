using System;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Core.Other.Analytics{
    sealed class AnalyticsFile{
        private static readonly FileSerializer<AnalyticsFile> Serializer = new FileSerializer<AnalyticsFile>{
            HandleUnknownProperties = FileSerializer<AnalyticsFile>.IgnoreProperties("CountGCReloads")
        };

        static AnalyticsFile(){
            Serializer.RegisterTypeConverter(typeof(DateTime), new SingleTypeConverter<DateTime>{
                ConvertToString = value => value.ToBinary().ToString(),
                ConvertToObject = value => DateTime.FromBinary(long.Parse(value))
            });
        }

        public enum Event{
            OpenOptions, OpenPlugins, OpenAbout, OpenGuide,
            DesktopNotification, SoundNotification, MuteNotification,
            BrowserContextMenu, BrowserExtraMouseButton,
            NotificationContextMenu, NotificationExtraMouseButton, NotificationKeyboardShortcut,
            TweetScreenshot, TweetDetail, VideoPlay
        }

        // STATE PROPERTIES
        
        public DateTime LastDataCollection  { get; set; } = DateTime.MinValue;
        public string LastCollectionVersion { get; set; } = null;
        public string LastCollectionMessage { get; set; } = null;

        // USAGE DATA

        public int CountOpenOptions { get; private set; } = 0;
        public int CountOpenPlugins { get; private set; } = 0;
        public int CountOpenAbout   { get; private set; } = 0;
        public int CountOpenGuide   { get; private set; } = 0;

        public int CountDesktopNotifications { get; private set; } = 0;
        public int CountSoundNotifications   { get; private set; } = 0;
        public int CountMuteNotifications    { get; private set; } = 0;
        
        public int CountBrowserContextMenus           { get; private set; } = 0;
        public int CountBrowserExtraMouseButtons      { get; private set; } = 0;
        public int CountNotificationContextMenus      { get; private set; } = 0;
        public int CountNotificationExtraMouseButtons { get; private set; } = 0;
        public int CountNotificationKeyboardShortcuts { get; private set; } = 0;

        public int CountTweetScreenshots { get; private set; } = 0;
        public int CountTweetDetails     { get; private set; } = 0;
        public int CountVideoPlays       { get; private set; } = 0;

        // END OF DATA
        
        private readonly string file;
        
        private AnalyticsFile(string file){
            this.file = file;
        }

        public void TriggerEvent(Event e){
            switch(e){
                case Event.OpenOptions: ++CountOpenOptions; break;
                case Event.OpenPlugins: ++CountOpenPlugins; break;
                case Event.OpenAbout: ++CountOpenAbout; break;
                case Event.OpenGuide: ++CountOpenGuide; break;

                case Event.DesktopNotification: ++CountDesktopNotifications; break;
                case Event.SoundNotification: ++CountSoundNotifications; break;
                case Event.MuteNotification: ++CountMuteNotifications; break;

                case Event.BrowserContextMenu: ++CountBrowserContextMenus; break;
                case Event.BrowserExtraMouseButton: ++CountBrowserExtraMouseButtons; break;
                case Event.NotificationContextMenu: ++CountNotificationContextMenus; break;
                case Event.NotificationExtraMouseButton: ++CountNotificationExtraMouseButtons; break;
                case Event.NotificationKeyboardShortcut: ++CountNotificationKeyboardShortcuts; break;

                case Event.TweetScreenshot: ++CountTweetScreenshots; break;
                case Event.TweetDetail: ++CountTweetDetails; break;
                case Event.VideoPlay: ++CountVideoPlays; break;
            }
        }

        public void Save(){
            try{
                Serializer.Write(file, this);
            }catch(Exception e){
                Program.Reporter.HandleException("Analytics File Error", "Could not save the analytics file.", true, e);
            }
        }
        
        public static AnalyticsFile Load(string file){
            AnalyticsFile config = new AnalyticsFile(file);
            
            try{
                Serializer.ReadIfExists(file, config);
            }catch(Exception e){
                Program.Reporter.HandleException("Analytics File Error", "Could not open the analytics file.", true, e);
            }

            return config;
        }
    }
}
