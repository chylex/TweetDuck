using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TweetDck.Plugins.Events;

namespace TweetDck.Plugins{
    class PluginBridge{
        private readonly PluginManager manager;
        private readonly Dictionary<string, string> fileCache = new Dictionary<string, string>(2);

        public PluginBridge(PluginManager manager){
            this.manager = manager;
            this.manager.Reloaded += manager_Reloaded;
        }

        private void manager_Reloaded(object sender, PluginLoadEventArgs e){
            fileCache.Clear();
        }

        private string GetFullPathIfSafe(int token, string path){
            Plugin plugin = manager.GetPluginFromToken(token);
            return plugin == null ? string.Empty : plugin.GetFullPathIfSafe(path);
        }

        public void WriteFile(int token, string path, string contents){
            string fullPath = GetFullPathIfSafe(token, path);

            if (fullPath == string.Empty){
                throw new Exception("File path has to be relative to the plugin folder.");
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            File.WriteAllText(fullPath, contents, Encoding.UTF8);
            fileCache[fullPath] = contents;
        }

        public string ReadFile(int token, string path, bool cache){
            string fullPath = GetFullPathIfSafe(token, path);

            if (fullPath.Length == 0){
                throw new Exception("File path has to be relative to the plugin folder.");
            }

            string cachedContents;
            
            if (cache && fileCache.TryGetValue(fullPath, out cachedContents)){
                return cachedContents;
            }

            try{
                return fileCache[fullPath] = File.ReadAllText(fullPath, Encoding.UTF8);
            }catch(FileNotFoundException){
                throw new Exception("File not found.");
            }catch(DirectoryNotFoundException){
                throw new Exception("Directory not found.");
            }
        }

        public void DeleteFile(int token, string path){
            string fullPath = GetFullPathIfSafe(token, path);

            if (fullPath.Length == 0){
                throw new Exception("File path has to be relative to the plugin folder.");
            }
            
            fileCache.Remove(fullPath);
            File.Delete(fullPath);
        }

        public bool CheckFileExists(int token, string path){
            string fullPath = GetFullPathIfSafe(token, path);

            if (fullPath.Length == 0){
                throw new Exception("File path has to be relative to the plugin folder.");
            }

            return File.Exists(fullPath);
        }
    }
}
