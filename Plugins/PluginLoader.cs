using System;
using System.IO;
using System.Linq;
using System.Text;
using TweetDuck.Plugins.Enums;

namespace TweetDuck.Plugins{
    static class PluginLoader{
        private static readonly string[] EndTag = { "[END]" };

        public static Plugin FromFolder(string path, PluginGroup group){
            string name = Path.GetFileName(path);

            if (string.IsNullOrEmpty(name)){
                throw new ArgumentException("Could not extract directory name from path: "+path);
            }
            
            Plugin.Builder builder = new Plugin.Builder(group, name, path, Path.Combine(Program.PluginDataPath, group.GetIdentifierPrefix(), name));

            foreach(string file in Directory.EnumerateFiles(path, "*.js", SearchOption.TopDirectoryOnly).Select(Path.GetFileName)){
                builder.AddEnvironment(PluginEnvironmentExtensions.Values.FirstOrDefault(env => file.Equals(env.GetPluginScriptFile(), StringComparison.Ordinal)));
            }

            string metaFile = Path.Combine(path, ".meta");

            if (!File.Exists(metaFile)){
                throw new ArgumentException("Plugin is missing a .meta file");
            }
            
            string currentTag = null, currentContents = string.Empty;

            foreach(string line in File.ReadAllLines(metaFile, Encoding.UTF8).Concat(EndTag).Select(line => line.TrimEnd()).Where(line => line.Length > 0)){
                if (line[0] == '[' && line[line.Length-1] == ']'){
                    if (currentTag != null){
                        SetProperty(builder, currentTag, currentContents);
                    }

                    currentTag = line.Substring(1, line.Length-2).ToUpper();
                    currentContents = string.Empty;

                    if (line.Equals(EndTag[0])){
                        break;
                    }
                }
                else if (currentTag != null){
                    currentContents = currentContents.Length == 0 ? line : currentContents+Environment.NewLine+line;
                }
                else{
                    throw new FormatException("Missing metadata tag before value: "+line);
                }
            }

            return builder.BuildAndSetup();
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
                case "REQUIRES":      builder.RequiredVersion = Version.TryParse(value, out Version version) ? version : throw new FormatException("Invalid required minimum version: "+value); break;
                default: throw new FormatException("Invalid metadata tag: "+tag);
            }
        }
    }
}
