using System;
using System.IO;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Configuration{
    sealed class SystemConfig{
        private static readonly FileSerializer<SystemConfig> Serializer = new FileSerializer<SystemConfig>();

        public static readonly bool IsHardwareAccelerationSupported = File.Exists(Path.Combine(Program.ProgramPath, "libEGL.dll")) &&
                                                                      File.Exists(Path.Combine(Program.ProgramPath, "libGLESv2.dll"));

        // CONFIGURATION DATA

        private bool _hardwareAcceleration = true;

        public bool EnableBrowserGCReload { get; set; } = true;
        public int BrowserMemoryThreshold { get; set; } = 400;

        // SPECIAL PROPERTIES
        
        public bool HardwareAcceleration{
            get => _hardwareAcceleration && IsHardwareAccelerationSupported;
            set => _hardwareAcceleration = value;
        }

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
