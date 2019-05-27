using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;
using TweetDuck.Resources;
using TweetLib.Core.Data;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetDuck.Plugins{
    sealed class PluginManager : IPluginManager{
        private const string SetupScriptPrefix = "plugins.";

        public string PathCustomPlugins => Path.Combine(pluginFolder, PluginGroup.Custom.GetSubFolder());

        public IEnumerable<Plugin> Plugins => plugins;
        public IEnumerable<InjectedHTML> NotificationInjections => bridge.NotificationInjections;
        
        public IPluginConfig Config { get; }
        
        public event EventHandler<PluginErrorEventArgs> Reloaded;
        public event EventHandler<PluginErrorEventArgs> Executed;
        
        private readonly string pluginFolder;
        private readonly string pluginDataFolder;

        private readonly Control sync;
        private readonly PluginBridge bridge;

        private readonly HashSet<Plugin> plugins = new HashSet<Plugin>();
        private readonly Dictionary<int, Plugin> tokens = new Dictionary<int, Plugin>();
        private readonly Random rand = new Random();

        private IWebBrowser mainBrowser;

        public PluginManager(Control sync, IPluginConfig config, string pluginFolder, string pluginDataFolder){
            this.Config = config;
            this.Config.PluginChangedState += Config_PluginChangedState;

            this.pluginFolder = pluginFolder;
            this.pluginDataFolder = pluginDataFolder;
            
            this.sync = sync;
            this.bridge = new PluginBridge(this);
        }

        public void Register(IWebBrowser browser, PluginEnvironment environment, bool asMainBrowser = false){
            browser.FrameLoadEnd += (sender, args) => {
                IFrame frame = args.Frame;

                if (frame.IsMain && TwitterUtils.IsTweetDeckWebsite(frame)){
                    ExecutePlugins(frame, environment);
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
                    using(Process.Start("explorer.exe", "/select,\"" + plugin.ConfigPath.Replace('/', '\\') + "\"")){}
                }
                else{
                    using(Process.Start("explorer.exe", '"' + plugin.GetPluginFolder(PluginFolder.Data).Replace('/', '\\') + '"')){}
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
                token = -tokens.Count - 1;
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

            List<string> loadErrors = new List<string>(1);

            foreach(var result in PluginGroupExtensions.Values.SelectMany(group => PluginLoader.AllInFolder(pluginFolder, pluginDataFolder, group))){
                if (result.HasValue){
                    plugins.Add(result.Value);
                }
                else{
                    loadErrors.Add(result.Exception.Message);
                }
            }

            Reloaded?.Invoke(this, new PluginErrorEventArgs(loadErrors));
        }

        private void ExecutePlugins(IFrame frame, PluginEnvironment environment){
            if (!HasAnyPlugin(environment) || !ScriptLoader.ExecuteFile(frame, SetupScriptPrefix + environment.GetPluginScriptFile(), sync)){
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
                    failedPlugins.Add($"{plugin.Identifier} ({Path.GetFileName(path)}): {e.Message}");
                    continue;
                }
                
                ScriptLoader.ExecuteScript(frame, PluginScriptGenerator.GeneratePlugin(plugin.Identifier, script, GetTokenFromPlugin(plugin), environment), $"plugin:{plugin}");
            }

            sync.InvokeAsyncSafe(() => {
                Executed?.Invoke(this, new PluginErrorEventArgs(failedPlugins));
            });
        }
    }
}
