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

[assembly: CLSCompliant(true)]
namespace TweetDck{
    static class Program{
        public const string BrandName = "TweetDuck";
        public const string Website = "https://tweetduck.chylex.com";

        public const string VersionTag = "1.5";
        public const string VersionFull = "1.5.0.0";

        public static readonly Version Version = new Version(VersionTag);

        public static readonly bool IsPortable = File.Exists("makeportable");
        private static readonly CommandLineArgs Args = CommandLineArgs.FromStringArray('-', Environment.GetCommandLineArgs());

        public static readonly string ProgramPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string StoragePath = IsPortable ? Path.Combine(ProgramPath, "portable", "storage") : GetDataStoragePath();
        public static readonly string TemporaryPath = IsPortable ? Path.Combine(ProgramPath, "portable", "tmp") : Path.Combine(Path.GetTempPath(), BrandName+'_'+Path.GetRandomFileName().Substring(0, 6));

        public static readonly string PluginDataPath = Path.Combine(StoragePath, "TD_Plugins");
        public static readonly string ConfigFilePath = Path.Combine(StoragePath, "TD_UserConfig.cfg");
        private static readonly string LogFilePath = Path.Combine(StoragePath, "TD_Log.txt");
        
        public static readonly string ScriptPath = Path.Combine(ProgramPath, "scripts");
        public static readonly string PluginPath = Path.Combine(ProgramPath, "plugins");

        public static uint WindowRestoreMessage;

        private static readonly LockManager LockManager = new LockManager(Path.Combine(StoragePath, ".lock"));
        private static bool HasCleanedUp;
        
        public static UserConfig UserConfig { get; private set; }
        public static Reporter Reporter { get; private set; }

        [STAThread]
        private static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WindowRestoreMessage = NativeMethods.RegisterWindowMessage("TweetDuckRestore");

            if (!WindowsUtils.CheckFolderWritePermission(StoragePath)){
                MessageBox.Show(BrandName+" does not have write permissions to the storage folder: "+StoragePath, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Reporter = new Reporter(LogFilePath);

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
                Exception ex = args.ExceptionObject as Exception;

                if (ex != null){
                    Reporter.HandleException(BrandName+" Has Failed :(", "An unhandled exception has occurred.", false, ex);
                }
            };

            if (Args.HasFlag("-restart")){
                for(int attempt = 0; attempt < 21; attempt++){
                    if (LockManager.Lock()){
                        break;
                    }
                    else if (attempt == 20){
                        using(FormMessage form = new FormMessage(BrandName+" Cannot Restart", BrandName+" is taking too long to close.", MessageBoxIcon.Warning)){
                            form.AddButton("Exit");
                            Button btnRetry = form.AddButton("Retry");

                            if (form.ShowDialog() == DialogResult.OK){
                                if (form.ClickedButton == btnRetry){
                                    attempt /= 2;
                                    continue;
                                }
                            }

                            return;
                        }
                    }
                    else{
                        Thread.Sleep(500);
                    }
                }
            }
            else{
                if (!LockManager.Lock()){
                    if (LockManager.LockingProcess.MainWindowHandle == IntPtr.Zero && LockManager.LockingProcess.Responding){ // restore if the original process is in tray
                        NativeMethods.SendMessage(NativeMethods.HWND_BROADCAST, WindowRestoreMessage, 0, IntPtr.Zero);
                        return;
                    }
                    else if (MessageBox.Show("Another instance of "+BrandName+" is already running.\r\nDo you want to close it?", BrandName+" is Already Running", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                        if (!LockManager.CloseLockingProcess(20000)){
                            MessageBox.Show("Could not close the other process.", BrandName+" Has Failed :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        LockManager.Lock();
                    }
                    else return;
                }
            }

            ReloadConfig();

            if (Args.HasFlag("-importcookies")){
                ExportManager.ImportCookies();
            }

            BrowserCache.ClearOldCacheFiles();

            CefSettings settings = new CefSettings{
                AcceptLanguageList = BrowserUtils.HeaderAcceptLanguage,
                UserAgent = BrowserUtils.HeaderUserAgent,
                Locale = Args.GetValue("-locale", "en"),
                CachePath = StoragePath,
                LogFile = Path.Combine(StoragePath, "TD_Console.txt"),
                #if !DEBUG
                BrowserSubprocessPath = BrandName+".Browser.exe",
                LogSeverity = Args.HasFlag("-log") ? LogSeverity.Info : LogSeverity.Disable
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
            plugins.PluginChangedState += (sender, args) => UserConfig.Save();
            plugins.Reload();

            FormBrowser mainForm = new FormBrowser(plugins);
            Application.Run(mainForm);

            if (mainForm.UpdateInstallerPath != null){
                ExitCleanup();

                bool runElevated = !IsPortable || !WindowsUtils.CheckFolderWritePermission(ProgramPath);

                WindowsUtils.StartProcess(mainForm.UpdateInstallerPath, "/SP- /SILENT /CLOSEAPPLICATIONS /UPDATEPATH=\""+ProgramPath+"\""+(IsPortable ? " /PORTABLE=1" : ""), runElevated); // ProgramPath has a trailing backslash
                Application.Exit();
            }
        }

        private static void plugins_Reloaded(object sender, PluginLoadEventArgs e){
            if (!e.Success){
                MessageBox.Show("The following plugins will not be available until the issues are resolved:\n"+string.Join("\n", e.Errors), "Error Loading Plugins", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            ((PluginManager)sender).SetConfig(UserConfig.Plugins);
        }

        private static string GetDataStoragePath(){
            string custom = Args.GetValue("-datafolder", null);

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
            FormBrowser browserForm = Application.OpenForms.OfType<FormBrowser>().FirstOrDefault();

            if (browserForm == null){
                return;
            }

            CommandLineArgs args = Args.Clone();
            args.AddFlag("-restart");
            args.RemoveFlag("-importcookies");
            
            CommandLineArgs.ReadStringArray('-', extraArgs, args);

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
