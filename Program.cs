using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using TweetDck.Configuration;
using TweetDck.Core;
using TweetDck.Migration;
using TweetDck.Core.Utils;
using System.Linq;
using System.Threading;
using TweetDck.Plugins;

[assembly: CLSCompliant(true)]
namespace TweetDck{
    static class Program{
        #if DUCK
        public const string BrandName = "TweetDuck";
        public const string Website = "http://tweetduck.chylex.com";
        #else
        public const string BrandName = "TweetDick";
        public const string Website = "http://tweetdick.chylex.com";
        #endif

        public const string VersionTag = "1.2.3";
        public const string VersionFull = "1.2.3.0";

        public static readonly string StoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),BrandName);
        public static readonly string PluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"plugins");
        public static readonly string TemporaryPath = Path.Combine(Path.GetTempPath(),BrandName);

        private static readonly LockManager LockManager = new LockManager(Path.Combine(StoragePath,".lock"));
        private static bool HasCleanedUp;
        
        public static UserConfig UserConfig { get; private set; }

        public static string LogFile{
            get{
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"td-log.txt");
            }
        }

        [STAThread]
        private static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Environment.GetCommandLineArgs().Contains("-restart")){
                for(int attempt = 0; attempt < 21; attempt++){
                    if (LockManager.Lock()){
                        break;
                    }
                    else if (attempt == 20){
                        MessageBox.Show(BrandName+" is taking too long to close, please wait and then start the application again manually.",BrandName+" Cannot Restart",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }
                    else{
                        Thread.Sleep(500);
                    }
                }
            }
            else{
                if (!LockManager.Lock()){
                    if (MessageBox.Show("Another instance of "+BrandName+" is already running.\r\nDo you want to close it?",BrandName+" is Already Running",MessageBoxButtons.YesNo,MessageBoxIcon.Error,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                        if (!LockManager.CloseLockingProcess(10000)){
                            MessageBox.Show("Could not close the other process.",BrandName+" Has Failed :(",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            return;
                        }

                        LockManager.Lock();
                    }
                    else return;
                }
            }

            UserConfig = UserConfig.Load(Path.Combine(StoragePath,"TD_UserConfig.cfg"));

            MigrationManager.Run();

            Cef.OnContextInitialized = () => {
                using(IRequestContext ctx = Cef.GetGlobalRequestContext()){
                    string err;
                    ctx.SetPreference("browser.enable_spellchecking",false,out err);
                }
            };

            Cef.Initialize(new CefSettings{
                AcceptLanguageList = BrowserUtils.HeaderAcceptLanguage,
                UserAgent = BrowserUtils.HeaderUserAgent,
                Locale = CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                CachePath = StoragePath,
                #if !DEBUG
                LogSeverity = LogSeverity.Disable
                #endif
            });

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
                Exception ex = args.ExceptionObject as Exception;

                if (ex != null){
                    HandleException("An unhandled exception has occurred.",ex);
                }
            };

            Application.ApplicationExit += (sender, args) => ExitCleanup();

            PluginManager plugins = new PluginManager(PluginPath,UserConfig.Plugins);
            plugins.ReloadError += plugins_ReloadError;
            plugins.Config.PluginChangedState += (sender, args) => UserConfig.Save();
            plugins.Reload();

            FormBrowser mainForm = new FormBrowser(plugins);
            Application.Run(mainForm);

            if (mainForm.UpdateInstallerPath != null){
                ExitCleanup();

                Process.Start(mainForm.UpdateInstallerPath,"/SP- /SILENT /NOICONS /CLOSEAPPLICATIONS");
                Application.Exit();
            }
        }

        private static void plugins_ReloadError(object sender, PluginLoadErrorEventArgs e){
            MessageBox.Show("The following plugins will not be available until the issues are resolved:\n"+string.Join("\n",e.Errors),"Error Loading Plugins",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }

        public static void HandleException(string message, Exception e){
            Log(e.ToString());
            
            if (MessageBox.Show(message+"\r\nDo you want to open the log file to report the issue?",BrandName+" Has Failed :(",MessageBoxButtons.YesNo,MessageBoxIcon.Error,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                Process.Start(LogFile);
            }
        }

        public static void Log(string data){
            StringBuilder build = new StringBuilder();

            if (!File.Exists(LogFile)){
                build.Append("Please, report all issues to: https://github.com/chylex/TweetDuck/issues\r\n\r\n");
            }

            build.Append("[").Append(DateTime.Now.ToString("G")).Append("]\r\n");
            build.Append(data).Append("\r\n\r\n");

            try{
                File.AppendAllText(LogFile,build.ToString(),Encoding.UTF8);
            }catch{
                // oops
            }
        }

        private static void ExitCleanup(){
            if (HasCleanedUp)return;

            UserConfig.Save();

            try{
                Directory.Delete(TemporaryPath,true);
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
