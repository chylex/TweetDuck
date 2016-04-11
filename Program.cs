using CefSharp;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TweetDick.Configuration;
using TweetDick.Core;
using TweetDick.Migration;

namespace TweetDick{
    static class Program{
        #if DUCK
        public const string BrandName = "TweetDuck";
        public const string Website = "http://tweetduck.chylex.com";
        #else
        public const string BrandName = "TweetDick";
        public const string Website = "http://tweetdick.chylex.com";
        #endif

        public static readonly string StoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),BrandName);
        public static readonly UserConfig UserConfig;

        private static string HeaderAcceptLanguage{
            get{
                string culture = CultureInfo.CurrentCulture.Name;

                if (culture == "en"){
                    return "en-us,en";
                }
                else{
                    return culture.ToLowerInvariant()+",en;q=0.9";
                }
            }
        }

        static Program(){
            UserConfig = UserConfig.Load(Path.Combine(StoragePath,"TD_UserConfig.cfg"));
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("Shell32.dll")]
        public static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        [STAThread]
        private static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MigrationManager.Run();

            Cef.OnContextInitialized = () => {
                using(IRequestContext ctx = Cef.GetGlobalRequestContext()){
                    string err;
                    ctx.SetPreference("browser.enable_spellchecking",false,out err);
                }
            };

            Cef.Initialize(new CefSettings{
                AcceptLanguageList = HeaderAcceptLanguage,
                UserAgent = BrandName+" "+Application.ProductVersion,
                Locale = CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                CachePath = StoragePath,
                #if !DEBUG
                LogSeverity = LogSeverity.Disable
                #endif
            });

            Application.Run(new FormBrowser());

            Application.ApplicationExit += (sender, args) => {
                UserConfig.Save();
                Cef.Shutdown();
            };
        }
    }
}
