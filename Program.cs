using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using TweetDck.Configuration;
using TweetDck.Core;
using TweetDck.Migration;
using TweetDck.Core.Utils;

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

        public const string VersionTag = "1.1.1";

        public static readonly string StoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),BrandName);
        private static readonly LockManager LockManager = new LockManager(Path.Combine(StoragePath,".lock"));
        
        public static UserConfig UserConfig { get; private set; }

        public static string LogFile{
            get{
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"td-log.txt");
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("Shell32.dll")]
        public static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        [STAThread]
        private static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            if (!LockManager.Lock()){
                if (MessageBox.Show("Another instance of "+BrandName+" is already running.\r\nDo you want to close it?",BrandName+" is Already Running",MessageBoxButtons.YesNo,MessageBoxIcon.Error,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    if (!LockManager.CloseLockingProcess(10000)){
                        MessageBox.Show("Could not close the other process.",BrandName+" Has Failed :(",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }
                }
                else return;
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

            Application.ApplicationExit += (sender, args) => {
                UserConfig.Save();
                LockManager.Unlock();
                Cef.Shutdown();
            };

            FormBrowser mainForm = new FormBrowser();
            Application.Run(mainForm);

            if (mainForm.UpdateInstallerPath != null){
                Cef.Shutdown();

                Process.Start(mainForm.UpdateInstallerPath,"/SP- /SILENT /NOICONS /CLOSEAPPLICATIONS");
                Application.Exit();
            }
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
    }
}
