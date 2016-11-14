using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TweetDck.Plugins.Enums;
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

        private string ReadFileUnsafe(string fullPath, bool readCached){
            string cachedContents;
            
            if (readCached && fileCache.TryGetValue(fullPath, out cachedContents)){
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

        // Public methods

        public void WriteFile(int token, string path, string contents){
            string fullPath = GetFullPathOrThrow(token, PluginFolder.Data, path);

            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            File.WriteAllText(fullPath, contents, Encoding.UTF8);
            fileCache[fullPath] = contents;
        }

        public string ReadFile(int token, string path, bool cache){
            return ReadFileUnsafe(GetFullPathOrThrow(token, PluginFolder.Data, path), cache);
        }

        public void DeleteFile(int token, string path){
            string fullPath = GetFullPathOrThrow(token, PluginFolder.Data, path);

            fileCache.Remove(fullPath);
            File.Delete(fullPath);
        }

        public bool CheckFileExists(int token, string path){
            return File.Exists(GetFullPathOrThrow(token, PluginFolder.Data, path));
        }

        public string ReadFileRoot(int token, string path){
            return ReadFileUnsafe(GetFullPathOrThrow(token, PluginFolder.Root, path), true);
        }

        public bool CheckFileExistsRoot(int token, string path){
            return File.Exists(GetFullPathOrThrow(token, PluginFolder.Root, path));
        }
    }
}
