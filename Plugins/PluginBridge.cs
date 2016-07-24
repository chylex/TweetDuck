using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TweetDck.Plugins{
    class PluginBridge{
        private readonly PluginManager manager;
        private readonly Dictionary<string,string> fileCache = new Dictionary<string,string>(2);

        public PluginBridge(PluginManager manager){
            this.manager = manager;
        }

        private string GetFullPathIfSafe(int token, string path){
            Plugin plugin = manager.GetPluginFromToken(token);
            
            if (plugin == null){
                return string.Empty;
            }

            string fullPath = Path.Combine(plugin.FolderPath,path);

            try{
                string folderPathName = new DirectoryInfo(plugin.FolderPath).FullName;
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

        public void WriteFile(int token, string path, string contents){
            string fullPath = GetFullPathIfSafe(token,path);

            if (fullPath == string.Empty){
                throw new Exception("File path has to be relative to the plugin folder.");
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            File.WriteAllText(fullPath,contents,Encoding.UTF8);
            fileCache[fullPath] = contents;
        }

        public string ReadFile(int token, string path){
            return ReadFile(token,path,true);
        }

        public string ReadFile(int token, string path, bool cache){
            string fullPath = GetFullPathIfSafe(token,path);

            if (fullPath == string.Empty){
                throw new Exception("File path has to be relative to the plugin folder.");
            }

            string cachedContents;
            
            if (fileCache.TryGetValue(fullPath,out cachedContents)){
                return cachedContents;
            }

            return fileCache[fullPath] = File.ReadAllText(fullPath,Encoding.UTF8);
        }

        public void DeleteFile(int token, string path){
            string fullPath = GetFullPathIfSafe(token,path);

            if (fullPath == string.Empty){
                throw new Exception("File path has to be relative to the plugin folder.");
            }
            
            fileCache.Remove(fullPath);
            File.Delete(fullPath);
        }
    }
}
