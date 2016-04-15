using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TweetDck.Core.Handling;

namespace TweetDck.Configuration{
    [Serializable]
    sealed class UserConfig{
        private static readonly IFormatter Formatter = new BinaryFormatter(){
            Binder = new SerializationCompatibilityHandler()
        };

        // START OF CONFIGURATION

        public bool IgnoreMigration { get; set; }
        public bool IgnoreUninstallCheck { get; set; }

        public bool IsMaximized { get; set; }
        public Point WindowLocation { get; set; }
        public Size WindowSize { get; set; }

        public TweetNotification.Duration NotificationDuration { get; set; }
        public TweetNotification.Position NotificationPosition { get; set; }
        public Point CustomNotificationPosition { get; set; }
        public int NotificationEdgeDistance { get; set; }
        public int NotificationDisplay { get; set; }

        public bool IsCustomWindowLocationSet{
            get{
                return WindowLocation.X != -32000 && WindowLocation.X != 32000;
            }
        }

        public bool IsCustomNotificationPositionSet{
            get{
                return CustomNotificationPosition.X != -32000 && CustomNotificationPosition.X != 32000;
            }
        }

        // END OF CONFIGURATION

        [NonSerialized]
        private string file;

        private UserConfig(string file){
            this.file = file;

            IsMaximized = true;
            WindowLocation = new Point(-32000,-32000);
            NotificationDuration = TweetNotification.Duration.Medium;
            NotificationPosition = TweetNotification.Position.TopRight;
            CustomNotificationPosition = new Point(-32000,-32000);
            NotificationEdgeDistance = 8;
        }

        public bool Save(){
            try{
                string directory = Path.GetDirectoryName(file);
                if (directory == null)return false;

                Directory.CreateDirectory(directory);

                if (File.Exists(file)){
                    string backupFile = GetBackupFile(file);
                    File.Delete(backupFile);
                    File.Move(file,backupFile);
                }

                using(Stream stream = new FileStream(file,FileMode.Create,FileAccess.Write,FileShare.None)){
                    Formatter.Serialize(stream,this);
                }

                return true;
            }catch(Exception e){
                Program.HandleException("Could not save the configuration file.",e);
                return false;
            }
        }
        
        public static UserConfig Load(string file){
            UserConfig config = null;

            for(int attempt = 0; attempt < 2; attempt++){
                try{
                    using(Stream stream = new FileStream(attempt == 0 ? file : GetBackupFile(file),FileMode.Open,FileAccess.Read,FileShare.Read)){
                        if ((config = Formatter.Deserialize(stream) as UserConfig) != null){
                            config.file = file;
                        }
                    }
                    
                    break;
                }catch(FileNotFoundException){
                }catch(Exception e){
                    Program.HandleException("Could not open the configuration file.",e);
                }
            }

            return config ?? new UserConfig(file);
        }

        private static string GetBackupFile(string file){
            return file+".bak";
        }

        private class SerializationCompatibilityHandler : SerializationBinder{
            public override Type BindToType(string assemblyName, string typeName){
                typeName = typeName.Replace("TweetDick","TweetDck");
                return Type.GetType(string.Format("{0}, {1}",typeName,assemblyName));
            }
        }
    }
}
