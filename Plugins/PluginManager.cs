using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TweetDuck.Plugins.Enums;
using TweetDuck.Plugins.Events;
using TweetDuck.Resources;

namespace TweetDuck.Plugins{
    sealed class PluginManager{
        private const int InvalidToken = 0;

        private static readonly Dictionary<PluginEnvironment, string> PluginSetupScripts = new Dictionary<PluginEnvironment, string>(4){
            { PluginEnvironment.None, ScriptLoader.LoadResource("plugins.js") },
            { PluginEnvironment.Browser, ScriptLoader.LoadResource("plugins.browser.js") },
            { PluginEnvironment.Notification, ScriptLoader.LoadResource("plugins.notification.js") }
        };

        public string PathOfficialPlugins => Path.Combine(rootPath, "official");
        public string PathCustomPlugins => Path.Combine(rootPath, "user");

        public IEnumerable<Plugin> Plugins => plugins;
        public PluginConfig Config { get; }
        public PluginBridge Bridge { get; }
        
        public event EventHandler<PluginErrorEventArgs> Reloaded;
        public event EventHandler<PluginErrorEventArgs> Executed;
        public event EventHandler<PluginChangedStateEventArgs> PluginChangedState;

        private readonly string rootPath;
        private readonly string configPath;

        private readonly HashSet<Plugin> plugins = new HashSet<Plugin>();
        private readonly Dictionary<int, Plugin> tokens = new Dictionary<int, Plugin>();
        private readonly Random rand = new Random();

        private List<string> loadErrors;

        public PluginManager(string rootPath, string configPath){
            this.rootPath = rootPath;
            this.configPath = configPath;

            this.Config = new PluginConfig();
            this.Bridge = new PluginBridge(this);

            Config.Load(configPath);
            
            Config.InternalPluginChangedState += Config_InternalPluginChangedState;
            Program.UserConfigReplaced += Program_UserConfigReplaced;
        }

        private void Program_UserConfigReplaced(object sender, EventArgs e){
            Config.Load(configPath);
            Reload();
        }

        private void Config_InternalPluginChangedState(object sender, PluginChangedStateEventArgs e){
            PluginChangedState?.Invoke(this, e);
            Config.Save(configPath);
        }

        public bool IsPluginInstalled(string identifier){
            return plugins.Any(plugin => plugin.Identifier.Equals(identifier));
        }

        public bool HasAnyPlugin(PluginEnvironment environment){
            return plugins.Any(plugin => plugin.Environments.HasFlag(environment));
        }

        public int GetTokenFromPlugin(Plugin plugin){
            foreach(KeyValuePair<int, Plugin> kvp in tokens){
                if (kvp.Value.Equals(plugin)){
                    return kvp.Key;
                }
            }

            return InvalidToken;
        }

        public Plugin GetPluginFromToken(int token){
            return tokens.TryGetValue(token, out Plugin plugin) ? plugin : null;
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

            Reloaded?.Invoke(this, new PluginErrorEventArgs(loadErrors));
        }

        public void ExecutePlugins(IFrame frame, PluginEnvironment environment){
            if (HasAnyPlugin(environment)){
                ScriptLoader.ExecuteScript(frame, PluginSetupScripts[environment], environment.GetScriptIdentifier());
                ScriptLoader.ExecuteScript(frame, PluginSetupScripts[PluginEnvironment.None], PluginEnvironment.None.GetScriptIdentifier());
                ExecutePluginScripts(frame, environment);
            }
        }

        private void ExecutePluginScripts(IFrame frame, PluginEnvironment environment){
            bool includeDisabled = environment.IncludesDisabledPlugins();

            if (includeDisabled){
                ScriptLoader.ExecuteScript(frame, PluginScriptGenerator.GenerateConfig(Config), "gen:pluginconfig");
            }

            List<string> failedPlugins = new List<string>(1);

            foreach(Plugin plugin in Plugins){
                string path = plugin.GetScriptPath(environment);
                if (string.IsNullOrEmpty(path) || (!includeDisabled && !Config.IsEnabled(plugin)) || !plugin.CanRun)continue;

                string script;

                try{
                    script = File.ReadAllText(path);
                }catch(Exception e){
                    failedPlugins.Add(plugin.Identifier+" ("+Path.GetFileName(path)+"): "+e.Message);
                    continue;
                }

                int token;

                if (tokens.ContainsValue(plugin)){
                    token = GetTokenFromPlugin(plugin);
                }
                else{
                    token = GenerateToken();
                    tokens[token] = plugin;
                }

                ScriptLoader.ExecuteScript(frame, PluginScriptGenerator.GeneratePlugin(plugin.Identifier, script, token, environment), "plugin:"+plugin);
            }

            Executed?.Invoke(this, new PluginErrorEventArgs(failedPlugins));
        }

        private IEnumerable<Plugin> LoadPluginsFrom(string path, PluginGroup group){
            if (!Directory.Exists(path)){
                yield break;
            }

            foreach(string fullDir in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly)){
                Plugin plugin = Plugin.CreateFromFolder(fullDir, group, out string error);

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

                if (!tokens.ContainsKey(token) && token != InvalidToken){
                    return token;
                }
            }

            return -tokens.Count;
        }
    }
}
