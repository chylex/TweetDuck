using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TweetDuck.Configuration.Instance{
    class PluginConfigInstance : IConfigInstance<PluginConfig>{
        public PluginConfig Instance { get; }

        private readonly string filename;

        public PluginConfigInstance(string filename, PluginConfig instance){
            this.filename = filename;
            this.Instance = instance;
        }

        public void Load(){
            try{
                using(StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8)){
                    string line = reader.ReadLine();

                    if (line == "#Disabled"){
                        HashSet<string> newDisabled = new HashSet<string>();

                        while((line = reader.ReadLine()) != null){
                            newDisabled.Add(line);
                        }

                        Instance.ReloadSilently(newDisabled);
                    }
                }
            }catch(FileNotFoundException){
            }catch(DirectoryNotFoundException){
            }catch(Exception e){
                Program.Reporter.HandleException("Plugin Configuration Error", "Could not read the plugin configuration file. If you continue, the list of disabled plugins will be reset to default.", true, e);
            }
        }

        public void Save(){
            try{
                using(StreamWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8)){
                    writer.WriteLine("#Disabled");

                    foreach(string identifier in Instance.DisabledPlugins){
                        writer.WriteLine(identifier);
                    }
                }
            }catch(Exception e){
                Program.Reporter.HandleException("Plugin Configuration Error", "Could not save the plugin configuration file.", true, e);
            }
        }

        public void Reload(){
            Load();
        }

        public void Reset(){
            try{
                File.Delete(filename);
                Instance.ReloadSilently(Instance.ConstructWithDefaults<PluginConfig>().DisabledPlugins);
            }catch(Exception e){
                Program.Reporter.HandleException("Plugin Configuration Error", "Could not delete the plugin configuration file.", true, e);
                return;
            }
            
            Reload();
        }
    }
}
