using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TweetDick.Configuration{
    [Serializable]
    sealed class UserConfig{
        private static readonly IFormatter Formatter = new BinaryFormatter();

        // START OF CONFIGURATION

        public bool IgnoreMigration { get; set; }

        public bool IsMaximized { get; set; }
        public Point WindowLocation { get; set; }
        public Size WindowSize { get; set; }

        // END OF CONFIGURATION

        [NonSerialized]
        private readonly string file;

        private UserConfig(string file){
            this.file = file;
        }

        public bool Save(){
            try{
                string directory = Path.GetDirectoryName(file);
                if (directory == null)return false;

                Directory.CreateDirectory(directory);

                using(Stream stream = new FileStream(file,FileMode.Create,FileAccess.Write,FileShare.None)){
                    Formatter.Serialize(stream,this);
                }

                return true;
            }catch(Exception){
                // TODO
                return false;
            }
        }
        
        public static UserConfig Load(string file){
            UserConfig config = null;

            try{
                using(Stream stream = new FileStream(file,FileMode.Open,FileAccess.Read,FileShare.Read)){
                    config = Formatter.Deserialize(stream) as UserConfig;
                }
            }catch(FileNotFoundException){
            }catch(Exception){
                // TODO
            }

            return config ?? new UserConfig(file);
        }
    }
}
