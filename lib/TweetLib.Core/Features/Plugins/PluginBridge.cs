using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using TweetLib.Core.Collections;
using TweetLib.Core.Data;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Features.Plugins.Events;
using TweetLib.Core.Utils;

namespace TweetLib.Core.Features.Plugins{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal sealed class PluginBridge{
        private readonly Dictionary<int, Plugin> tokens = new Dictionary<int, Plugin>();
        private readonly Random rand = new Random();

        private readonly FileCache fileCache = new FileCache();
        private readonly TwoKeyDictionary<int, string, InjectedHTML> notificationInjections = new TwoKeyDictionary<int, string, InjectedHTML>(4, 1);

        internal IEnumerable<InjectedHTML> NotificationInjections => notificationInjections.InnerValues;
        internal ISet<Plugin> WithConfigureFunction { get; } = new HashSet<Plugin>();

        public PluginBridge(PluginManager manager){
            manager.Reloaded += manager_Reloaded;
            manager.Config.PluginChangedState += Config_PluginChangedState;
        }

        internal int GetTokenFromPlugin(Plugin plugin){
            foreach(KeyValuePair<int, Plugin> kvp in tokens){
                if (kvp.Value.Equals(plugin)){
                    return kvp.Key;
                }
            }

            int token, attempts = 1000;

            do{
                token = rand.Next();
            }while(tokens.ContainsKey(token) && --attempts >= 0);
            
            if (attempts < 0){
                token = -tokens.Count - 1;
            }

            tokens[token] = plugin;
            return token;
        }

        internal Plugin? GetPluginFromToken(int token){
            return tokens.TryGetValue(token, out Plugin plugin) ? plugin : null;
        }

        // Event handlers

        private void manager_Reloaded(object sender, PluginErrorEventArgs e){
            tokens.Clear();
            fileCache.Clear();
        }

        private void Config_PluginChangedState(object sender, PluginChangedStateEventArgs e){
            if (!e.IsEnabled){
                int token = GetTokenFromPlugin(e.Plugin);

                fileCache.Remove(token);
                notificationInjections.Remove(token);
            }
        }

        // Utility methods

        private string GetFullPathOrThrow(int token, PluginFolder folder, string path){
            Plugin? plugin = GetPluginFromToken(token);
            string fullPath = plugin == null ? string.Empty : plugin.GetFullPathIfSafe(folder, path);

            if (fullPath.Length == 0){
                throw folder switch{
                    PluginFolder.Data => new ArgumentException("File path has to be relative to the plugin data folder."),
                    PluginFolder.Root => new ArgumentException("File path has to be relative to the plugin root folder."),
                    _ => new ArgumentException($"Invalid folder type {folder}, this is a TweetDuck error.")
                };
            }
            else{
                return fullPath;
            }
        }

        private string ReadFileUnsafe(int token, PluginFolder folder, string path, bool readCached){
            string fullPath = GetFullPathOrThrow(token, folder, path);

            if (readCached && fileCache.TryGetValue(token, folder, path, out string cachedContents)){
                return cachedContents;
            }

            try{
                return fileCache[token, folder, path] = File.ReadAllText(fullPath, Encoding.UTF8);
            }catch(FileNotFoundException){
                throw new FileNotFoundException("File not found.");
            }catch(DirectoryNotFoundException){
                throw new DirectoryNotFoundException("Directory not found.");
            }
        }

        // Public methods

        public void WriteFile(int token, string path, string contents){
            string fullPath = GetFullPathOrThrow(token, PluginFolder.Data, path);

            FileUtils.CreateDirectoryForFile(fullPath);
            File.WriteAllText(fullPath, contents, Encoding.UTF8);
            fileCache[token, PluginFolder.Data, path] = contents;
        }

        public string ReadFile(int token, string path, bool cache){
            return ReadFileUnsafe(token, PluginFolder.Data, path, cache);
        }

        public void DeleteFile(int token, string path){
            string fullPath = GetFullPathOrThrow(token, PluginFolder.Data, path);

            fileCache.Remove(token, PluginFolder.Data, path);
            File.Delete(fullPath);
        }

        public bool CheckFileExists(int token, string path){
            return File.Exists(GetFullPathOrThrow(token, PluginFolder.Data, path));
        }

        public string ReadFileRoot(int token, string path){
            return ReadFileUnsafe(token, PluginFolder.Root, path, true);
        }

        public bool CheckFileExistsRoot(int token, string path){
            return File.Exists(GetFullPathOrThrow(token, PluginFolder.Root, path));
        }

        public void InjectIntoNotificationsBefore(int token, string key, string search, string html){
            notificationInjections[token, key] = new InjectedHTML(InjectedHTML.Position.Before, search, html);
        }

        public void InjectIntoNotificationsAfter(int token, string key, string search, string html){
            notificationInjections[token, key] = new InjectedHTML(InjectedHTML.Position.After, search, html);
        }

        public void SetConfigurable(int token){
            Plugin? plugin = GetPluginFromToken(token);

            if (plugin != null){
                WithConfigureFunction.Add(plugin);
            }
        }

        private sealed class FileCache{
            private readonly TwoKeyDictionary<int, string, string> cache = new TwoKeyDictionary<int, string, string>(4, 2);

            public string this[int token, PluginFolder folder, string path]{
                set => cache[token, Key(folder, path)] = value;
            }

            public void Clear(){
                cache.Clear();
            }

            public bool TryGetValue(int token, PluginFolder folder, string path, out string contents){
                return cache.TryGetValue(token, Key(folder, path), out contents);
            }

            public void Remove(int token){
                cache.Remove(token);
            }

            public void Remove(int token, PluginFolder folder, string path){
                cache.Remove(token, Key(folder, path));
            }

            private static string Key(PluginFolder folder, string path){
                string prefix = folder switch{
                    PluginFolder.Root => "root/",
                    PluginFolder.Data => "data/",
                    _ => throw new InvalidOperationException($"Invalid folder type {folder}, this is a TweetDuck error.")
                };

                return prefix + path.Replace('\\', '/').Trim();
            }
        }
    }
}
