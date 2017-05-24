using System;
using System.IO;

namespace TweetDuck.Configuration{
    sealed class SystemConfig{
        public static readonly bool IsHardwareAccelerationSupported = File.Exists(Path.Combine(Program.ProgramPath, "libEGL.dll")) &&
                                                                      File.Exists(Path.Combine(Program.ProgramPath, "libGLESv2.dll"));

        public bool HardwareAcceleration{
            get => hardwareAcceleration && IsHardwareAccelerationSupported;
            set => hardwareAcceleration = value;
        }

        private readonly string file;

        private bool hardwareAcceleration;
        
        private SystemConfig(string file){
            this.file = file;

            HardwareAcceleration = true;
        }

        private void WriteToStream(Stream stream){
            stream.WriteByte((byte)(HardwareAcceleration ? 1 : 0));
        }

        private void ReadFromStream(Stream stream){
            HardwareAcceleration = stream.ReadByte() > 0;
        }

        public bool Save(){
            try{
                string directory = Path.GetDirectoryName(file);
                if (directory == null)return false;

                Directory.CreateDirectory(directory);

                using(Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None)){
                    WriteToStream(stream);
                }

                return true;
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not save the system configuration file.", true, e);
                return false;
            }
        }
        
        public static SystemConfig Load(string file){
            SystemConfig config = new SystemConfig(file);
            
            try{
                using(Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)){
                    config.ReadFromStream(stream);
                }
            }catch(FileNotFoundException){
            }catch(DirectoryNotFoundException){
            }catch(Exception e){
                Program.Reporter.HandleException("Configuration Error", "Could not open the system configuration file. If you continue, you will lose system specific configuration such as Hardware Acceleration.", true, e);
            }

            return config;
        }
    }
}
