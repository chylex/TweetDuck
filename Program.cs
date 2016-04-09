using CefSharp;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using TweetDick.Core;

namespace TweetDick{
    static class Program{
        public static string StoragePath{
            get{
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"TweetDick");
            }
        }

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

        [STAThread]
        private static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Cef.Initialize(new CefSettings{
                AcceptLanguageList = HeaderAcceptLanguage,
                UserAgent = "TweetDick "+Application.ProductVersion,
                Locale = CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                CachePath = StoragePath
            });

            Application.Run(new FormBrowser());

            Application.ApplicationExit += (sender, args) => {
                Cef.Shutdown();
            };
        }
    }
}
