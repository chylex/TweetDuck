using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace TweetDuck.Core.Utils{
    static class BrowserCache{
        private static bool ClearOnExit { get; set; }

        public static readonly string CacheFolder = Path.Combine(Program.StoragePath, "Cache");
        private static IEnumerable<FileInfo> CacheFiles => new DirectoryInfo(CacheFolder).EnumerateFiles();

        public static void CalculateCacheSize(Action<Task<long>> callbackBytes){
            Task<long> task = new Task<long>(() => {
                return CacheFiles.Select(file => {
                    try{
                        return file.Length;
                    }catch{
                        return 0L;
                    }
                }).Sum();
            });
            
            task.ContinueWith(callbackBytes);
            task.Start();
        }

        public static void SetClearOnExit(){
            ClearOnExit = true;
        }

        public static void Exit(){
            if (ClearOnExit){
                try{
                    Directory.Delete(CacheFolder, true);
                }catch{
                    // welp, too bad
                }
            }
        }
    }
}
