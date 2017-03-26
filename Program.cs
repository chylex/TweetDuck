using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using TweetDck.Configuration;
using TweetDck.Core;
using TweetDck.Core.Utils;
using System.Linq;
using System.Threading;
using TweetDck.Plugins;
using TweetDck.Plugins.Events;
using TweetDck.Core.Other.Settings.Export;
using TweetDck.Core.Handling;
using TweetDck.Core.Other;
using TweetDck.Updates;

namespace TweetDck{
    static class Program{
        public const string BrandName = "TweetDuck";
        public const string Website = "https://tweetduck.chylex.com";

        public const string VersionTag = "1.6.7";
        public const string VersionFull = "1.6.7.0";

        public static readonly Version Version = new Version(VersionTag);
        public static readonly bool IsPortable = File.Exists("makeportable");

        public static readonly string ProgramPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string StoragePath = IsPortable ? Path.Combine(ProgramPath, "portable", "storage") : GetDataStoragePath();
        public static readonly string TemporaryPath = IsPortable ? Path.Combine(ProgramPath, "portable", "tmp") : Path.Combine(Path.GetTempPath(), BrandName+'_'+Path.GetRandomFileName().Substring(0, 6));

        public static readonly string PluginDataPath = Path.Combine(StoragePath, "TD_Plugins");
        public static readonly string ConfigFilePath = Path.Combine(StoragePath, "TD_UserConfig.cfg");
        private static readonly string ErrorLogFilePath = Path.Combine(StoragePath, "TD_Log.txt");
        private static readonly string ConsoleLogFilePath = Path.Combine(StoragePath, "TD_Console.txt");
        
        public static readonly string ScriptPath = Path.Combine(ProgramPath, "scripts");
        public static readonly string PluginPath = Path.Combine(ProgramPath, "plugins");

        public static uint WindowRestoreMessage;

        private static readonly LockManager LockManager = new LockManager(Path.Combine(StoragePath, ".lock"));
        private static bool HasCleanedUp;
        
        public static UserConfig UserConfig { get; private set; }
        public static Reporter Reporter { get; private set; }

        public static event EventHandler UserConfigReplaced;

        [STAThread]
        private static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WindowRestoreMessage = NativeMethods.RegisterWindowMessage("TweetDuckRestore");

