using System;
using System.IO;
using TweetDuck.Core.Utils;
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

        public bool Save(){
            try{
                WindowsUtils.CreateDirectoryForFile(file);
                Serializer.Write(file, this);
                return true;
            }catch(Exception e){
                Program.Reporter.HandleException("Analytics File Error", "Could not save the analytics file.", true, e);
                return false;
            }
        }
        
        public static AnalyticsFile Load(string file){
            AnalyticsFile config = new AnalyticsFile(file);
            
            try{
                Serializer.Read(file, config);
            }catch(FileNotFoundException){
            }catch(DirectoryNotFoundException){
            }catch(Exception e){
                Program.Reporter.HandleException("Analytics File Error", "Could not open the analytics file.", true, e);
            }

            return config;
        }
    }
}
