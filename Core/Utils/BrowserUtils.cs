using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Windows.Forms;

namespace TweetDck.Core.Utils{
    static class BrowserUtils{
        public static string HeaderAcceptLanguage{
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

        public static string HeaderUserAgent{
            get{
               return Program.BrandName+" "+Application.ProductVersion; 
            }
        }

        public static void OpenExternalBrowser(string url){ // TODO implement mailto
            Process.Start(url);
        }
    }
}
