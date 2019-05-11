using System;
using System.IO;
using TweetLib.Core.Features.Configuration;
using TweetLib.Core.Serialization;

namespace TweetDuck.Configuration.Instance{
    sealed class FileConfigInstance<T> : IConfigInstance<T> where T : BaseConfig{
        private const string ErrorTitle = "Configuration Error";

        public T Instance { get; }
        public FileSerializer<T> Serializer { get; }

        private readonly string filenameMain;
        private readonly string filenameBackup;
        private readonly string errorIdentifier;

        public FileConfigInstance(string filename, T instance, string errorIdentifier){
            this.filenameMain = filename;
            this.filenameBackup = filename+".bak";
            this.errorIdentifier = errorIdentifier;

            this.Instance = instance;
            this.Serializer = new FileSerializer<T>();
        }

        private void LoadInternal(bool backup){
            Serializer.Read(backup ? filenameBackup : filenameMain, Instance);
        }

        public void Load(){
            Exception firstException = null;
            
            for(int attempt = 0; attempt < 2; attempt++){
                try{
                    LoadInternal(attempt > 0);

                    if (firstException != null){ // silently log exception that caused a backup restore
                        Program.Reporter.LogImportant(firstException.ToString());
                    }

                    return;
                }catch(FileNotFoundException){
                }catch(DirectoryNotFoundException){
                    break;
                }catch(Exception e){
                    if (firstException == null){
                        firstException = e;
                    }
                }
            }
            
            if (firstException is FormatException){
                Program.Reporter.HandleException(ErrorTitle, "The configuration file for "+errorIdentifier+" is outdated or corrupted. If you continue, your "+errorIdentifier+" will be reset.", true, firstException);
            }
            else if (firstException is SerializationSoftException sse){
                Program.Reporter.HandleException(ErrorTitle, $"{sse.Errors.Count} error{(sse.Errors.Count == 1 ? " was" : "s were")} encountered while loading the configuration file for "+errorIdentifier+". If you continue, some of your "+errorIdentifier+" will be reset.", true, firstException);
            }
            else if (firstException != null){
                Program.Reporter.HandleException(ErrorTitle, "Could not open the configuration file for "+errorIdentifier+". If you continue, your "+errorIdentifier+" will be reset.", true, firstException);
            }
        }

        public void Save(){
            try{
                if (File.Exists(filenameMain)){
                    File.Delete(filenameBackup);
                    File.Move(filenameMain, filenameBackup);
                }

                Serializer.Write(filenameMain, Instance);
            }catch(SerializationSoftException e){
                Program.Reporter.HandleException(ErrorTitle, $"{e.Errors.Count} error{(e.Errors.Count == 1 ? " was" : "s were")} encountered while saving the configuration file for "+errorIdentifier+".", true, e);
            }catch(Exception e){
                Program.Reporter.HandleException(ErrorTitle, "Could not save the configuration file for "+errorIdentifier+".", true, e);
            }
        }

        public void Reload(){
            try{
                LoadInternal(false);
            }catch(FileNotFoundException){
                try{
                    Serializer.Write(filenameMain, Instance.ConstructWithDefaults<T>());
                    LoadInternal(false);
                }catch(Exception e){
                    Program.Reporter.HandleException(ErrorTitle, "Could not regenerate the configuration file for "+errorIdentifier+".", true, e);
                }
            }catch(Exception e){
                Program.Reporter.HandleException(ErrorTitle, "Could not reload the configuration file for "+errorIdentifier+".", true, e);
            }
        }

        public void Reset(){
            try{
                File.Delete(filenameMain);
                File.Delete(filenameBackup);
            }catch(Exception e){
                Program.Reporter.HandleException(ErrorTitle, "Could not delete configuration files to reset "+errorIdentifier+".", true, e);
                return;
            }
            
            Reload();
        }
    }
}
