using System;
using CefSharp;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TweetDuck.Core.Other;

namespace TweetDuck.Core.Utils{
    static class TwitterUtils{
        public const string TweetDeckURL = "https://tweetdeck.twitter.com";

        public static readonly Color BackgroundColor = Color.FromArgb(28, 99, 153);
        public const string BackgroundColorFix = "let e=document.createElement('style');document.head.appendChild(e);e.innerHTML='body::before{background:#1c6399!important}'";
        
        private static readonly Lazy<Regex> RegexAccountLazy = new Lazy<Regex>(() => new Regex(@"^https?://twitter\.com/([^/]+)/?$", RegexOptions.Compiled), false);
        public static Regex RegexAccount => RegexAccountLazy.Value;

        public static readonly string[] DictionaryWords = {
            "tweetdeck", "TweetDeck", "tweetduck", "TweetDuck", "TD"
        };

        public enum ImageQuality{
            Default, Orig
        }

        public static bool IsTweetDeckWebsite(IFrame frame){
            return frame.Url.Contains("//tweetdeck.twitter.com/");
        }

        public static bool IsTwitterWebsite(IFrame frame){
            return frame.Url.Contains("//twitter.com/");
        }

        private static string ExtractImageBaseLink(string url){
            int dot = url.LastIndexOf('.');
            return dot == -1 ? url : StringUtils.ExtractBefore(url, ':', dot);
        }

        public static string GetImageLink(string url, ImageQuality quality){
            if (quality == ImageQuality.Orig){
                string result = ExtractImageBaseLink(url);

                if (result != url){
                    result += ":orig";
                }

                return result;
            }
            else{
                return url;
            }
        }
        
        public static void DownloadImage(string url, ImageQuality quality){
            DownloadImages(new string[]{ url }, quality);
        }

        public static void DownloadImages(string[] urls, ImageQuality quality){
            if (urls.Length == 0){
                return;
            }

            string file = BrowserUtils.GetFileNameFromUrl(ExtractImageBaseLink(urls[0]));
            string ext = Path.GetExtension(file); // includes dot
            
            using(SaveFileDialog dialog = new SaveFileDialog{
                AutoUpgradeEnabled = true,
                OverwritePrompt = urls.Length == 1,
                Title = "Save image",
                FileName = file,
                Filter = (urls.Length == 1 ? "Image" : "Images")+(string.IsNullOrEmpty(ext) ? " (unknown)|*.*" : $" (*{ext})|*{ext}")
            }){
                if (dialog.ShowDialog() == DialogResult.OK){
                    void OnFailure(Exception ex){
                        FormMessage.Error("Image Download", "An error occurred while downloading the image: "+ex.Message, FormMessage.OK);
                    }

                    if (urls.Length == 1){
                        BrowserUtils.DownloadFileAsync(GetImageLink(urls[0], quality), dialog.FileName, null, OnFailure);
                    }
                    else{
                        string pathBase = Path.ChangeExtension(dialog.FileName, null);
                        string pathExt = Path.GetExtension(dialog.FileName);

                        for(int index = 0; index < urls.Length; index++){
                            BrowserUtils.DownloadFileAsync(GetImageLink(urls[index], quality), pathBase+(index+1)+pathExt, null, OnFailure);
                        }
                    }
                }
            }
        }
    }
}
