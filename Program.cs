using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using TweetDck.Configuration;
using TweetDck.Core;
using TweetDck.Migration;
using TweetDck.Core.Utils;
using System.Linq;
using System.Threading;
using TweetDck.Plugins;
using TweetDck.Plugins.Events;
using TweetDck.Core.Other.Settings.Export;
using TweetDck.Core.Handling;
using System.Security.AccessControl;

[assembly: CLSCompliant(true)]
namespace TweetDck{
    static class Program{
        public const string BrandName = "TweetDuck";
        public const string Website = "http://tweetduck.chylex.com";

        public const string BrowserSubprocess = BrandName+".Browser.exe";

        public const string VersionTag = "1.3.3";
        public const string VersionFull = "1.3.3.0";

        public static readonly Version Version = new Version(VersionTag);

        public static readonly bool IsPortable = File.Exists("makeportable");

        public static readonly string ProgramPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string StoragePath = IsPortable ? Path.Combine(ProgramPath, "portable", "storage") : GetDataStoragePath();
        public static readonly string TemporaryPath = IsPortable ? Path.Combine(ProgramPath, "portable", "tmp") : Path.Combine(Path.GetTempPath(), BrandName+'_'+Path.GetRandomFileName().Substring(0, 6));

        public static readonly string ConfigFilePath = Path.Combine(StoragePath, "TD_UserConfig.cfg");
        private static readonly string LogFilePath = Path.Combine(ProgramPath, "td-log.txt");
        
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

            if (!WindowsUtils.CheckFolderPermission(ProgramPath, FileSystemRights.WriteData)){
                MessageBox.Show(BrandName+" does not have write permissions to the program folder. If it is installed in Program Files, please run it as Administrator.", "Administrator Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Reporter = new Reporter(LogFilePath);

            string[] programArguments = Environment.GetCommandLineArgs();

            if (programArguments.Contains("-restart")){
                for(int attempt = 0; attempt < 41; attempt++){
                    if (LockManager.Lock()){
                        break;
                    }
                    else if (attempt == 40){
                        MessageBox.Show(BrandName+" is taking too long to close, please wait and then start the application again manually.", BrandName+" Cannot Restart", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else{
                        Thread.Sleep(500);
                    }
                }

                ReloadConfig();
            }
            else{
                ReloadConfig();
                MigrationManager.Run();

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

            if (programArguments.Contains("-importcookies") && File.Exists(ExportManager.TempCookiesPath)){
                try{
                    if (File.Exists(ExportManager.CookiesPath)){
                        File.Delete(ExportManager.CookiesPath);
                    }

                    File.Move(ExportManager.TempCookiesPath, ExportManager.CookiesPath);
                }catch(Exception e){
                    HandleException("Could not import the cookie file to restore login session.", e);
                }
            }

            BrowserCache.ClearOldCacheFiles();

            CefSettings settings = new CefSettings{
                AcceptLanguageList = BrowserUtils.HeaderAcceptLanguage,
                UserAgent = BrowserUtils.HeaderUserAgent,
                Locale = GetCmdArgumentValue("locale") ?? "en",
                CachePath = StoragePath,
                BrowserSubprocessPath = File.Exists(BrowserSubprocess) ? BrowserSubprocess : "CefSharp.BrowserSubprocess.exe",
                #if !DEBUG
                LogSeverity = programArguments.Contains("-log") ? LogSeverity.Info : LogSeverity.Disable
                #endif
            };

            CommandLineArgsParser.AddToDictionary(UserConfig.CustomCefArgs, settings.CefCommandLineArgs);

            Cef.Initialize(settings, false, new BrowserProcessHandler());

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
                Exception ex = args.ExceptionObject as Exception;

                if (ex != null){
                    HandleException("An unhandled exception has occurred.", ex);
                }
            };

            Application.ApplicationExit += (sender, args) => ExitCleanup();

            PluginManager plugins = new PluginManager(PluginPath, UserConfig.Plugins);
            plugins.Reloaded += plugins_Reloaded;
            plugins.Config.PluginChangedState += (sender, args) => UserConfig.Save();
            plugins.Reload();

            FormBrowser mainForm = new FormBrowser(plugins);
            Application.Run(mainForm);

            if (mainForm.UpdateInstallerPath != null){
                ExitCleanup();

                Process.Start(mainForm.UpdateInstallerPath, "/SP- /SILENT /NOICONS /CLOSEAPPLICATIONS");
                Application.Exit();
            }
        }

        private static void plugins_Reloaded(object sender, PluginLoadEventArgs e){
            if (!e.Success){
                MessageBox.Show("The following plugins will not be available until the issues are resolved:\n"+string.Join("\n", e.Errors), "Error Loading Plugins", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static string GetCmdArgumentValue(string search){
            string[] args = Environment.GetCommandLineArgs();
            int index = Array.FindIndex(args, arg => arg.Equals(search, StringComparison.OrdinalIgnoreCase));

            return index >= 0 && index < args.Length-1 ? args[index+1] : null;
        }

        private static string GetDataStoragePath(){
            string custom = GetCmdArgumentValue("-datafolder");

            if (custom != null && (custom.Contains(Path.DirectorySeparatorChar) || custom.Contains(Path.AltDirectorySeparatorChar))){
                return custom;
            }
            else{
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), custom ?? BrandName);
            }
        }

        public static void HandleException(string message, Exception e){ // TODO replace all uses
            Reporter.HandleException(BrandName+" Has Failed :(", message, false, new Exception());
        }

        public static void Log(string message){ // TODO replace all uses
            Reporter.Log(message);
        }

        public static void ReloadConfig(){
            UserConfig = UserConfig.Load(ConfigFilePath);
        }

        public static void ResetConfig(){
            try{
                File.Delete(ConfigFilePath);
                File.Delete(UserConfig.GetBackupFile(ConfigFilePath));
            }catch(Exception e){
                HandleException("Could not delete configuration files to reset the settings.", e);
                return;
            }

            ReloadConfig();
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
