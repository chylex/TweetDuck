using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TweetDck.Plugins{
    class PluginManager{
        public string PathOfficialPlugins { get { return Path.Combine(rootPath,"official"); } }
        public string PathCustomPlugins { get { return Path.Combine(rootPath,"user"); } }

        public IEnumerable<Plugin> Plugins { get { return plugins; } }
        public PluginConfig Config { get; private set; }

        public event EventHandler<PluginLoadErrorEventArgs> ReloadError;

        private readonly string rootPath;
        private readonly HashSet<Plugin> plugins = new HashSet<Plugin>();

        private List<string> loadErrors;

        public PluginManager(string path, PluginConfig config){
            this.rootPath = path;
            this.Config = config;
        }

        public IEnumerable<Plugin> GetPluginsByGroup(PluginGroup group){
            return plugins.Where(plugin => plugin.Group == group);
        }

        public void Reload(){
            plugins.Clear();
            loadErrors = new List<string>(2);
            
            foreach(Plugin plugin in LoadPluginsFrom(PathOfficialPlugins,PluginGroup.Official)){
                plugins.Add(plugin);
            }

            foreach(Plugin plugin in LoadPluginsFrom(PathCustomPlugins,PluginGroup.Custom)){
                plugins.Add(plugin);
            }

            if (loadErrors.Count > 0 && ReloadError != null){
                ReloadError(this,new PluginLoadErrorEventArgs(loadErrors));
            }
        }

        private IEnumerable<Plugin> LoadPluginsFrom(string path, PluginGroup group){
            foreach(string fullDir in Directory.EnumerateDirectories(path,"*",SearchOption.TopDirectoryOnly)){
                string error;
                Plugin plugin = Plugin.CreateFromFolder(fullDir,group,out error);

                if (plugin == null){
                    loadErrors.Add(group.GetIdentifierPrefix()+Path.GetDirectoryName(fullDir)+": "+error);
                }
                else{
                    yield return plugin;
                }
            }
        }
    }
}
