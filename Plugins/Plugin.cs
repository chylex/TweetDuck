using System;
using System.IO;
using TweetDuck.Plugins.Enums;

namespace TweetDuck.Plugins{
    sealed class Plugin{
        private const string VersionWildcard = "*";
        private static readonly Version AppVersion = new Version(Program.VersionTag);

        public string Identifier { get; }
        public PluginGroup Group { get; }
        public PluginEnvironment Environments { get; }

        public string Name { get; }
        public string Description { get; }
        public string Author { get; }
        public string Version { get; }
        public string Website { get; }
        public string ConfigFile { get; }
        public string ConfigDefault { get; }
        public Version RequiredVersion { get; }

        public bool CanRun { get; }

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

        private Plugin(PluginGroup group, string identifier, string pathRoot, string pathData, Builder builder){
            this.pathRoot = pathRoot;
            this.pathData = pathData;

            this.Group = group;
            this.Identifier = identifier;
            this.Environments = builder.Environments;

            this.Name = builder.Name;
            this.Description = builder.Description;
            this.Author = builder.Author;
            this.Version = builder.Version;
            this.Website = builder.Website;
            this.ConfigFile = builder.ConfigFile;
            this.ConfigDefault = builder.ConfigDefault;
            this.RequiredVersion = builder.RequiredVersion;

            this.CanRun = AppVersion >= RequiredVersion;
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

        // Builder

        public sealed class Builder{
            private static readonly Version DefaultRequiredVersion = new Version(0, 0, 0, 0);

            public string Name             { get; private set; }
            public string Description      { get; private set; } = string.Empty;
            public string Author           { get; private set; } = "(anonymous)";
            public string Version          { get; private set; } = "(unknown)";
            public string Website          { get; private set; } = string.Empty;
            public string ConfigFile       { get; private set; } = string.Empty;
            public string ConfigDefault    { get; private set; } = string.Empty;
            public Version RequiredVersion { get; private set; } = DefaultRequiredVersion;

            public PluginEnvironment Environments { get; private set; } = PluginEnvironment.None;

            private readonly PluginGroup group;
            private readonly string pathRoot;
            private readonly string pathData;
            private readonly string identifier;

            public Builder(PluginGroup group, string name, string pathRoot, string pathData){
                this.group = group;
                this.pathRoot = pathRoot;
                this.pathData = pathData;
                this.identifier = group.GetIdentifierPrefix()+name;
            }

            public void SetFromTag(string tag, string value){
                switch(tag){
                    case "NAME":          this.Name = value; break;
                    case "DESCRIPTION":   this.Description = value; break;
                    case "AUTHOR":        this.Author = value; break;
                    case "VERSION":       this.Version = value; break;
                    case "WEBSITE":       this.Website = value; break;
                    case "CONFIGFILE":    this.ConfigFile = value; break;
                    case "CONFIGDEFAULT": this.ConfigDefault = value; break;
                    case "REQUIRES":      SetRequiredVersion(value); break;
                    default: throw new FormatException("Invalid metadata tag: "+tag);
                }
            }

            public void AddEnvironment(PluginEnvironment environment){
                this.Environments |= environment;
            }

            private void SetRequiredVersion(string versionStr){
                if (System.Version.TryParse(versionStr, out Version version)){
                    this.RequiredVersion = version;
                }
                else if (versionStr == VersionWildcard){
                    this.RequiredVersion = DefaultRequiredVersion;
                }
                else{
                    throw new FormatException("Plugin contains invalid minimum version: "+versionStr);
                }
            }

            public Plugin BuildAndSetup(){
                Plugin plugin = new Plugin(group, identifier, pathRoot, pathData, this);

                if (plugin.Name.Length == 0){
                    throw new InvalidOperationException("Plugin is missing a name in the .meta file");
                }

                if (plugin.Environments == PluginEnvironment.None){
                    throw new InvalidOperationException("Plugin has no script files");
                }

                // setup

                string configPath = plugin.ConfigPath, defaultConfigPath = plugin.DefaultConfigPath;

                if (configPath.Length > 0 && defaultConfigPath.Length > 0 && !File.Exists(configPath) && File.Exists(defaultConfigPath)){
                    string dataFolder = plugin.GetPluginFolder(PluginFolder.Data);

                    try{
                        Directory.CreateDirectory(dataFolder);
                        File.Copy(defaultConfigPath, configPath, false);
                    }catch(Exception e){
                        throw new IOException($"Could not generate a configuration file for '{plugin.Identifier}' plugin: {e.Message}", e);
                    }
                }

                // done

                return plugin;
            }
        }
    }
}
