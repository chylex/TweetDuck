using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TweetDck.Plugins{
    class Plugin{
        public string Identifier { get { return identifier; } }
        public string Name { get { return metadata["NAME"]; } }
        public string Description { get { return metadata["DESCRIPTION"]; } }
        public string Author { get { return metadata["AUTHOR"]; } }
        public string Version { get { return metadata["VERSION"]; } }
        public string Website { get { return metadata["WEBSITE"]; } }
        public string RequiredVersion { get { return metadata["REQUIRES"]; } }
        public PluginGroup Group { get; private set; }
        public PluginEnvironment Environments { get; private set; }

        public bool CanRun{
            get{
                return canRun ?? (canRun = CheckRequiredVersion(RequiredVersion)).Value;
            }
        }

        public string FolderPath{
            get{
                return path;
            }
        }

        private readonly string path;
        private readonly string identifier;
        private readonly Dictionary<string,string> metadata = new Dictionary<string,string>(4){
            { "NAME", "" },
            { "DESCRIPTION", "" },
            { "AUTHOR", "(anonymous)" },
            { "VERSION", "(unknown)" },
            { "WEBSITE", "" },
            { "REQUIRES", "*" }
        };

        private bool? canRun;

        private Plugin(string path, PluginGroup group){
            this.path = path;
            this.identifier = group.GetIdentifierPrefix()+Path.GetFileName(path);
            this.Group = group;
            this.Environments = PluginEnvironment.None;
        }

        public string GetScriptPath(PluginEnvironment environment){
            if (Environments.HasFlag(environment)){
                string file = environment.GetScriptFile();
                return file != null ? Path.Combine(path,file) : string.Empty;
            }
            else{
                return string.Empty;
            }
        }

        public override string ToString(){
            return Identifier;
        }

        public override int GetHashCode(){
            return identifier.GetHashCode();
        }

        public override bool Equals(object obj){
            Plugin plugin = obj as Plugin;
            return plugin != null && plugin.path.Equals(path);
        }

        public static Plugin CreateFromFolder(string path, PluginGroup group, out string error){
            Plugin plugin = new Plugin(path,group);

            if (!LoadMetadata(path,plugin,out error)){
                return null;
            }

            if (!LoadEnvironments(path,plugin,out error)){
                return null;
            }

            error = string.Empty;
            return plugin;
        }

        private static bool LoadEnvironments(string path, Plugin plugin, out string error){
            foreach(string file in Directory.EnumerateFiles(path,"*.js",SearchOption.TopDirectoryOnly).Select(Path.GetFileName)){
                PluginEnvironment environment = PluginEnvironmentExtensions.Values.FirstOrDefault(env => file.Equals(env.GetScriptFile(),StringComparison.Ordinal));

                if (environment != PluginEnvironment.None){
                    plugin.Environments |= environment;
                }
                else{
                    error = "Unknown script file: "+file;
                    return false;
                }
            }

            if (plugin.Environments == PluginEnvironment.None){
                error = "Plugin has no script files.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static readonly string[] endTag = { "[END]" };

        private static bool LoadMetadata(string path, Plugin plugin, out string error){
            string metaFile = Path.Combine(path,".meta");

            if (!File.Exists(metaFile)){
                error = "Missing .meta file.";
                return false;
            }

            string[] lines = File.ReadAllLines(metaFile,Encoding.UTF8);
            string currentTag = null, currentContents = "";

            foreach(string line in lines.Concat(endTag).Select(line => line.TrimEnd()).Where(line => line.Length > 0)){
                if (line[0] == '[' && line[line.Length-1] == ']'){
                    if (currentTag != null){
                        plugin.metadata[currentTag] = currentContents;
                    }

                    currentTag = line.Substring(1,line.Length-2).ToUpperInvariant();
                    currentContents = "";

                    if (line.Equals(endTag[0])){
                        break;
                    }

                    if (!plugin.metadata.ContainsKey(currentTag)){
                        error = "Invalid metadata tag: "+currentTag;
                        return false;
                    }
                }
                else if (currentTag != null){
                    currentContents = currentContents.Length == 0 ? line : currentContents+"\r\n"+line;
                }
                else{
                    error = "Missing metadata tag before value: "+line;
                    return false;
                }
            }

            if (plugin.Name.Length == 0){
                error = "Plugin is missing a name in the .meta file.";
                return false;
            }

            Version ver;

            if (plugin.RequiredVersion.Length == 0 || !(plugin.RequiredVersion.Equals("*") || System.Version.TryParse(plugin.RequiredVersion,out ver))){
                error = "Plugin contains invalid version: "+plugin.RequiredVersion;
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool CheckRequiredVersion(string requires){
            return requires.Equals("*",StringComparison.Ordinal) || Program.Version >= new Version(requires);
        }
    }
}
