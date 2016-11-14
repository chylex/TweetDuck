using System.Text;
using TweetDck.Plugins.Enums;

namespace TweetDck.Plugins{
    static class PluginScriptGenerator{
        public static string GenerateConfig(PluginConfig config){
            return config.AnyDisabled ? "window.TD_PLUGINS.disabled = [\""+string.Join("\",\"", config.DisabledPlugins)+"\"];" : string.Empty;
        }

        public static string GeneratePlugin(string pluginIdentifier, string pluginContents, int pluginToken, PluginEnvironment environment){
            StringBuilder build = new StringBuilder(2*pluginIdentifier.Length+pluginContents.Length+165);

            build.Append("(function(").Append(environment.GetScriptVariables()).Append("){");
            
            build.Append("let tmp={");
            build.Append("id:\"").Append(pluginIdentifier).Append("\",");
            build.Append("obj:new class extends PluginBase{").Append(pluginContents).Append("}");
            build.Append("};");
            
            build.Append("tmp.obj.$id=\"").Append(pluginIdentifier).Append("\";");
            build.Append("tmp.obj.$token=").Append(pluginToken).Append(";");
            build.Append("window.TD_PLUGINS.install(tmp);");

            build.Append("})(").Append(environment.GetScriptVariables()).Append(");");

            return build.ToString();
        }
    }
}
