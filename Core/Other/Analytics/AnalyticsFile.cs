using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Core.Other.Analytics{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
    sealed class AnalyticsFile{
        private static readonly FileSerializer<AnalyticsFile> Serializer = new FileSerializer<AnalyticsFile>();

        static AnalyticsFile(){
            Serializer.RegisterTypeConverter(typeof(DateTime), new SingleTypeConverter<DateTime>{
                ConvertToString = value => value.ToBinary().ToString(),
                ConvertToObject = value => DateTime.FromBinary(long.Parse(value))
            });

            Serializer.RegisterTypeConverter(typeof(Counter), new SingleTypeConverter<Counter>{
                ConvertToString = value => value.Value.ToString(),
                ConvertToObject = value => new Counter(int.Parse(value))
            });
        }

        public static readonly AnalyticsFile Dummy = new AnalyticsFile(null);

        // STATE PROPERTIES
        
        public DateTime LastDataCollection  { get; set; } = DateTime.MinValue;
        public string LastCollectionVersion { get; set; } = null;
        public string LastCollectionMessage { get; set; } = null;

        // USAGE DATA

        public Counter CountOpenOptions { get; private set; } = 0;
        public Counter CountOpenPlugins { get; private set; } = 0;
        public Counter CountOpenAbout   { get; private set; } = 0;
        public Counter CountOpenGuide   { get; private set; } = 0;

        public Counter CountDesktopNotifications { get; private set; } = 0;
        public Counter CountSoundNotifications   { get; private set; } = 0;
        public Counter CountMuteNotifications    { get; private set; } = 0;
        
        public Counter CountBrowserContextMenus           { get; private set; } = 0;
        public Counter CountBrowserExtraMouseButtons      { get; private set; } = 0;
        public Counter CountNotificationContextMenus      { get; private set; } = 0;
        public Counter CountNotificationExtraMouseButtons { get; private set; } = 0;
        public Counter CountNotificationKeyboardShortcuts { get; private set; } = 0;

        public Counter CountBrowserReloads   { get; private set; } = 0;
        public Counter CountCopiedUsernames  { get; private set; } = 0;
        public Counter CountViewedImages     { get; private set; } = 0;
        public Counter CountDownloadedImages { get; private set; } = 0;
        public Counter CountDownloadedVideos { get; private set; } = 0;
        public Counter CountUsedROT13        { get; private set; } = 0;

        public Counter CountTweetScreenshots { get; private set; } = 0;
        public Counter CountTweetDetails     { get; private set; } = 0;
        public Counter CountVideoPlays       { get; private set; } = 0;

        // END OF DATA

        public event EventHandler PropertyChanged;
        
        private readonly string file;
        
        private AnalyticsFile(string file){
            this.file = file;
        }

        private void SetupProperties(){
            foreach(Counter counter in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.PropertyType == typeof(Counter)).Select(prop => (Counter)prop.GetValue(this))){
                counter.SetOwner(this);
            }
        }

        public void OnPropertyChanged(){
            PropertyChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Save(){
            if (file == null){
                return;
            }

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
            
            config.SetupProperties();
            return config;
        }

        public interface IProvider{
            AnalyticsFile AnalyticsFile { get; }
        }

        public sealed class Counter{
            public int Value { get; private set; }

            private AnalyticsFile owner;

            public Counter(int value){
                this.Value = value;
            }

            public void SetOwner(AnalyticsFile owner){
                this.owner = owner;
            }

            public void Trigger(){
                ++Value;
                owner?.OnPropertyChanged();
            }

            public static implicit operator int(Counter counter) => counter.Value;
            public static implicit operator Counter(int value) => new Counter(value);
        }
    }
}
