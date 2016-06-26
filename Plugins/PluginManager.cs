using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TweetDck.Plugins.Events;
using TweetDck.Resources;

namespace TweetDck.Plugins{
    class PluginManager{
        public string PathOfficialPlugins { get { return Path.Combine(rootPath,"official"); } }
        public string PathCustomPlugins { get { return Path.Combine(rootPath,"user"); } }

        public IEnumerable<Plugin> Plugins { get { return plugins; } }
        public PluginConfig Config { get; private set; }
        
        public event EventHandler<PluginLoadEventArgs> Reloaded;

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

        public int CountPluginByGroup(PluginGroup group){
            return plugins.Count(plugin => plugin.Group == group);
        }

        public void Reload(){
            HashSet<Plugin> prevPlugins = new HashSet<Plugin>(plugins);
            plugins.Clear();

            loadErrors = new List<string>(2);
            
            foreach(Plugin plugin in LoadPluginsFrom(PathOfficialPlugins,PluginGroup.Official)){
                plugins.Add(plugin);
            }

            foreach(Plugin plugin in LoadPluginsFrom(PathCustomPlugins,PluginGroup.Custom)){
                plugins.Add(plugin);
            }

            if (Reloaded != null && (loadErrors.Count > 0 || !prevPlugins.SetEquals(plugins))){
                Reloaded(this,new PluginLoadEventArgs(loadErrors));
            }
        }

        public string GenerateScript(PluginEnvironment environment){
            string mainScript = ScriptLoader.LoadResource("plugins.js");
            if (mainScript == null)return string.Empty;

            StringBuilder build = new StringBuilder((1+plugins.Count)*512);

            PluginScriptGenerator.AppendStart(build,environment);
            build.Append(mainScript);
            PluginScriptGenerator.AppendConfig(build,Config);

            foreach(Plugin plugin in Plugins){
                string path = plugin.GetScriptPath(environment);
                if (string.IsNullOrEmpty(path))continue;

                try{
                    PluginScriptGenerator.AppendPlugin(build,plugin.Identifier,File.ReadAllText(path));
                }catch{
                    // TODO
                }
            }
            
            PluginScriptGenerator.AppendEnd(build,environment);
            return build.ToString();
        }

        private IEnumerable<Plugin> LoadPluginsFrom(string path, PluginGroup group){
            foreach(string fullDir in Directory.EnumerateDirectories(path,"*",SearchOption.TopDirectoryOnly)){
                string error;
                Plugin plugin = Plugin.CreateFromFolder(fullDir,group,out error);

                if (plugin == null){
                    loadErrors.Add(group.GetIdentifierPrefix()+Path.GetFileName(fullDir)+": "+error);
                }
                else{
                    yield return plugin;
                }
            }
        }
    }
}
