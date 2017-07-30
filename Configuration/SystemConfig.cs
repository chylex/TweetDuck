using System;
using System.IO;
using TweetDuck.Core.Utils;
using TweetDuck.Data.Serialization;

namespace TweetDuck.Configuration{
    sealed class SystemConfig{
        private static readonly FileSerializer<SystemConfig> Serializer = new FileSerializer<SystemConfig>{
            OnReadUnknownProperty = (obj, property, value) => false
        };

        public static readonly bool IsHardwareAccelerationSupported = File.Exists(Path.Combine(Program.ProgramPath, "libEGL.dll")) &&
                                                                      File.Exists(Path.Combine(Program.ProgramPath, "libGLESv2.dll"));

        // CONFIGURATION DATA

        private bool _hardwareAcceleration = true;

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

        public bool Save(){
            try{
                WindowsUtils.CreateDirectoryForFile(file);
                Serializer.Write(file, this);
                return true;
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not save the system configuration file.", true, e);
                return false;
            }
        }
        
        public static SystemConfig Load(string file){
            SystemConfig config = new SystemConfig(file);
            
            try{
                Serializer.Read(file, config);
                return config;
            }catch(FileNotFoundException){
            }catch(DirectoryNotFoundException){
            }catch(FormatException){
                try{
                    using(Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)){
                        config.HardwareAcceleration = stream.ReadByte() > 0;
                    }

                    config.Save();
                }catch(Exception e){
                    Program.Reporter.HandleException("Configuration Error", "Could not update the system configuration file.", true, e);
                }
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not open the system configuration file. If you continue, you will lose system specific configuration such as Hardware Acceleration.", true, e);
            }

            return config;
        }
    }
}
