using CefSharp;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Other;
using TweetDuck.Core.Other.Settings.Export;
using TweetDuck.Core.Utils;
using TweetDuck.Data;
using TweetDuck.Plugins;
using TweetDuck.Plugins.Events;
using TweetDuck.Updates;

namespace TweetDuck{
    static class Program{
        public const string BrandName = "TweetDuck";
        public const string Website = "https://tweetduck.chylex.com";

        public const string VersionTag = "1.8.3";
        public const string VersionFull = "1.8.3.0";

        public static readonly Version Version = new Version(VersionTag);
        public static readonly bool IsPortable = File.Exists("makeportable");

        public static readonly string ProgramPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string StoragePath = IsPortable ? Path.Combine(ProgramPath, "portable", "storage") : GetDataStoragePath();

        public static readonly string ScriptPath = Path.Combine(ProgramPath, "scripts");
        public static readonly string PluginPath = Path.Combine(ProgramPath, "plugins");

        public static readonly string UserConfigFilePath = Path.Combine(StoragePath, "TD_UserConfig.cfg");
        public static readonly string SystemConfigFilePath = Path.Combine(StoragePath, "TD_SystemConfig.cfg");
        public static readonly string PluginConfigFilePath = Path.Combine(StoragePath, "TD_PluginConfig.cfg");

        public static readonly string PluginDataPath = Path.Combine(StoragePath, "TD_Plugins");
        private static readonly string InstallerPath = Path.Combine(StoragePath, "TD_Updates");

        private static string ErrorLogFilePath => Path.Combine(StoragePath, "TD_Log.txt");
        private static string ConsoleLogFilePath => Path.Combine(StoragePath, "TD_Console.txt");

        public static uint WindowRestoreMessage;
        public static uint SubProcessMessage;

        private static readonly LockManager LockManager = new LockManager(Path.Combine(StoragePath, ".lock"));
        private static bool HasCleanedUp;
        
        public static UserConfig UserConfig { get; private set; }
        public static SystemConfig SystemConfig { get; private set; }
        public static Reporter Reporter { get; }
        public static CultureInfo Culture { get; }

        public static event EventHandler UserConfigReplaced;

        static Program(){
            Culture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            Reporter = new Reporter(ErrorLogFilePath);
            Reporter.SetupUnhandledExceptionHandler("TweetDuck Has Failed :(");
        }

        [STAThread]
        private static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            WindowRestoreMessage = NativeMethods.RegisterWindowMessage("TweetDuckRestore");
            SubProcessMessage = NativeMethods.RegisterWindowMessage("TweetDuckSubProcess");

            if (!WindowsUtils.CheckFolderWritePermission(StoragePath)){
                FormMessage.Warning("Permission Error", "TweetDuck does not have write permissions to the storage folder: "+StoragePath, FormMessage.OK);
                return;
            }
            
            if (Arguments.HasFlag(Arguments.ArgRestart)){
                for(int attempt = 0; attempt < 21; attempt++){
                    LockManager.Result lockResult = LockManager.Lock();

                    if (lockResult == LockManager.Result.Success){
                        break;
                    }
                    else if (lockResult == LockManager.Result.Fail){
                        FormMessage.Error("TweetDuck Has Failed :(", "An unknown error occurred accessing the data folder. Please, make sure TweetDuck is not already running. If the problem persists, try restarting your system.", FormMessage.OK);
                        return;
                    }
                    else if (attempt == 20){
                        if (FormMessage.Warning("TweetDuck Cannot Restart", "TweetDuck is taking too long to close.", FormMessage.Retry, FormMessage.Exit)){
                            attempt /= 2;
                            continue;
                        }

                        return;
                    }
                    else Thread.Sleep(500);
                }
            }
            else{
                LockManager.Result lockResult = LockManager.Lock();

                if (lockResult == LockManager.Result.HasProcess){
                    if (LockManager.LockingProcess.MainWindowHandle == IntPtr.Zero){ // restore if the original process is in tray
                        NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, WindowRestoreMessage, new UIntPtr((uint)LockManager.LockingProcess.Id), IntPtr.Zero);

                        if (WindowsUtils.TrySleepUntil(() => {
                            LockManager.LockingProcess.Refresh();
                            return LockManager.LockingProcess.HasExited || (LockManager.LockingProcess.MainWindowHandle != IntPtr.Zero && LockManager.LockingProcess.Responding);
                        }, 2000, 250)){
                            return;
                        }
                    }
                    
                    if (FormMessage.Error("TweetDuck is Already Running", "Another instance of TweetDuck is already running.\nDo you want to close it?", FormMessage.Yes, FormMessage.No)){
                        if (!LockManager.CloseLockingProcess(10000, 5000)){
                            FormMessage.Error("TweetDuck Has Failed :(", "Could not close the other process.", FormMessage.OK);
                            return;
                        }

                        LockManager.Lock();
                    }
                    else return;
                }
                else if (lockResult != LockManager.Result.Success){
                    FormMessage.Error("TweetDuck Has Failed :(", "An unknown error occurred accessing the data folder. Please, make sure TweetDuck is not already running. If the problem persists, try restarting your system.", FormMessage.OK);
                    return;
                }
            }

