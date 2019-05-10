using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Core.Utils;
using TweetDuck.Resources;
using TweetLib.Core.Data;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetDuck.Plugins{
    sealed class PluginManager{
        private static readonly IReadOnlyDictionary<PluginEnvironment, string> PluginSetupScriptNames = PluginEnvironmentExtensions.Map(null, "plugins.browser.js", "plugins.notification.js");

        public string PathOfficialPlugins => Path.Combine(rootPath, "official");
        public string PathCustomPlugins => Path.Combine(rootPath, "user");

        public IEnumerable<Plugin> Plugins => plugins;
        public IEnumerable<InjectedHTML> NotificationInjections => bridge.NotificationInjections;

        public IPluginConfig Config { get; }
        
        public event EventHandler<PluginErrorEventArgs> Reloaded;
        public event EventHandler<PluginErrorEventArgs> Executed;
        
        private readonly string rootPath;
        private readonly PluginBridge bridge;

        private readonly HashSet<Plugin> plugins = new HashSet<Plugin>();
        private readonly Dictionary<int, Plugin> tokens = new Dictionary<int, Plugin>();
        private readonly Random rand = new Random();

        private IWebBrowser mainBrowser;

        public PluginManager(IPluginConfig config, string rootPath){
            this.Config = config;
            this.Config.PluginChangedState += Config_PluginChangedState;

            this.rootPath = rootPath;
            this.bridge = new PluginBridge(this);
        }

        public void Register(IWebBrowser browser, PluginEnvironment environment, Control sync, bool asMainBrowser = false){
            browser.FrameLoadEnd += (sender, args) => {
                IFrame frame = args.Frame;

                if (frame.IsMain && TwitterUtils.IsTweetDeckWebsite(frame)){
                    ExecutePlugins(frame, environment, sync);
                }
            };

            browser.RegisterAsyncJsObject("$TDP", bridge);

            if (asMainBrowser){
                mainBrowser = browser;
            }
        }

        private void Config_PluginChangedState(object sender, PluginChangedStateEventArgs e){
            mainBrowser?.ExecuteScriptAsync("TDPF_setPluginState", e.Plugin, e.IsEnabled);
        }

        public bool IsPluginInstalled(string identifier){
            return plugins.Any(plugin => plugin.Identifier.Equals(identifier));
        }

        public bool HasAnyPlugin(PluginEnvironment environment){
            return plugins.Any(plugin => plugin.Environments.HasFlag(environment));
        }

        public bool IsPluginConfigurable(Plugin plugin){
            return plugin.HasConfig || bridge.WithConfigureFunction.Contains(plugin);
        }

        public void ConfigurePlugin(Plugin plugin){
            if (bridge.WithConfigureFunction.Contains(plugin)){
                mainBrowser?.ExecuteScriptAsync("TDPF_configurePlugin", plugin);
            }
            else if (plugin.HasConfig){
                if (File.Exists(plugin.ConfigPath)){
                    using(Process.Start("explorer.exe", "/select,\""+plugin.ConfigPath.Replace('/', '\\')+"\"")){}
                }
                else{
                    using(Process.Start("explorer.exe", '"'+plugin.GetPluginFolder(PluginFolder.Data).Replace('/', '\\')+'"')){}
                }
            }
        }

        public int GetTokenFromPlugin(Plugin plugin){
            foreach(KeyValuePair<int, Plugin> kvp in tokens){
                if (kvp.Value.Equals(plugin)){
                    return kvp.Key;
                }
            }

            int token, attempts = 1000;

            do{
                token = rand.Next();
            }while(tokens.ContainsKey(token) && --attempts >= 0);
            
            if (attempts < 0){
                token = -tokens.Count-1;
            }

            tokens[token] = plugin;
            return token;
        }

        public Plugin GetPluginFromToken(int token){
            return tokens.TryGetValue(token, out Plugin plugin) ? plugin : null;
        }

        public void Reload(){
            plugins.Clear();
            tokens.Clear();

            List<string> loadErrors = new List<string>(2);

            IEnumerable<Plugin> LoadPluginsFrom(string path, PluginGroup group){
                if (!Directory.Exists(path)){
                    yield break;
                }

                foreach(string fullDir in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly)){
                    string name = Path.GetFileName(fullDir);

                    if (string.IsNullOrEmpty(name)){
                        loadErrors.Add($"{group.GetIdentifierPrefix()}(?): Could not extract directory name from path: {fullDir}");
                        continue;
                    }

                    Plugin plugin;

                    try{
                        plugin = PluginLoader.FromFolder(name, fullDir, Path.Combine(Program.PluginDataPath, group.GetIdentifierPrefix(), name), group);
                    }catch(Exception e){
                        loadErrors.Add($"{group.GetIdentifierPrefix()}{name}: {e.Message}");
                        continue;
                    }

                    yield return plugin;
                }
            }
            
            plugins.UnionWith(LoadPluginsFrom(PathOfficialPlugins, PluginGroup.Official));
            plugins.UnionWith(LoadPluginsFrom(PathCustomPlugins, PluginGroup.Custom));

            Reloaded?.Invoke(this, new PluginErrorEventArgs(loadErrors));
        }

        private void ExecutePlugins(IFrame frame, PluginEnvironment environment, Control sync){
            if (!HasAnyPlugin(environment) || !ScriptLoader.ExecuteFile(frame, PluginSetupScriptNames[environment], sync)){
                return;
            }
            
            bool includeDisabled = environment.IncludesDisabledPlugins();

            if (includeDisabled){
                ScriptLoader.ExecuteScript(frame, PluginScriptGenerator.GenerateConfig(Config), "gen:pluginconfig");
            }

            List<string> failedPlugins = new List<string>(1);

            foreach(Plugin plugin in Plugins){
                string path = plugin.GetScriptPath(environment);

                if (string.IsNullOrEmpty(path) || (!includeDisabled && !Config.IsEnabled(plugin)) || !plugin.CanRun){
                    continue;
                }

                string script;

                try{
                    script = File.ReadAllText(path);
                }catch(Exception e){
                    failedPlugins.Add(plugin.Identifier+" ("+Path.GetFileName(path)+"): "+e.Message);
                    continue;
                }
                
                ScriptLoader.ExecuteScript(frame, PluginScriptGenerator.GeneratePlugin(plugin.Identifier, script, GetTokenFromPlugin(plugin), environment), "plugin:"+plugin);
            }

            Executed?.Invoke(this, new PluginErrorEventArgs(failedPlugins));
        }
    }
}
