using System;
using System.IO;
using TweetLib.Core.Features.Plugins.Enums;

namespace TweetLib.Core.Features.Plugins{
    public sealed class Plugin{
        private static readonly Version AppVersion = new Version(Lib.VersionTag);

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
                string? file = environment.GetPluginScriptFile();
                return file != null ? Path.Combine(pathRoot, file) : string.Empty;
            }
            else{
                return string.Empty;
            }
        }

        public string GetPluginFolder(PluginFolder folder){
            return folder switch{
                PluginFolder.Root => pathRoot,
                PluginFolder.Data => pathData,
                _                 => string.Empty
            };
        }

        public string GetFullPathIfSafe(PluginFolder folder, string relativePath){
            string rootFolder = GetPluginFolder(folder);
            string fullPath = Path.Combine(rootFolder, relativePath);

            try{
                string folderPathName = new DirectoryInfo(rootFolder).FullName;
                DirectoryInfo currentInfo = new DirectoryInfo(fullPath); // initially points to the file, which is convenient for the Attributes check below
                DirectoryInfo parentInfo = currentInfo.Parent;

                while(parentInfo != null){
                    if (currentInfo.Exists && currentInfo.Attributes.HasFlag(FileAttributes.ReparsePoint)){
                        return string.Empty; // no reason why a plugin should have files/folders with symlinks, junctions, or any other crap
                    }

                    if (parentInfo.FullName == folderPathName){
                        return fullPath;
                    }

                    currentInfo = parentInfo;
                    parentInfo = currentInfo.Parent;
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

            public string Name             { get; set; } = string.Empty;
            public string Description      { get; set; } = string.Empty;
            public string Author           { get; set; } = "(anonymous)";
            public string Version          { get; set; } = string.Empty;
            public string Website          { get; set; } = string.Empty;
            public string ConfigFile       { get; set; } = string.Empty;
            public string ConfigDefault    { get; set; } = string.Empty;
            public Version RequiredVersion { get; set; } = DefaultRequiredVersion;

            public PluginEnvironment Environments { get; private set; } = PluginEnvironment.None;

            private readonly PluginGroup group;
            private readonly string pathRoot;
            private readonly string pathData;
            private readonly string identifier;

            public Builder(PluginGroup group, string name, string pathRoot, string pathData){
                this.group = group;
                this.pathRoot = pathRoot;
                this.pathData = pathData;
                this.identifier = group.GetIdentifierPrefix() + name;
            }

            public void AddEnvironment(PluginEnvironment environment){
                this.Environments |= environment;
            }

            public Plugin BuildAndSetup(){
                Plugin plugin = new Plugin(group, identifier, pathRoot, pathData, this);

                if (plugin.Name.Length == 0){
                    throw new InvalidOperationException("Plugin is missing a name in the .meta file");
                }

                if (plugin.Environments == PluginEnvironment.None){
                    throw new InvalidOperationException("Plugin has no script files");
                }

                if (plugin.Group == PluginGroup.Official){
                    if (plugin.RequiredVersion != AppVersion){
                        throw new InvalidOperationException("Plugin is not supported in this version of TweetDuck, this may indicate a failed update or an unsupported plugin that was not removed automatically");
                    }
                    else if (!string.IsNullOrEmpty(plugin.Version)){
                        throw new InvalidOperationException("Official plugins cannot have a version identifier");
                    }
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
