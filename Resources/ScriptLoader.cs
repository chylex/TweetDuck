using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other;
using TweetLib.Core.Application;

#if DEBUG
using System.Diagnostics;
using System.Reflection;
using TweetDuck.Core;
using TweetLib.Core.Features.Plugins;
#endif

namespace TweetDuck.Resources{
    sealed class ScriptLoader : IAppResourceHandler{
        private static readonly Dictionary<string, string> CachedData = new Dictionary<string, string>(16);

        public static void ClearCache(){
            CachedData.Clear();
        }

        private Control sync;

        public void Initialize(Control sync){
            this.sync = sync;
        }

        public string Load(string path){
            return LoadInternal(path, silent: false);
        }

        public string LoadSilent(string path){
            return LoadInternal(path, silent: true);
        }

        private string LoadInternal(string name, bool silent){
            if (sync == null){
                throw new InvalidOperationException("Cannot use ScriptLoader before initialization.");
            }
            else if (sync.IsDisposed){
                return null; // better than crashing I guess...?
            }

            if (CachedData.TryGetValue(name, out string resourceData)){
                return resourceData;
            }

            string path = Program.ScriptPath;

            #if DEBUG
            if (Directory.Exists(HotSwapTargetDir)){
                path = Path.Combine(HotSwapTargetDir, "scripts");
                Debug.WriteLine($"Hot swap active, redirecting {name}");
            }
            #endif

            string resource;

            try{
                string contents = File.ReadAllText(Path.Combine(path, name), Encoding.UTF8);
                int separator;

                // first line can be either:
                // #<version>\r\n
                // #<version>\n

                if (contents[0] != '#'){
                    ShowLoadError(silent ? null : sync, $"File {name} appears to be corrupted, please try reinstalling the app.");
                    separator = 0;
                }
                else{
                    separator = contents.IndexOf('\n');
                    string fileVersion = contents.Substring(1, separator-1).TrimEnd();

                    if (fileVersion != Program.VersionTag){
                        ShowLoadError(silent ? null : sync, $"File {name} is made for a different version of TweetDuck ({fileVersion}) and may not function correctly in this version, please try reinstalling the app.");
                    }
                }

                resource = contents.Substring(separator).TrimStart();
            }catch(Exception ex){
                ShowLoadError(silent ? null : sync, $"Could not load {name}. The program will continue running with limited functionality.\n\n{ex.Message}");
                resource = null;
            }

            return CachedData[name] = resource;
        }

        private static void ShowLoadError(Control sync, string message){
            sync?.InvokeSafe(() => FormMessage.Error("Resource Error", message, FormMessage.OK));
        }
        
        // TODO move hot swap to another implementation

        #if DEBUG
        private static readonly string HotSwapProjectRoot = FixPathSlash(Path.GetFullPath(Path.Combine(Program.ProgramPath, "../../../")));
        private static readonly string HotSwapTargetDir = FixPathSlash(Path.Combine(HotSwapProjectRoot, "bin", "tmp"));
        private static readonly string HotSwapRebuildScript = Path.Combine(HotSwapProjectRoot, "bld", "post_build.exe");

        static ScriptLoader(){
            if (File.Exists(HotSwapRebuildScript)){
                Debug.WriteLine("Activating resource hot swap...");

                ResetHotSwap();
                Application.ApplicationExit += (sender, args) => ResetHotSwap();
            }
        }

        public static void HotSwap(){
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

        private static void ResetHotSwap(){
            try{
                Directory.Delete(HotSwapTargetDir, true);
            }catch(DirectoryNotFoundException){}
        }

        private static string FixPathSlash(string path){
            return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + '\\';
        }
        #endif
    }
}
