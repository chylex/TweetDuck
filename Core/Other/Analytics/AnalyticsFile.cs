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

        public Counter OpenOptions { get; private set; } = 0;
        public Counter OpenPlugins { get; private set; } = 0;
        public Counter OpenAbout   { get; private set; } = 0;
        public Counter OpenGuide   { get; private set; } = 0;

        public Counter DesktopNotifications { get; private set; } = 0;
        public Counter SoundNotifications   { get; private set; } = 0;
        public Counter NotificationMutes    { get; private set; } = 0;
        
        public Counter BrowserContextMenus           { get; private set; } = 0;
        public Counter BrowserExtraMouseButtons      { get; private set; } = 0;
        public Counter NotificationContextMenus      { get; private set; } = 0;
        public Counter NotificationExtraMouseButtons { get; private set; } = 0;
        public Counter NotificationKeyboardShortcuts { get; private set; } = 0;

        public Counter BrowserReloads   { get; private set; } = 0;
        public Counter CopiedUsernames  { get; private set; } = 0;
        public Counter ViewedImages     { get; private set; } = 0;
        public Counter DownloadedImages { get; private set; } = 0;
        public Counter DownloadedVideos { get; private set; } = 0;
        public Counter UsedROT13        { get; private set; } = 0;

        public Counter TweetScreenshots { get; private set; } = 0;
        public Counter TweetDetails     { get; private set; } = 0;
        public Counter VideoPlays       { get; private set; } = 0;

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
