using System;
using System.IO;
using TweetLib.Core.Serialization;

namespace TweetLib.Core.Systems.Configuration{
    public sealed class FileConfigInstance<T> : IConfigInstance<T> where T : BaseConfig{
        public T Instance { get; }
        public FileSerializer<T> Serializer { get; }

        private readonly string filenameMain;
        private readonly string filenameBackup;
        private readonly string identifier;

        public FileConfigInstance(string filename, T instance, string identifier){
            this.filenameMain = filename;
            this.filenameBackup = filename + ".bak";
            this.identifier = identifier;

            this.Instance = instance;
            this.Serializer = new FileSerializer<T>();
        }

        private void LoadInternal(bool backup){
            Serializer.Read(backup ? filenameBackup : filenameMain, Instance);
        }

        public void Load(){
            Exception? firstException = null;
            
            for(int attempt = 0; attempt < 2; attempt++){
                try{
                    LoadInternal(attempt > 0);

                    if (firstException != null){ // silently log exception that caused a backup restore
                        App.ErrorHandler.Log(firstException.ToString());
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
                OnException($"The configuration file for {identifier} is outdated or corrupted. If you continue, your {identifier} will be reset.", firstException);
            }
            else if (firstException is SerializationSoftException sse){
                OnException($"{sse.Errors.Count} error{(sse.Errors.Count == 1 ? " was" : "s were")} encountered while loading the configuration file for {identifier}. If you continue, some of your {identifier} will be reset.", firstException);
            }
            else if (firstException != null){
                OnException($"Could not open the configuration file for {identifier}. If you continue, your {identifier} will be reset.", firstException);
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
                OnException($"{e.Errors.Count} error{(e.Errors.Count == 1 ? " was" : "s were")} encountered while saving the configuration file for {identifier}.", e);
            }catch(Exception e){
                OnException($"Could not save the configuration file for {identifier}.", e);
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
                    OnException($"Could not regenerate the configuration file for {identifier}.", e);
                }
            }catch(Exception e){
                OnException($"Could not reload the configuration file for {identifier}.", e);
            }
        }

        public void Reset(){
            try{
                File.Delete(filenameMain);
                File.Delete(filenameBackup);
            }catch(Exception e){
                OnException($"Could not delete configuration files to reset {identifier}.", e);
                return;
            }
            
            Reload();
        }

        private static void OnException(string message, Exception e){
            App.ErrorHandler.HandleException("Configuration Error", message, true, e);
        }
    }
}
