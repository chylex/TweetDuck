#if DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TweetDuck.Browser;
using TweetDuck.Management;
using TweetLib.Core.Features.Plugins;

namespace TweetDuck.Resources{
    sealed class ScriptLoaderDebug : ScriptLoader{
        private static readonly string HotSwapProjectRoot = FixPathSlash(Path.GetFullPath(Path.Combine(Program.ProgramPath, "../../../")));
        private static readonly string HotSwapTargetDir = FixPathSlash(Path.Combine(HotSwapProjectRoot, "bin", "tmp"));
        private static readonly string HotSwapRebuildScript = Path.Combine(HotSwapProjectRoot, "bld", "post_build.exe");
        
        private static string FixPathSlash(string path){
            return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + '\\';
        }

        public ScriptLoaderDebug(){
            if (File.Exists(HotSwapRebuildScript)){
                Debug.WriteLine("Activating resource hot swap...");

                ResetHotSwap();
                System.Windows.Forms.Application.ApplicationExit += (sender, args) => ResetHotSwap();
            }
        }

        public override void OnReloadTriggered(){
            HotSwap();
        }

        protected override string LocateFile(string path){
            if (Directory.Exists(HotSwapTargetDir)){
                Debug.WriteLine($"Hot swap active, redirecting {path}");
                return Path.Combine(HotSwapTargetDir, "scripts", path);
            }

            return base.LocateFile(path);
        }

        private void HotSwap(){
            if (!File.Exists(HotSwapRebuildScript)){
                Debug.WriteLine($"Failed resource hot swap, missing rebuild script: {HotSwapRebuildScript}");
                return;
            }
            
            ResetHotSwap();
            Directory.CreateDirectory(HotSwapTargetDir);

            Stopwatch sw = Stopwatch.StartNew();

            using(Process process = Process.Start(new ProcessStartInfo{
                FileName = HotSwapRebuildScript,
                Arguments = $"\"{HotSwapTargetDir}\\\" \"{HotSwapProjectRoot}\\\" \"Debug\" \"{Program.VersionTag}\"",
                WindowStyle = ProcessWindowStyle.Hidden
            })){
                // ReSharper disable once PossibleNullReferenceException
                if (!process.WaitForExit(8000)){
                    Debug.WriteLine("Failed resource hot swap, script did not finish in time");
                    return;
                }
                else if (process.ExitCode != 0){
                    Debug.WriteLine($"Failed resource hot swap, script exited with code {process.ExitCode}");
                    return;
                }
            }

            sw.Stop();
            Debug.WriteLine($"Finished rebuild script in {sw.ElapsedMilliseconds} ms");

            ClearCache();

            // Force update plugin manager setup scripts

            string newPluginRoot = Path.Combine(HotSwapTargetDir, "plugins");
            
            const BindingFlags flagsInstance = BindingFlags.Instance | BindingFlags.NonPublic;

            Type typePluginManager = typeof(PluginManager);
            Type typeFormBrowser = typeof(FormBrowser);

            // ReSharper disable PossibleNullReferenceException
            object instPluginManager = typeFormBrowser.GetField("plugins", flagsInstance).GetValue(FormManager.TryFind<FormBrowser>());
            typePluginManager.GetField("pluginFolder", flagsInstance).SetValue(instPluginManager, newPluginRoot);

            Debug.WriteLine("Reloading hot swapped plugins...");
            ((PluginManager)instPluginManager).Reload();
            // ReSharper restore PossibleNullReferenceException
        }

        private void ResetHotSwap(){
            try{
                Directory.Delete(HotSwapTargetDir, true);
            }catch(DirectoryNotFoundException){}
        }
    }
}

#endif
