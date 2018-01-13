using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace TweetDuck.Core.Utils{
    static class BrowserCache{
        public static string CacheFolder => Path.Combine(Program.StoragePath, "Cache");
        
        private static bool ClearOnExit;
        private static Timer AutoClearTimer;

        private static long CalculateCacheSize(){
            return new DirectoryInfo(CacheFolder).EnumerateFiles().Select(file => {
                try{
                    return file.Length;
                }catch{
                    return 0L;
                }
            }).Sum();
        }

        public static void GetCacheSize(Action<Task<long>> callbackBytes){
            Task<long> task = new Task<long>(CalculateCacheSize);
            task.ContinueWith(callbackBytes);
            task.Start();
        }
        
        public static void RefreshTimer(){
            bool shouldRun = Program.SystemConfig.ClearCacheAutomatically && !ClearOnExit;

            if (!shouldRun && AutoClearTimer != null){
                AutoClearTimer.Dispose();
                AutoClearTimer = null;
            }
            else if (shouldRun && AutoClearTimer == null){
                AutoClearTimer = new Timer(state => {
                    if (AutoClearTimer != null){
                        try{
                            if (CalculateCacheSize() >= Program.SystemConfig.ClearCacheThreshold*1024L*1024L){
                                SetClearOnExit();
                            }
                        }catch(Exception){
                            // TODO should probably log errors and report them at some point
                        }
                    }
                }, null, TimeSpan.FromSeconds(30), TimeSpan.FromHours(4));
            }
        }

        public static void SetClearOnExit(){
            ClearOnExit = true;
            RefreshTimer();
        }

        public static void Exit(){
            if (AutoClearTimer != null){
                AutoClearTimer.Dispose();
                AutoClearTimer = null;
            }

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
