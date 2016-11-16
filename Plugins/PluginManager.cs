using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CefSharp;
using TweetDck.Plugins.Enums;
using TweetDck.Plugins.Events;
using TweetDck.Resources;

namespace TweetDck.Plugins{
    class PluginManager{
        public const string PluginBrowserScriptFile = "plugins.browser.js";
        public const string PluginNotificationScriptFile = "plugins.notification.js";
        public const string PluginGlobalScriptFile = "plugins.js";

        public string PathOfficialPlugins { get { return Path.Combine(rootPath, "official"); } }
        public string PathCustomPlugins { get { return Path.Combine(rootPath, "user"); } }

        public IEnumerable<Plugin> Plugins { get { return plugins; } }
        public PluginConfig Config { get; private set; }
        public PluginBridge Bridge { get; private set; }
        
        public event EventHandler<PluginLoadEventArgs> Reloaded;
        public event EventHandler<PluginChangedStateEventArgs> PluginChangedState;

        private readonly string rootPath;
        private readonly HashSet<Plugin> plugins = new HashSet<Plugin>();
        private readonly Dictionary<int, Plugin> tokens = new Dictionary<int, Plugin>();
        private readonly Random rand = new Random();

        private List<string> loadErrors;

        public PluginManager(string path, PluginConfig config){
            this.rootPath = path;
            this.SetConfig(config);
            this.Bridge = new PluginBridge(this);
        }

        public void SetConfig(PluginConfig config){
            this.Config = config;
            this.Config.InternalPluginChangedState += Config_InternalPluginChangedState;
        }

        private void Config_InternalPluginChangedState(object sender, PluginChangedStateEventArgs e){
            if (PluginChangedState != null){
                PluginChangedState(this, e);
            }
        }

        public bool IsPluginInstalled(string identifier){
            return plugins.Any(plugin => plugin.Identifier.Equals(identifier));
        }

        public IEnumerable<Plugin> GetPluginsByGroup(PluginGroup group){
            return plugins.Where(plugin => plugin.Group == group);
        }

        public int CountPluginByGroup(PluginGroup group){
            return plugins.Count(plugin => plugin.Group == group);
        }

        public bool HasAnyPlugin(PluginEnvironment environment){
            return plugins.Any(plugin => plugin.Environments.HasFlag(environment));
        }

        public Plugin GetPluginFromToken(int token){
            Plugin plugin;
            return tokens.TryGetValue(token, out plugin) ? plugin : null;
        }

        public void Reload(){
            plugins.Clear();
            tokens.Clear();

            loadErrors = new List<string>(2);
            
            foreach(Plugin plugin in LoadPluginsFrom(PathOfficialPlugins, PluginGroup.Official)){
                plugins.Add(plugin);
            }

            foreach(Plugin plugin in LoadPluginsFrom(PathCustomPlugins, PluginGroup.Custom)){
                plugins.Add(plugin);
            }

            if (Reloaded != null){
                Reloaded(this, new PluginLoadEventArgs(loadErrors));
            }
        }

        public void ExecutePlugins(IFrame frame, PluginEnvironment environment, bool includeDisabled){
            if (includeDisabled){
                ScriptLoader.ExecuteScript(frame, PluginScriptGenerator.GenerateConfig(Config), "gen:pluginconfig");
            }

            foreach(Plugin plugin in Plugins){
                string path = plugin.GetScriptPath(environment);
                if (string.IsNullOrEmpty(path) || !plugin.CanRun || (!includeDisabled && !Config.IsEnabled(plugin)))continue;

                string script;

                try{
                    script = File.ReadAllText(path);
                }catch{
                    // TODO
                    continue;
                }

                int token;

                if (tokens.ContainsValue(plugin)){
                    token = tokens.First(kvp => kvp.Value.Equals(plugin)).Key;
                }
                else{
                    token = GenerateToken();
                    tokens[token] = plugin;
                }

                ScriptLoader.ExecuteScript(frame, PluginScriptGenerator.GeneratePlugin(plugin.Identifier, script, token, environment), "plugin:"+plugin);
            }
        }

        private IEnumerable<Plugin> LoadPluginsFrom(string path, PluginGroup group){
            if (!Directory.Exists(path)){
                yield break;
            }

            foreach(string fullDir in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly)){
                string error;
                Plugin plugin = Plugin.CreateFromFolder(fullDir, group, out error);

                if (plugin == null){
                    loadErrors.Add(group.GetIdentifierPrefix()+Path.GetFileName(fullDir)+": "+error);
                }
                else{
                    yield return plugin;
                }
            }
        }

        private int GenerateToken(){
            for(int attempt = 0; attempt < 1000; attempt++){
                int token = rand.Next();

                if (!tokens.ContainsKey(token)){
                    return token;
                }
            }

            return -tokens.Count;
        }
    }
}
