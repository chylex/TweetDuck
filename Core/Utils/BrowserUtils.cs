using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CefSharp;

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
            using(Process.Start(url)){}
        }

        public static string GetFileNameFromUrl(string url){
            string file = Path.GetFileName(new Uri(url).AbsolutePath);
            return string.IsNullOrEmpty(file) ? null : file;
        }

        public static void DownloadFileAsync(string url, string target, Action<Exception> onFailure){
            WebClient client = new WebClient{ Proxy = null };
            client.Headers[HttpRequestHeader.UserAgent] = HeaderUserAgent;

            client.DownloadFileCompleted += (sender, args) => {
                if (args.Error != null){
                    onFailure(args.Error);
                }
            };

            client.DownloadFileAsync(new Uri(url), target);
        }

        public static bool IsTweetDeckWebsite(IFrame frame){
            return frame.Url.Contains("//tweetdeck.twitter.com/");
        }

        #if DEBUG
        public static void HandleConsoleMessage(object sender, ConsoleMessageEventArgs e){
            Debug.WriteLine("[Console] {0} ({1}:{2})", e.Message, e.Source, e.Line);
        }
        #endif
    }
}
