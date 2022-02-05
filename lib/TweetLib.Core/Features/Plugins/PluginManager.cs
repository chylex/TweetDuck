using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Features.Plugins.Events;
using TweetLib.Core.Resources;
using TweetLib.Utils.Data;

namespace TweetLib.Core.Features.Plugins {
	public sealed class PluginManager {
		public IEnumerable<Plugin> Plugins => plugins;
		public IEnumerable<InjectedString> NotificationInjections => bridge.NotificationInjections;

		public PluginConfig Config { get; }
		public string PluginFolder { get; }
		public string PluginDataFolder { get; }

		internal event EventHandler<PluginErrorEventArgs>? Reloaded;
		internal event EventHandler<PluginErrorEventArgs>? Executed;

		internal readonly PluginBridge bridge;
		private IScriptExecutor? browserExecutor;

		private readonly HashSet<Plugin> plugins = new ();

		internal PluginManager(PluginConfig config, string pluginFolder, string pluginDataFolder) {
			this.Config = config;
			this.Config.PluginChangedState += Config_PluginChangedState;
			this.PluginFolder = pluginFolder;
			this.PluginDataFolder = pluginDataFolder;
			this.bridge = new PluginBridge(this);
		}

		public string GetPluginFolder(PluginGroup group) {
			return Path.Combine(PluginFolder, group.GetSubFolder());
		}

		internal void Register(PluginEnvironment environment, IBrowserComponent browserComponent) {
			browserComponent.AttachBridgeObject("$TDP", bridge);

			if (environment == PluginEnvironment.Browser) {
				browserExecutor = browserComponent;
			}
		}

		public void Reload() {
			plugins.Clear();

			var errors = new List<string>(1);

			foreach (var result in PluginGroups.All.SelectMany(group => PluginLoader.AllInFolder(PluginFolder, PluginDataFolder, group))) {
				if (result.HasValue) {
					plugins.Add(result.Value);
				}
				else {
					errors.Add(result.Exception.Message);
				}
			}

			Reloaded?.Invoke(this, new PluginErrorEventArgs(errors));
		}

		internal void Execute(PluginEnvironment environment, IScriptExecutor executor) {
			if (!plugins.Any(plugin => plugin.HasEnvironment(environment))) {
				return;
			}

			executor.RunScript("gen:pluginstall", PluginScriptGenerator.GenerateInstaller());

			bool includeDisabled = environment == PluginEnvironment.Browser;

			if (includeDisabled) {
				executor.RunScript("gen:pluginconfig", PluginScriptGenerator.GenerateConfig(Config));
			}

			var errors = new List<string>(1);

			foreach (Plugin plugin in Plugins) {
				string path = plugin.GetScriptPath(environment);

				if (string.IsNullOrEmpty(path) || (!includeDisabled && !Config.IsEnabled(plugin)) || !plugin.CanRun) {
					continue;
				}

				string script;

				try {
					script = File.ReadAllText(path);
				} catch (Exception e) {
					errors.Add($"{plugin.Identifier} ({Path.GetFileName(path)}): {e.Message}");
					continue;
				}

				executor.RunScript($"plugin:{plugin}", PluginScriptGenerator.GeneratePlugin(plugin.Identifier, script, bridge.GetTokenFromPlugin(plugin), environment));
			}

			executor.RunBootstrap($"plugins/{environment.GetPluginScriptNamespace()}");

			Executed?.Invoke(this, new PluginErrorEventArgs(errors));
		}

		private void Config_PluginChangedState(object sender, PluginChangedStateEventArgs e) {
			browserExecutor?.RunFunction("TDPF_setPluginState", e.Plugin, e.IsEnabled);
		}

		public bool IsPluginConfigurable(Plugin plugin) {
			return plugin.HasConfig || bridge.WithConfigureFunction.Contains(plugin);
		}

		public void ConfigurePlugin(Plugin plugin) {
			if (bridge.WithConfigureFunction.Contains(plugin) && browserExecutor != null) {
				browserExecutor.RunFunction("TDPF_configurePlugin", plugin);
			}
			else if (plugin.HasConfig) {
				App.SystemHandler.OpenFileExplorer(File.Exists(plugin.ConfigPath) ? plugin.ConfigPath : plugin.GetPluginFolder(Enums.PluginFolder.Data));
			}
		}
	}
}
