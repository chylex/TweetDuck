using System.Linq;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Features.Plugins.Enums;

namespace TweetLib.Core.Features.Plugins {
	internal static class PluginScriptGenerator {
		public static string GenerateConfig(IPluginConfig config) {
			return "window.TD_PLUGINS_DISABLE = [" + string.Join(",", config.DisabledPlugins.Select(static id => '"' + id + '"')) + "]";
		}

		public static string GenerateInstaller() {
			return @"if (!window.TD_PLUGINS_INSTALL) { window.TD_PLUGINS_SETUP = []; window.TD_PLUGINS_INSTALL = function(f) { window.TD_PLUGINS_SETUP.push(f); }; }";
		}

		public static string GeneratePlugin(string pluginIdentifier, string pluginContents, int pluginToken, PluginEnvironment environment) {
			return PluginGen
			       .Replace("%params", environment.GetPluginScriptVariables())
			       .Replace("%id", pluginIdentifier)
			       .Replace("%token", pluginToken.ToString())
			       .Replace("%contents", pluginContents);
		}

		private const string PluginGen = "window.TD_PLUGINS_INSTALL(function(){" +
		                                 "return (function(%params,$d){let tmp={id:'%id',obj:new class extends PluginBase{%contents}};$d(tmp.obj,'$id',{value:'%id'});$d(tmp.obj,'$token',{value:%token});window.TD_PLUGINS.install(tmp);})(%params,Object.defineProperty);" +
		                                 "});";

		/* PluginGen

		(function(%params, $d){
		  let tmp = {
		    id: '%id',
		    obj: new class extends PluginBase{%contents}
		  };

		  $d(tmp.obj, '$id', { value: '%id' });
		  $d(tmp.obj, '$token', { value: %token });

		  window.TD_PLUGINS.install(tmp);
		})(%params, Object.defineProperty);

		*/
	}
}
