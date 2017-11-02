using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TweetDuck.Plugins.Events;

namespace TweetDuck.Plugins{
    sealed class PluginConfig{
        public event EventHandler<PluginChangedStateEventArgs> InternalPluginChangedState; // should only be accessed from PluginManager

        public IEnumerable<string> DisabledPlugins => disabled;
        public bool AnyDisabled => disabled.Count > 0;

        private static readonly string[] DefaultDisabled = {
            "official/clear-columns",
            "official/reply-account"
        };

        private readonly HashSet<string> disabled = new HashSet<string>();

        public void SetEnabled(Plugin plugin, bool enabled){
            if ((enabled && disabled.Remove(plugin.Identifier)) || (!enabled && disabled.Add(plugin.Identifier))){
                InternalPluginChangedState?.Invoke(this, new PluginChangedStateEventArgs(plugin, enabled));
            }
        }

        public void ToggleEnabled(Plugin plugin){
            SetEnabled(plugin, !IsEnabled(plugin));
        }

        public bool IsEnabled(Plugin plugin){
            return !disabled.Contains(plugin.Identifier);
        }

        public void Load(string file){
            try{
                using(StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8)){
                    string line = reader.ReadLine();

                    if (line == "#Disabled"){
                        disabled.Clear();

                        while((line = reader.ReadLine()) != null){
                            disabled.Add(line);
                        }
                    }
                }
            }catch(FileNotFoundException){
                disabled.Clear();
                disabled.UnionWith(DefaultDisabled);
                Save(file);
            }catch(DirectoryNotFoundException){
            }catch(Exception e){
                Program.Reporter.HandleException("Plugin Configuration Error", "Could not read the plugin configuration file. If you continue, the list of disabled plugins will be reset to default.", true, e);
            }
        }

        public void Save(string file){
            try{
                using(StreamWriter writer = new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8)){
                    writer.WriteLine("#Disabled");

                    foreach(string identifier in disabled){
                        writer.WriteLine(identifier);
                    }
                }
            }catch(Exception e){
                Program.Reporter.HandleException("Plugin Configuration Error", "Could not save the plugin configuration file.", true, e);
            }
        }
    }
}
