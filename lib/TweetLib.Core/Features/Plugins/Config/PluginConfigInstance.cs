﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TweetLib.Core.Features.Configuration;

namespace TweetLib.Core.Features.Plugins.Config{
    public sealed class PluginConfigInstance<T> : IConfigInstance<T> where T : BaseConfig, IPluginConfig{
        public T Instance { get; }

        private readonly string filename;

        public PluginConfigInstance(string filename, T instance){
            this.filename = filename;
            this.Instance = instance;
        }

        public void Load(){
            try{
                using var reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8);
                string line = reader.ReadLine();

                if (line == "#Disabled"){
                    HashSet<string> newDisabled = new HashSet<string>();

                    while((line = reader.ReadLine()) != null){
                        newDisabled.Add(line);
                    }

                    Instance.Reset(newDisabled);
                }
            }catch(FileNotFoundException){
            }catch(DirectoryNotFoundException){
            }catch(Exception e){
                OnException("Could not read the plugin configuration file. If you continue, the list of disabled plugins will be reset to default.", e);
            }
        }

        public void Save(){
            try{
                using var writer = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8);
                writer.WriteLine("#Disabled");

                foreach(string identifier in Instance.DisabledPlugins){
                    writer.WriteLine(identifier);
                }
            }catch(Exception e){
                OnException("Could not save the plugin configuration file.", e);
            }
        }

        public void Reload(){
            Load();
        }

        public void Reset(){
            try{
                File.Delete(filename);
                Instance.Reset(Instance.ConstructWithDefaults<T>().DisabledPlugins);
            }catch(Exception e){
                OnException("Could not delete the plugin configuration file.", e);
                return;
            }
            
            Reload();
        }

        private static void OnException(string message, Exception e){
            App.ErrorHandler.HandleException("Plugin Configuration Error", message, true, e);
        }
    }
}
