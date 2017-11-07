using System;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Core.Other.Analytics{
    sealed class AnalyticsFile{
        private static readonly FileSerializer<AnalyticsFile> Serializer = new FileSerializer<AnalyticsFile>();

        static AnalyticsFile(){
            Serializer.RegisterTypeConverter(typeof(DateTime), new SingleTypeConverter<DateTime>{
                ConvertToString = value => value.ToBinary().ToString(),
                ConvertToObject = value => DateTime.FromBinary(long.Parse(value))
            });
        }

        // STATE PROPERTIES
        
        public DateTime LastDataCollection  { get; set; } = DateTime.MinValue;
        public string LastCollectionVersion { get; set; } = null;
        public string LastCollectionMessage { get; set; } = null;

        // END OF DATA
        
        private readonly string file;
        
        private AnalyticsFile(string file){
            this.file = file;
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
