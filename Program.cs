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
        public static readonly string StoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"TweetDick");
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

        [STAThread]
        private static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MigrationManager.Run();

            Cef.Initialize(new CefSettings{
                AcceptLanguageList = HeaderAcceptLanguage,
                UserAgent = "TweetDick "+Application.ProductVersion,
                Locale = CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                CachePath = StoragePath
            });

            Application.Run(new FormBrowser());

            Application.ApplicationExit += (sender, args) => {
                UserConfig.Save();
                Cef.Shutdown();
            };
        }
    }
}
