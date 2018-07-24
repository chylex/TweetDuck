using System;
using System.Collections.Generic;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Configuration{
    sealed class SystemConfig{
        private static readonly FileSerializer<SystemConfig> Serializer = new FileSerializer<SystemConfig>();

        // CONFIGURATION DATA
        
        public bool _hardwareAcceleration = true;
        
        public bool ClearCacheAutomatically { get; set; } = true;
        public int ClearCacheThreshold      { get; set; } = 250;

        // SPECIAL PROPERTIES
        
        public bool HardwareAcceleration{
            get => _hardwareAcceleration;
            set => UpdatePropertyWithEvent(ref _hardwareAcceleration, value, ProgramRestartRequested);
        }

        // EVENTS
        
        public event EventHandler ProgramRestartRequested;

        // END OF CONFIG

        private readonly string file;
        
        private SystemConfig(string file){
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
