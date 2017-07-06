using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TweetDuck.Data;
using TweetDuck.Plugins.Enums;
using TweetDuck.Plugins.Events;

namespace TweetDuck.Plugins{
    class PluginBridge{
        private static string SanitizeCacheKey(string key){
            return key.Replace('\\', '/').Trim();
        }

        private readonly PluginManager manager;
        private readonly TwoKeyDictionary<int, string, string> fileCache = new TwoKeyDictionary<int, string, string>(4, 2);
        private readonly TwoKeyDictionary<int, string, InjectedHTML> notificationInjections = new TwoKeyDictionary<int,string,InjectedHTML>(4, 1);

        public IEnumerable<InjectedHTML> NotificationInjections => notificationInjections.InnerValues;

        public PluginBridge(PluginManager manager){
            this.manager = manager;
            this.manager.Reloaded += manager_Reloaded;
            this.manager.PluginChangedState += manager_PluginChangedState;
        }

        // Event handlers

        private void manager_Reloaded(object sender, PluginErrorEventArgs e){
            fileCache.Clear();
        }

        private void manager_PluginChangedState(object sender, PluginChangedStateEventArgs e){
            if (!e.IsEnabled){
                int token = manager.GetTokenFromPlugin(e.Plugin);

                fileCache.Remove(token);
                notificationInjections.Remove(token);
            }
        }

        // Utility methods

        private string GetFullPathOrThrow(int token, PluginFolder folder, string path){
            Plugin plugin = manager.GetPluginFromToken(token);
            string fullPath = plugin == null ? string.Empty : plugin.GetFullPathIfSafe(folder, path);

            if (fullPath.Length == 0){
                switch(folder){
                    case PluginFolder.Data: throw new Exception("File path has to be relative to the plugin data folder.");
                    case PluginFolder.Root: throw new Exception("File path has to be relative to the plugin root folder.");
                    default: throw new Exception("Invalid folder type "+folder+", this is a "+Program.BrandName+" error.");
                }
            }
            else{
                return fullPath;
            }
        }

        private string ReadFileUnsafe(int token, string cacheKey, string fullPath, bool readCached){
            cacheKey = SanitizeCacheKey(cacheKey);
            
            if (readCached && fileCache.TryGetValue(token, cacheKey, out string cachedContents)){
                return cachedContents;
            }

            try{
                return fileCache[token, cacheKey] = File.ReadAllText(fullPath, Encoding.UTF8);
            }catch(FileNotFoundException){
                throw new Exception("File not found.");
            }catch(DirectoryNotFoundException){
                throw new Exception("Directory not found.");
            }
        }

        // Public methods

        public void WriteFile(int token, string path, string contents){
            string fullPath = GetFullPathOrThrow(token, PluginFolder.Data, path);

            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            File.WriteAllText(fullPath, contents, Encoding.UTF8);
            fileCache[token, SanitizeCacheKey(path)] = contents;
        }

        public string ReadFile(int token, string path, bool cache){
            return ReadFileUnsafe(token, path, GetFullPathOrThrow(token, PluginFolder.Data, path), cache);
        }

        public void DeleteFile(int token, string path){
            string fullPath = GetFullPathOrThrow(token, PluginFolder.Data, path);

            fileCache.Remove(token, SanitizeCacheKey(path));
            File.Delete(fullPath);
        }

        public bool CheckFileExists(int token, string path){
            return File.Exists(GetFullPathOrThrow(token, PluginFolder.Data, path));
        }

        public string ReadFileRoot(int token, string path){
            return ReadFileUnsafe(token, "root*"+path, GetFullPathOrThrow(token, PluginFolder.Root, path), true);
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
    }
}
