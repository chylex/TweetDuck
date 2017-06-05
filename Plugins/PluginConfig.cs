using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TweetDuck.Plugins.Events;

namespace TweetDuck.Plugins{
    sealed class PluginConfig{
        public event EventHandler<PluginChangedStateEventArgs> InternalPluginChangedState; // should only be accessed from PluginManager

        public IEnumerable<string> DisabledPlugins => Disabled;
        public bool AnyDisabled => Disabled.Count > 0;

        private readonly HashSet<string> Disabled = new HashSet<string>{
            "official/clear-columns",
            "official/reply-account"
        };

        public void SetEnabled(Plugin plugin, bool enabled){
            if ((enabled && Disabled.Remove(plugin.Identifier)) || (!enabled && Disabled.Add(plugin.Identifier))){
                InternalPluginChangedState?.Invoke(this, new PluginChangedStateEventArgs(plugin, enabled));
            }
        }

        public bool IsEnabled(Plugin plugin){
            return !Disabled.Contains(plugin.Identifier);
        }

        public void Load(string file){
            try{
                using(FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                using(StreamReader reader = new StreamReader(stream, Encoding.UTF8)){
                    string line = reader.ReadLine();

                    if (line == "#Disabled"){
                        Disabled.Clear();

                        while((line = reader.ReadLine()) != null){
                            Disabled.Add(line);
                        }
                    }
                }
            }catch(FileNotFoundException){
            }catch(DirectoryNotFoundException){
            }catch(Exception e){
                Program.Reporter.HandleException("Plugin Configuration Error", "Could not read the plugin configuration file. If you continue, the list of disabled plugins will be reset to default.", true, e);
            }
        }

        public void Save(string file){
            try{
                using(FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
                using(StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)){
                    writer.WriteLine("#Disabled");

                    foreach(string disabled in Disabled){
                        writer.WriteLine(disabled);
                    }
                }
            }catch(Exception e){
                Program.Reporter.HandleException("Plugin Configuration Error", "Could not save the plugin configuration file.", true, e);
            }
        }
    }
}
