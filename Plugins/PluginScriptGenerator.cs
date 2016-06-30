using System.Text;

namespace TweetDck.Plugins{
    static class PluginScriptGenerator{
        public static string GenerateConfig(PluginConfig config){
            return config.AnyDisabled ? "window.TD_PLUGINS.disabled = [\""+string.Join("\",\"",config.DisabledPlugins)+"\"];" : string.Empty;
        }

        public static string GeneratePlugin(string pluginIdentifier, string pluginContents, PluginEnvironment environment){
            StringBuilder build = new StringBuilder(pluginIdentifier.Length+pluginContents.Length+110);

            build.Append("(function(").Append(environment.GetScriptVariables()).Append("){");
            
            build.Append("window.TD_PLUGINS.install({");
            build.Append("id:\"").Append(pluginIdentifier).Append("\",");
            build.Append("obj:new class extends PluginBase{").Append(pluginContents).Append("}");
            build.Append("});");

            build.Append("})(").Append(environment.GetScriptVariables()).Append(");");

            return build.ToString();
        }
    }
}
