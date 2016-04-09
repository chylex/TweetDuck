using CefSharp;
using System;
using System.Globalization;
using System.Windows.Forms;
using TweetDick.Forms;

namespace TweetDick{
    static class Program{
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
            Cef.Initialize(new CefSettings{
                AcceptLanguageList = HeaderAcceptLanguage,
                UserAgent = "TweetDick "+Application.ProductVersion,
                Locale = CultureInfo.CurrentCulture.TwoLetterISOLanguageName
            });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormBrowser());

            Application.ApplicationExit += (sender, args) => {
                Cef.Shutdown();
            };
        }
    }
}
