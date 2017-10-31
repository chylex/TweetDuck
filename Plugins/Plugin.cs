using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TweetDuck.Plugins.Enums;

namespace TweetDuck.Plugins{
    sealed class Plugin{
        private const string VersionWildcard = "*";

        public string Identifier { get; }
        public PluginGroup Group { get; }
        public PluginEnvironment Environments { get; }

        public string Name => metadata["NAME"];
        public string Description => metadata["DESCRIPTION"];
        public string Author => metadata["AUTHOR"];
        public string Version => metadata["VERSION"];
        public string Website => metadata["WEBSITE"];
        public string ConfigFile => metadata["CONFIGFILE"];
        public string ConfigDefault => metadata["CONFIGDEFAULT"];
        public string RequiredVersion => metadata["REQUIRES"];

        public bool CanRun { get; private set; }

        public bool HasConfig{
            get => ConfigFile.Length > 0 && GetFullPathIfSafe(PluginFolder.Data, ConfigFile).Length > 0;
        }

        public string ConfigPath{
            get => HasConfig ? Path.Combine(GetPluginFolder(PluginFolder.Data), ConfigFile) : string.Empty;
        }

        public bool HasDefaultConfig{
            get => ConfigDefault.Length > 0 && GetFullPathIfSafe(PluginFolder.Root, ConfigDefault).Length > 0;
        }

        public string DefaultConfigPath{
            get => HasDefaultConfig ? Path.Combine(GetPluginFolder(PluginFolder.Root), ConfigDefault) : string.Empty;
        }

        private readonly string pathRoot;
        private readonly string pathData;
        private readonly Dictionary<string, string> metadata = new Dictionary<string, string>(8){
            { "NAME", "" },
            { "DESCRIPTION", "" },
            { "AUTHOR", "(anonymous)" },
            { "VERSION", "(unknown)" },
            { "WEBSITE", "" },
            { "CONFIGFILE", "" },
            { "CONFIGDEFAULT", "" },
            { "REQUIRES", VersionWildcard }
        };

        private Plugin(string path, string name, PluginGroup group, PluginEnvironment environments){
            this.pathRoot = path;
            this.pathData = Path.Combine(Program.PluginDataPath, group.GetIdentifierPrefix(), name);

            this.Identifier = group.GetIdentifierPrefix()+name;
            this.Group = group;
            this.Environments = environments;
        }

        private void OnMetadataLoaded(){
            CanRun = CheckRequiredVersion(RequiredVersion);

            string configPath = ConfigPath, defaultConfigPath = DefaultConfigPath;

            if (configPath.Length > 0 && defaultConfigPath.Length > 0 && !File.Exists(configPath) && File.Exists(defaultConfigPath)){
                string dataFolder = GetPluginFolder(PluginFolder.Data);

                try{
                    Directory.CreateDirectory(dataFolder);
                    File.Copy(defaultConfigPath, configPath, false);
                }catch(Exception e){
                    throw new IOException("Could not generate a configuration file for '"+Identifier+"' plugin: "+e.Message, e);
                }
            }
        }

        public string GetScriptPath(PluginEnvironment environment){
            if (Environments.HasFlag(environment)){
                string file = environment.GetPluginScriptFile();
                return file != null ? Path.Combine(pathRoot, file) : string.Empty;
            }
            else{
                return string.Empty;
            }
        }

        public string GetPluginFolder(PluginFolder folder){
            switch(folder){
                case PluginFolder.Root: return pathRoot;
                case PluginFolder.Data: return pathData;
                default: return string.Empty;
            }
        }

        public string GetFullPathIfSafe(PluginFolder folder, string relativePath){
            string rootFolder = GetPluginFolder(folder);
            string fullPath = Path.Combine(rootFolder, relativePath);

            try{
                string folderPathName = new DirectoryInfo(rootFolder).FullName;
                DirectoryInfo currentInfo = new DirectoryInfo(fullPath);

                while(currentInfo.Parent != null){
                    if (currentInfo.Parent.FullName == folderPathName){
                        return fullPath;
                    }
                    
                    currentInfo = currentInfo.Parent;
                }
            }
            catch{
                // ignore
            }

            return string.Empty;
        }

        public override string ToString(){
            return Identifier;
        }

        public override int GetHashCode(){
            return Identifier.GetHashCode();
        }

        public override bool Equals(object obj){
            return obj is Plugin plugin && plugin.Identifier.Equals(Identifier);
        }

        // Static
        
        private static readonly Version AppVersion = new Version(Program.VersionTag);
        private static readonly string[] EndTag = { "[END]" };

        public static Plugin CreateFromFolder(string path, PluginGroup group){
            Plugin plugin = new Plugin(path, Path.GetFileName(path), group, LoadEnvironments(path));
            LoadMetadata(path, plugin);
            return plugin;
        }

        private static PluginEnvironment LoadEnvironments(string path){
            PluginEnvironment environments = PluginEnvironment.None;

            foreach(string file in Directory.EnumerateFiles(path, "*.js", SearchOption.TopDirectoryOnly).Select(Path.GetFileName)){
                environments |= PluginEnvironmentExtensions.Values.FirstOrDefault(env => file.Equals(env.GetPluginScriptFile(), StringComparison.Ordinal));
            }

            if (environments == PluginEnvironment.None){
                throw new ArgumentException("Plugin has no script files");
            }

            return environments;
        }

        private static void LoadMetadata(string path, Plugin plugin){
            string metaFile = Path.Combine(path, ".meta");

            if (!File.Exists(metaFile)){
                throw new ArgumentException("Missing .meta file");
            }
            
            string currentTag = null, currentContents = string.Empty;

            foreach(string line in File.ReadAllLines(metaFile, Encoding.UTF8).Concat(EndTag).Select(line => line.TrimEnd()).Where(line => line.Length > 0)){
                if (line[0] == '[' && line[line.Length-1] == ']'){
                    if (currentTag != null){
                        plugin.metadata[currentTag] = currentContents;
                    }

                    currentTag = line.Substring(1, line.Length-2).ToUpper();
                    currentContents = string.Empty;

                    if (line.Equals(EndTag[0])){
                        break;
                    }

                    if (!plugin.metadata.ContainsKey(currentTag)){
                        throw new FormatException("Invalid metadata tag: "+currentTag);
                    }
                }
                else if (currentTag != null){
                    currentContents = currentContents.Length == 0 ? line : currentContents+Environment.NewLine+line;
                }
                else{
                    throw new FormatException("Missing metadata tag before value: "+line);
                }
            }

            if (plugin.Name.Length == 0){
                throw new FormatException("Plugin is missing a name in the .meta file");
            }

            if (plugin.RequiredVersion.Length == 0 || !(plugin.RequiredVersion == VersionWildcard || System.Version.TryParse(plugin.RequiredVersion, out Version _))){
                throw new FormatException("Plugin contains invalid version: "+plugin.RequiredVersion);
            }

            plugin.OnMetadataLoaded();
        }

        private static bool CheckRequiredVersion(string requires){
            return requires == VersionWildcard || AppVersion >= new Version(requires);
        }
    }
}
