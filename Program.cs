using CefSharp;
using CefSharp.WinForms;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core;
using TweetDuck.Core.Handling;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Other;
using TweetDuck.Core.Management;
using TweetDuck.Core.Utils;
using TweetLib.Core;
using TweetLib.Core.Collections;
using TweetLib.Core.Utils;

namespace TweetDuck{
    static class Program{
        public const string BrandName = Lib.BrandName;
        public const string VersionTag = Lib.VersionTag;

        public const string Website = "https://tweetduck.chylex.com";

        public static readonly string ProgramPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly bool IsPortable = File.Exists(Path.Combine(ProgramPath, "makeportable"));

        public static readonly string ScriptPath = Path.Combine(ProgramPath, "scripts");
        public static readonly string PluginPath = Path.Combine(ProgramPath, "plugins");

        public static readonly string StoragePath = IsPortable ? Path.Combine(ProgramPath, "portable", "storage") : GetDataStoragePath();

        public static readonly string PluginDataPath = Path.Combine(StoragePath, "TD_Plugins");
        public static readonly string InstallerPath = Path.Combine(StoragePath, "TD_Updates");
        private static readonly string CefDataPath = Path.Combine(StoragePath, "TD_Chromium");

        public static string UserConfigFilePath => Path.Combine(StoragePath, "TD_UserConfig.cfg");
        public static string SystemConfigFilePath => Path.Combine(StoragePath, "TD_SystemConfig.cfg");
        public static string PluginConfigFilePath => Path.Combine(StoragePath, "TD_PluginConfig.cfg");
        public static string AnalyticsFilePath => Path.Combine(StoragePath, "TD_Analytics.cfg");

        private static string ErrorLogFilePath => Path.Combine(StoragePath, "TD_Log.txt");
        private static string ConsoleLogFilePath => Path.Combine(StoragePath, "TD_Console.txt");

        public static uint WindowRestoreMessage;

        private static readonly LockManager LockManager = new LockManager(Path.Combine(StoragePath, ".lock"));
        private static bool HasCleanedUp;
        
        public static Reporter Reporter { get; }
        public static ConfigManager Config { get; }

        static Program(){
            Lib.Initialize();

            Reporter = new Reporter(ErrorLogFilePath);
            Reporter.SetupUnhandledExceptionHandler("TweetDuck Has Failed :(");

            Config = new ConfigManager();
        }

        [STAThread]
        private static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Cef.EnableHighDPISupport();
            
            WindowRestoreMessage = NativeMethods.RegisterWindowMessage("TweetDuckRestore");

            if (!FileUtils.CheckFolderWritePermission(StoragePath)){
                FormMessage.Warning("Permission Error", "TweetDuck does not have write permissions to the storage folder: "+StoragePath, FormMessage.OK);
                return;
            }
            
            if (Arguments.HasFlag(Arguments.ArgRestart)){
                LockManager.Result lockResult = LockManager.LockWait(10000);

                while(lockResult != LockManager.Result.Success){
                    if (lockResult == LockManager.Result.Fail){
                        FormMessage.Error("TweetDuck Has Failed :(", "An unknown error occurred accessing the data folder. Please, make sure TweetDuck is not already running. If the problem persists, try restarting your system.", FormMessage.OK);
                        return;
                    }
                    else if (!FormMessage.Warning("TweetDuck Cannot Restart", "TweetDuck is taking too long to close.", FormMessage.Retry, FormMessage.Exit)){
                        return;
                    }

                    lockResult = LockManager.LockWait(5000);
                }
            }
            else{
                LockManager.Result lockResult = LockManager.Lock();
                
                if (lockResult == LockManager.Result.HasProcess){
                    if (!LockManager.RestoreLockingProcess(2000) && FormMessage.Error("TweetDuck is Already Running", "Another instance of TweetDuck is already running.\nDo you want to close it?", FormMessage.Yes, FormMessage.No)){
                        if (!LockManager.CloseLockingProcess(10000, 5000)){
                            FormMessage.Error("TweetDuck Has Failed :(", "Could not close the other process.", FormMessage.OK);
                            return;
                        }

                        lockResult = LockManager.Lock();
                    }
                    else return;
                }

                if (lockResult != LockManager.Result.Success){
                    FormMessage.Error("TweetDuck Has Failed :(", "An unknown error occurred accessing the data folder. Please, make sure TweetDuck is not already running. If the problem persists, try restarting your system.", FormMessage.OK);
                    return;
                }
            }
            
