using System.Text;

namespace TweetDck.Plugins{
    static class PluginScriptGenerator{
        public static void AppendStart(StringBuilder build, PluginEnvironment environment){
            build.Append("(function(").Append(environment.GetScriptVariables()).Append("){");
        }

        public static void AppendConfig(StringBuilder build, PluginConfig config){
            if (config.AnyDisabled){
                build.Append("PLUGINS.disabled = [\"").Append(string.Join("\",\"",config.DisabledPlugins)).Append("\"];");
            }
        }

        public static void AppendPlugin(StringBuilder build, string pluginIdentifier, string pluginContents){
            build.Append("PLUGINS.installed.push({");
            build.Append("id: \"").Append(pluginIdentifier).Append("\",");
            build.Append("obj: new class extends PluginBase{ ").Append(pluginContents).Append(" }");
            build.Append("});");
        }

        public static void AppendEnd(StringBuilder build, PluginEnvironment environment){
            build.Append("PLUGINS.load();");
            build.Append("})(").Append(environment.GetScriptVariables()).Append(");");
        }

        public static string GenerateSetPluginState(Plugin plugin, bool enabled){
            return new StringBuilder().Append("window.TD_PLUGINS.setState(\"").Append(plugin.Identifier).Append("\",").Append(enabled ? "true" : "false").Append(");").ToString();
        }
    }
}
