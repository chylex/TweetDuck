using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TweetLib.Core.Browser;
using TweetLib.Core.Data;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Features.Plugins.Events;

namespace TweetLib.Core.Features.Plugins{
    public sealed class PluginManager{
        public string PathCustomPlugins => Path.Combine(pluginFolder, PluginGroup.Custom.GetSubFolder());

        public IEnumerable<Plugin> Plugins => plugins;
        public IEnumerable<InjectedHTML> NotificationInjections => bridge.NotificationInjections;
        
        public IPluginConfig Config { get; }
        
        public event EventHandler<PluginErrorEventArgs> Reloaded;
        public event EventHandler<PluginErrorEventArgs> Executed;
        
        private readonly string pluginFolder;
        private readonly string pluginDataFolder;

        private readonly PluginBridge bridge;
        private IScriptExecutor? browserExecutor;

        private readonly HashSet<Plugin> plugins = new HashSet<Plugin>();

        public PluginManager(IPluginConfig config, string pluginFolder, string pluginDataFolder){
            this.Config = config;
            this.Config.PluginChangedState += Config_PluginChangedState;

            this.pluginFolder = pluginFolder;
            this.pluginDataFolder = pluginDataFolder;
            
            this.bridge = new PluginBridge(this);
        }

        public void Register(PluginEnvironment environment, IPluginDispatcher dispatcher){
            dispatcher.AttachBridge("$TDP", bridge);
            dispatcher.Ready += (sender, args) => {
                IScriptExecutor executor = args.Executor;

                if (environment == PluginEnvironment.Browser){
                    browserExecutor = executor;
                }

                Execute(environment, executor);
            };
        }

        public void Reload(){
            plugins.Clear();

            List<string> errors = new List<string>(1);

            foreach(var result in PluginGroups.All.SelectMany(group => PluginLoader.AllInFolder(pluginFolder, pluginDataFolder, group))){
                if (result.HasValue){
                    plugins.Add(result.Value);
                }
                else{
                    errors.Add(result.Exception.Message);
                }
            }

            Reloaded?.Invoke(this, new PluginErrorEventArgs(errors));
        }

        private void Execute(PluginEnvironment environment, IScriptExecutor executor){
            if (!plugins.Any(plugin => plugin.HasEnvironment(environment)) || !executor.RunFile($"plugins.{environment.GetPluginScriptFile()}")){
                return;
            }
            
            bool includeDisabled = environment == PluginEnvironment.Browser;

            if (includeDisabled){
                executor.RunScript("gen:pluginconfig", PluginScriptGenerator.GenerateConfig(Config));
            }

            List<string> errors = new List<string>(1);

            foreach(Plugin plugin in Plugins){
                string path = plugin.GetScriptPath(environment);

                if (string.IsNullOrEmpty(path) || (!includeDisabled && !Config.IsEnabled(plugin)) || !plugin.CanRun){
                    continue;
                }

                string script;

                try{
                    script = File.ReadAllText(path);
                }catch(Exception e){
                    errors.Add($"{plugin.Identifier} ({Path.GetFileName(path)}): {e.Message}");
                    continue;
                }
                
                executor.RunScript($"plugin:{plugin}", PluginScriptGenerator.GeneratePlugin(plugin.Identifier, script, bridge.GetTokenFromPlugin(plugin), environment));
            }
            
            Executed?.Invoke(this, new PluginErrorEventArgs(errors));
        }

        private void Config_PluginChangedState(object sender, PluginChangedStateEventArgs e){
            browserExecutor?.RunFunction("TDPF_setPluginState", e.Plugin, e.IsEnabled);
        }

        public bool IsPluginConfigurable(Plugin plugin){
            return plugin.HasConfig || bridge.WithConfigureFunction.Contains(plugin);
        }

        public void ConfigurePlugin(Plugin plugin){
            if (bridge.WithConfigureFunction.Contains(plugin) && browserExecutor != null){
                browserExecutor.RunFunction("TDPF_configurePlugin", plugin);
            }
            else if (plugin.HasConfig){
                App.SystemHandler.OpenFileExplorer(File.Exists(plugin.ConfigPath) ? plugin.ConfigPath : plugin.GetPluginFolder(PluginFolder.Data));
            }
        }
    }
}