            if (!WindowsUtils.CheckFolderWritePermission(StoragePath)){
                MessageBox.Show(BrandName+" does not have write permissions to the storage folder: "+StoragePath, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Reporter = new Reporter(ErrorLogFilePath);
            Reporter.SetupUnhandledExceptionHandler(BrandName+" Has Failed :(");

            if (Arguments.HasFlag(Arguments.ArgRestart)){
                for(int attempt = 0; attempt < 21; attempt++){
                    LockManager.Result lockResult = LockManager.Lock();

                    if (lockResult == LockManager.Result.Success){
                        break;
                    }
                    else if (lockResult == LockManager.Result.Fail){
                        MessageBox.Show("An unknown error occurred accessing the data folder. Please, make sure "+BrandName+" is not already running. If the problem persists, try restarting your system.", BrandName+" Has Failed :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else if (attempt == 20){
                        using(FormMessage form = new FormMessage(BrandName+" Cannot Restart", BrandName+" is taking too long to close.", MessageBoxIcon.Warning)){
                            form.AddButton("Exit");
                            form.AddButton("Retry", DialogResult.Retry);

                            if (form.ShowDialog() == DialogResult.Retry){
                                attempt /= 2;
                                continue;
                            }

                            return;
                        }
                    }
                    else Thread.Sleep(500);
                }
            }
            else{
                LockManager.Result lockResult = LockManager.Lock();

                if (lockResult == LockManager.Result.HasProcess){
                    if (LockManager.LockingProcess.MainWindowHandle == IntPtr.Zero){ // restore if the original process is in tray
                        NativeMethods.SendMessage(NativeMethods.HWND_BROADCAST, WindowRestoreMessage, LockManager.LockingProcess.Id, IntPtr.Zero);

                        if (WindowsUtils.TrySleepUntil(() => {
                            LockManager.LockingProcess.Refresh();
                            return LockManager.LockingProcess.HasExited || (LockManager.LockingProcess.MainWindowHandle != IntPtr.Zero && LockManager.LockingProcess.Responding);
                        }, 2000, 250)){
                            return; // should trigger on first attempt if succeeded, but wait just in case
                        }
                    }
                    
                    if (MessageBox.Show("Another instance of "+BrandName+" is already running.\r\nDo you want to close it?", BrandName+" is Already Running", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                        if (!LockManager.CloseLockingProcess(10000, 5000)){
                            MessageBox.Show("Could not close the other process.", BrandName+" Has Failed :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        LockManager.Lock();
                    }
                    else return;
                }
                else if (lockResult != LockManager.Result.Success){
                    MessageBox.Show("An unknown error occurred accessing the data folder. Please, make sure "+BrandName+" is not already running. If the problem persists, try restarting your system.", BrandName+" Has Failed :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            ReloadConfig();

            if (Arguments.HasFlag(Arguments.ArgImportCookies)){
                ExportManager.ImportCookies();
            }
            
            CefSharpSettings.WcfEnabled = false;

            CefSettings settings = new CefSettings{
                AcceptLanguageList = BrowserUtils.HeaderAcceptLanguage,
                UserAgent = BrowserUtils.HeaderUserAgent,
                Locale = Arguments.GetValue(Arguments.ArgLocale, string.Empty),
                CachePath = StoragePath,
                LogFile = ConsoleLogFilePath,
                #if !DEBUG
                BrowserSubprocessPath = BrandName+".Browser.exe",
                LogSeverity = Arguments.HasFlag(Arguments.ArgLogging) ? LogSeverity.Info : LogSeverity.Disable
                #endif
            };

            CommandLineArgsParser.ReadCefArguments(UserConfig.CustomCefArgs).ToDictionary(settings.CefCommandLineArgs);

            if (!HardwareAcceleration.IsEnabled){
                settings.CefCommandLineArgs["disable-gpu"] = "1";
                settings.CefCommandLineArgs["disable-gpu-vsync"] = "1";
            }

            Cef.Initialize(settings, false, new BrowserProcessHandler());

            Application.ApplicationExit += (sender, args) => ExitCleanup();

            PluginManager plugins = new PluginManager(PluginPath, UserConfig.Plugins);
            plugins.Reloaded += plugins_Reloaded;
            plugins.Executed += plugins_Executed;
            plugins.Reload();

            FormBrowser mainForm = new FormBrowser(plugins, new UpdaterSettings{
                AllowPreReleases = Arguments.HasFlag(Arguments.ArgDebugUpdates),
                DismissedUpdate = UserConfig.DismissedUpdate
            });

            Application.Run(mainForm);

            if (mainForm.UpdateInstallerPath != null){
                ExitCleanup();

                // ProgramPath has a trailing backslash
                string updaterArgs = "/SP- /SILENT /CLOSEAPPLICATIONS /UPDATEPATH=\""+ProgramPath+"\" /RUNARGS=\""+Arguments.GetCurrentClean().ToString().Replace("\"", "^\"")+"\""+(IsPortable ? " /PORTABLE=1" : "");
                bool runElevated = !IsPortable || !WindowsUtils.CheckFolderWritePermission(ProgramPath);

                WindowsUtils.StartProcess(mainForm.UpdateInstallerPath, updaterArgs, runElevated);
                Application.Exit();
            }
        }

        private static void plugins_Reloaded(object sender, PluginErrorEventArgs e){
            if (e.HasErrors){
                string doubleNL = Environment.NewLine+Environment.NewLine;
                MessageBox.Show("The following plugins will not be available until the issues are resolved:"+doubleNL+string.Join(doubleNL, e.Errors), "Error Loading Plugins", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void plugins_Executed(object sender, PluginErrorEventArgs e){
            if (e.HasErrors){
                string doubleNL = Environment.NewLine+Environment.NewLine;
                MessageBox.Show("Failed to execute the following plugins:"+doubleNL+string.Join(doubleNL, e.Errors), "Error Executing Plugins", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetDataStoragePath(){
            string custom = Arguments.GetValue(Arguments.ArgDataFolder, null);

            if (custom != null && (custom.Contains(Path.DirectorySeparatorChar) || custom.Contains(Path.AltDirectorySeparatorChar))){
                if (Path.GetInvalidPathChars().Any(custom.Contains)){
                    Reporter.HandleEarlyFailure("Data Folder Invalid", "The data folder contains invalid characters:\n"+custom);
                }
                else if (!Path.IsPathRooted(custom)){
                    Reporter.HandleEarlyFailure("Data Folder Invalid", "The data folder has to be either a simple folder name, or a full path:\n"+custom);
                }

                return Environment.ExpandEnvironmentVariables(custom);
            }
            else{
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), custom ?? BrandName);
            }
        }

        public static void ReloadConfig(){
            UserConfig = UserConfig.Load(ConfigFilePath);

            if (UserConfigReplaced != null){
                UserConfigReplaced(UserConfig, new EventArgs());
            }
        }

        public static void ResetConfig(){
            try{
                File.Delete(ConfigFilePath);
                File.Delete(UserConfig.GetBackupFile(ConfigFilePath));
            }catch(Exception e){
                Reporter.HandleException("Configuration Reset Error", "Could not delete configuration files to reset the settings.", true, e);
                return;
            }

            ReloadConfig();
        }

        public static void Restart(){
            Restart(new string[0]);
        }

        public static void Restart(string[] extraArgs){
            CommandLineArgs args = Arguments.GetCurrentClean();
            CommandLineArgs.ReadStringArray('-', extraArgs, args);
            RestartWithArgs(args);
        }

        public static void RestartWithArgs(CommandLineArgs args){
            FormBrowser browserForm = Application.OpenForms.OfType<FormBrowser>().FirstOrDefault();
            if (browserForm == null)return;
            
            args.AddFlag(Arguments.ArgRestart);

            browserForm.ForceClose();
            ExitCleanup();

            Process.Start(Application.ExecutablePath, args.ToString());
            Application.Exit();
        }

        private static void ExitCleanup(){
            if (HasCleanedUp)return;

            UserConfig.Save();

            try{
                Directory.Delete(TemporaryPath, true);
            }catch(DirectoryNotFoundException){
            }catch(Exception e){
                // welp, too bad
                Debug.WriteLine(e.ToString());
            }

            Cef.Shutdown();
            BrowserCache.Exit();
            
            LockManager.Unlock();
            HasCleanedUp = true;
        }
    }
}
