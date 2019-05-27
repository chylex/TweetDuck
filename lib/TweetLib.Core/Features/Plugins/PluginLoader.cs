using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TweetLib.Core.Data;
using TweetLib.Core.Features.Plugins.Enums;

namespace TweetLib.Core.Features.Plugins{
    public static class PluginLoader{
        private static readonly string[] EndTag = { "[END]" };

        public static IEnumerable<Result<Plugin>> AllInFolder(string pluginFolder, string pluginDataFolder, PluginGroup group){
            string path = Path.Combine(pluginFolder, group.GetSubFolder());

            if (!Directory.Exists(path)){
                yield break;
            }

            foreach(string fullDir in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly)){
                string name = Path.GetFileName(fullDir);
                string prefix = group.GetIdentifierPrefix();

                if (string.IsNullOrEmpty(name)){
                    yield return new Result<Plugin>(new DirectoryNotFoundException($"{prefix}(?): Could not extract directory name from path: {fullDir}"));
                    continue;
                }

                Result<Plugin> result;

                try{
                    result = new Result<Plugin>(FromFolder(name, fullDir, Path.Combine(pluginDataFolder, prefix, name), group));
                }catch(Exception e){
                    result = new Result<Plugin>(new Exception($"{prefix}{name}: {e.Message}", e));
                }

                yield return result;
            }
        }

        public static Plugin FromFolder(string name, string pathRoot, string pathData, PluginGroup group){
            Plugin.Builder builder = new Plugin.Builder(group, name, pathRoot, pathData);

            foreach(var environment in Directory.EnumerateFiles(pathRoot, "*.js", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).Select(EnvironmentFromFileName)){
                builder.AddEnvironment(environment);
            }

            string metaFile = Path.Combine(pathRoot, ".meta");

            if (!File.Exists(metaFile)){
                throw new ArgumentException("Plugin is missing a .meta file");
            }
            
            string? currentTag = null;
            string currentContents = string.Empty;

            foreach(string line in File.ReadAllLines(metaFile, Encoding.UTF8).Concat(EndTag).Select(line => line.TrimEnd()).Where(line => line.Length > 0)){
                if (line[0] == '[' && line[line.Length - 1] == ']'){
                    if (currentTag != null){
                        SetProperty(builder, currentTag, currentContents);
                    }

                    currentTag = line.Substring(1, line.Length - 2).ToUpper();
                    currentContents = string.Empty;

                    if (line.Equals(EndTag[0])){
                        break;
                    }
                }
                else if (currentTag != null){
                    currentContents = currentContents.Length == 0 ? line : currentContents + Environment.NewLine + line;
                }
                else{
                    throw new FormatException($"Missing metadata tag before value: {line}");
                }
            }

            return builder.BuildAndSetup();
        }

        private static PluginEnvironment EnvironmentFromFileName(string file){
            return PluginEnvironmentExtensions.Values.FirstOrDefault(env => file.Equals(env.GetPluginScriptFile(), StringComparison.Ordinal));
        }

        private static void SetProperty(Plugin.Builder builder, string tag, string value){
            switch(tag){
                case "NAME":          builder.Name = value; break;
                case "DESCRIPTION":   builder.Description = value; break;
                case "AUTHOR":        builder.Author = value; break;
                case "VERSION":       builder.Version = value; break;
                case "WEBSITE":       builder.Website = value; break;
                case "CONFIGFILE":    builder.ConfigFile = value; break;
                case "CONFIGDEFAULT": builder.ConfigDefault = value; break;
                case "REQUIRES":      builder.RequiredVersion = Version.TryParse(value, out Version version) ? version : throw new FormatException($"Invalid required minimum version: {value}"); break;
                default: throw new FormatException($"Invalid metadata tag: {tag}");
            }
        }
    }
}
