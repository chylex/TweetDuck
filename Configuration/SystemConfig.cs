using System;
using System.IO;
using TweetDuck.Core;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Configuration{
    sealed class SystemConfig{
        private static readonly FileSerializer<SystemConfig> Serializer = new FileSerializer<SystemConfig>();
        
        // CONFIGURATION DATA
        
        public bool HardwareAcceleration { get; set; } = true;

        public bool ClearCacheAutomatically { get; set; } = true;
        public int ClearCacheThreshold      { get; set; } = 250;

        public FormBrowser.ThrottleBehavior ThrottleBehavior { get; set; } = FormBrowser.ThrottleBehavior.Covered;

        // END OF CONFIG

        private readonly string file;
        
        private SystemConfig(string file){
            this.file = file;
        }

        public void Save(){
            try{
                Serializer.Write(file, this);
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not save the system configuration file.", true, e);
            }
        }
        
        public static SystemConfig Load(string file){
            SystemConfig config = new SystemConfig(file);
            
            try{
                Serializer.ReadIfExists(file, config);
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not open the system configuration file. If you continue, you will lose system specific configuration such as Hardware Acceleration.", true, e);
            }

            return config;
        }
    }
}