            Config.LoadAll();

            if (Arguments.HasFlag(Arguments.ArgImportCookies)){
                ProfileManager.ImportCookies();
            }
            else if (Arguments.HasFlag(Arguments.ArgDeleteCookies)){
                ProfileManager.DeleteCookies();
            }

            if (Arguments.HasFlag(Arguments.ArgUpdated)){
                WindowsUtils.TryDeleteFolderWhenAble(InstallerPath, 8000);
                BrowserCache.TryClearNow();
            }
            
            try{
                RequestHandlerBase.LoadResourceRewriteRules(Arguments.GetValue(Arguments.ArgFreeze, null));
            }catch(Exception e){
                FormMessage.Error("Resource Freeze", "Error parsing resource rewrite rules: "+e.Message, FormMessage.OK);
                return;
            }

            BrowserCache.RefreshTimer();

            CefSharpSettings.WcfEnabled = false;
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;

            CefSettings settings = new CefSettings{
                UserAgent = BrowserUtils.UserAgentChrome,
                BrowserSubprocessPath = BrandName+".Browser.exe",
                CachePath = StoragePath,
                UserDataPath = CefDataPath,
                LogFile = ConsoleLogFilePath,
                #if !DEBUG
                LogSeverity = Arguments.HasFlag(Arguments.ArgLogging) ? LogSeverity.Info : LogSeverity.Disable
                #endif
            };
            
            CommandLineArgs.ReadCefArguments(Config.User.CustomCefArgs).ToDictionary(settings.CefCommandLineArgs);
            BrowserUtils.SetupCefArgs(settings.CefCommandLineArgs);
            
            Cef.Initialize(settings, false, new BrowserProcessHandler());

            Application.ApplicationExit += (sender, args) => ExitCleanup();
            
            FormBrowser mainForm = new FormBrowser();
            Application.Run(mainForm);

            if (mainForm.UpdateInstallerPath != null){
                ExitCleanup();

                // ProgramPath has a trailing backslash
                string updaterArgs = "/SP- /SILENT /FORCECLOSEAPPLICATIONS /UPDATEPATH=\""+ProgramPath+"\" /RUNARGS=\""+Arguments.GetCurrentForInstallerCmd()+"\""+(IsPortable ? " /PORTABLE=1" : "");
                bool runElevated = !IsPortable || !FileUtils.CheckFolderWritePermission(ProgramPath);

                if (WindowsUtils.OpenAssociatedProgram(mainForm.UpdateInstallerPath, updaterArgs, runElevated)){
                    Application.Exit();
                }
                else{
                    RestartWithArgsInternal(Arguments.GetCurrentClean());
                }
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

        public static void Restart(params string[] extraArgs){
            CommandLineArgs args = Arguments.GetCurrentClean();
            CommandLineArgs.ReadStringArray('-', extraArgs, args);
            RestartWithArgs(args);
        }

        public static void RestartWithArgs(CommandLineArgs args){
            FormBrowser browserForm = FormManager.TryFind<FormBrowser>();

            if (browserForm != null){
                browserForm.ForceClose();

                ExitCleanup();
                RestartWithArgsInternal(args);
            }
        }

        private static void RestartWithArgsInternal(CommandLineArgs args){
            args.AddFlag(Arguments.ArgRestart);
            Process.Start(Application.ExecutablePath, args.ToString());
            Application.Exit();
        }

        private static void ExitCleanup(){
            if (HasCleanedUp)return;

            Config.SaveAll();

            Cef.Shutdown();
            BrowserCache.Exit();
            
            LockManager.Unlock();
            HasCleanedUp = true;
        }
    }
}
