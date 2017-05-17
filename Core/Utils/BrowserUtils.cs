using CefSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TweetDuck.Core.Utils{
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

        public static string HeaderUserAgent => Program.BrandName+" "+Application.ProductVersion;

        public static readonly Color BackgroundColor = Color.FromArgb(28, 99, 153);
        public const string BackgroundColorFix = "let e=document.createElement('style');document.head.appendChild(e);e.innerHTML='body::before{background:#1c6399!important}'";

        public static readonly string[] DictionaryWords = {
            "tweetdeck", "TweetDeck", "tweetduck", "TweetDuck", "TD"
        };

        public static bool IsValidUrl(string url){
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri)){
                string scheme = uri.Scheme;
                return scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeHttps || scheme == Uri.UriSchemeFtp || scheme == Uri.UriSchemeMailto;
            }

            return false;
        }

        public static void OpenExternalBrowser(string url){
            if (string.IsNullOrWhiteSpace(url))return;

            if (IsValidUrl(url)){
                OpenExternalBrowserUnsafe(url);
            }
            else{
                MessageBox.Show("A potentially malicious URL was blocked from opening:"+Environment.NewLine+url, "Blocked URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static void OpenExternalBrowserUnsafe(string url){
            using(Process.Start(url)){}
        }

        public static string GetFileNameFromUrl(string url){
            string file = Path.GetFileName(new Uri(url).AbsolutePath);
            return string.IsNullOrEmpty(file) ? null : file;
        }

        public static string ConvertPascalCaseToScreamingSnakeCase(string str){
            return Regex.Replace(str, @"(\p{Ll})(\P{Ll})|(\P{Ll})(\P{Ll}\p{Ll})", "$1$3_$2$4").ToUpperInvariant();
        }

        public static string GetErrorName(CefErrorCode code){
            return ConvertPascalCaseToScreamingSnakeCase(Enum.GetName(typeof(CefErrorCode), code) ?? string.Empty);
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

        public static void SetZoomLevel(IBrowser browser, int percentage){
            browser.GetHost().SetZoomLevel(Math.Log(percentage/100.0, 1.2));
        }

        public static bool IsTweetDeckWebsite(IFrame frame){
            return frame.Url.Contains("//tweetdeck.twitter.com/");
        }

        public static bool IsTwitterWebsite(IFrame frame){
            return frame.Url.Contains("//twitter.com/");
        }

        #if DEBUG
        public static void HandleConsoleMessage(object sender, ConsoleMessageEventArgs e){
            Debug.WriteLine("[Console] {0} ({1}:{2})", e.Message, e.Source, e.Line);
        }
        #endif
    }
}