            ReloadConfig();
            SystemConfig = SystemConfig.Load(SystemConfigFilePath);

            if (Arguments.HasFlag(Arguments.ArgImportCookies)){
                ExportManager.ImportCookies();
            }

            if (Arguments.HasFlag(Arguments.ArgUpdated)){
                WindowsUtils.TryDeleteFolderWhenAble(InstallerPath, 8000);
            }
            
            CefSharpSettings.WcfEnabled = false;

            CefSettings settings = new CefSettings{
                AcceptLanguageList = BrowserUtils.HeaderAcceptLanguage,
                UserAgent = BrowserUtils.HeaderUserAgent,
                Locale = Arguments.GetValue(Arguments.ArgLocale, string.Empty),
                BrowserSubprocessPath = BrandName+".Browser.exe",
                CachePath = StoragePath,
                LogFile = ConsoleLogFilePath,
                #if !DEBUG
                LogSeverity = Arguments.HasFlag(Arguments.ArgLogging) ? LogSeverity.Info : LogSeverity.Disable
                #endif
            };

            CommandLineArgsParser.ReadCefArguments(UserConfig.CustomCefArgs).ToDictionary(settings.CefCommandLineArgs);
            BrowserUtils.SetupCefArgs(settings.CefCommandLineArgs);
            
            Cef.EnableHighDPISupport();
            Cef.Initialize(settings, false, new BrowserProcessHandler());

            Application.ApplicationExit += (sender, args) => ExitCleanup();

            PluginManager plugins = new PluginManager(PluginPath, PluginConfigFilePath);
            plugins.Reloaded += plugins_Reloaded;
            plugins.Executed += plugins_Executed;
            plugins.Reload();

            UpdaterSettings updaterSettings = new UpdaterSettings{
                AllowPreReleases = Arguments.HasFlag(Arguments.ArgDebugUpdates),
                DismissedUpdate = UserConfig.DismissedUpdate,
                InstallerDownloadFolder = InstallerPath
            };

            FormBrowser mainForm = new FormBrowser(plugins, updaterSettings);
            Application.Run(mainForm);

            if (mainForm.UpdateInstallerPath != null){
                ExitCleanup();

                // ProgramPath has a trailing backslash
                string updaterArgs = "/SP- /SILENT /CLOSEAPPLICATIONS /UPDATEPATH=\""+ProgramPath+"\" /RUNARGS=\""+Arguments.GetCurrentForInstallerCmd()+"\""+(IsPortable ? " /PORTABLE=1" : "");
                bool runElevated = !IsPortable || !WindowsUtils.CheckFolderWritePermission(ProgramPath);

                WindowsUtils.StartProcess(mainForm.UpdateInstallerPath, updaterArgs, runElevated);
                Application.Exit();
            }
        }

        private static void plugins_Reloaded(object sender, PluginErrorEventArgs e){
            if (e.HasErrors){
                FormMessage.Error("Error Loading Plugins", "The following plugins will not be available until the issues are resolved:\n\n"+string.Join("\n\n", e.Errors), FormMessage.OK);
            }
        }

        private static void plugins_Executed(object sender, PluginErrorEventArgs e){
            if (e.HasErrors){
                FormMessage.Error("Error Executing Plugins", "Failed to execute the following plugins:\n\n"+string.Join("\n\n", e.Errors), FormMessage.OK);
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
            UserConfig = UserConfig.Load(UserConfigFilePath);
            UserConfigReplaced?.Invoke(UserConfig, new EventArgs());
        }

        public static void ResetConfig(){
            try{
                File.Delete(UserConfigFilePath);
                File.Delete(UserConfig.GetBackupFile(UserConfigFilePath));
            }catch(Exception e){
                Reporter.HandleException("Configuration Reset Error", "Could not delete configuration files to reset the options.", true, e);
                return;
            }

            ReloadConfig();
        }

        public static void Restart(params string[] extraArgs){
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

            Cef.Shutdown();
            BrowserCache.Exit();
            
            LockManager.Unlock();
            HasCleanedUp = true;
        }
    }
}
