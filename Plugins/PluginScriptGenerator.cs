using System.Globalization;
using TweetDuck.Plugins.Enums;

namespace TweetDuck.Plugins{
    static class PluginScriptGenerator{
        public static string GenerateConfig(PluginConfig config){
            return config.AnyDisabled ? "window.TD_PLUGINS.disabled = [\""+string.Join("\",\"", config.DisabledPlugins)+"\"];" : string.Empty;
        }

        public static string GeneratePlugin(string pluginIdentifier, string pluginContents, int pluginToken, PluginEnvironment environment){
            return PluginGen
                .Replace("%params", environment.GetScriptVariables())
                .Replace("%id", pluginIdentifier)
                .Replace("%token", pluginToken.ToString(CultureInfo.InvariantCulture))
                .Replace("%contents", pluginContents);
        }

        private const string PluginGen = "(function(%params,$d){let tmp={id:'%id',obj:new class extends PluginBase{%contents}};$d(tmp.obj,'$id',{value:'%id'});$d(tmp.obj,'$token',{value:%token});window.TD_PLUGINS.install(tmp);})(%params,Object.defineProperty);";

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
