using CefSharp;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Core.Other;

namespace TweetDuck.Core.Utils{
    static class TwitterUtils{
        public const string TweetDeckURL = "https://tweetdeck.twitter.com";

        public static readonly Color BackgroundColor = Color.FromArgb(28, 99, 153);
        public const string BackgroundColorFix = "let e=document.createElement('style');document.head.appendChild(e);e.innerHTML='body::before{background:#1c6399!important}'";

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
            string file = BrowserUtils.GetFileNameFromUrl(ExtractImageBaseLink(url));
            string ext = Path.GetExtension(file); // includes dot
            
            using(SaveFileDialog dialog = new SaveFileDialog{
                AutoUpgradeEnabled = true,
                OverwritePrompt = true,
                Title = "Save image",
                FileName = file,
                Filter = string.IsNullOrEmpty(ext) ? "Image (unknown)|*.*" : $"Image (*{ext})|*{ext}"
            }){
                if (dialog.ShowDialog() == DialogResult.OK){
                    BrowserUtils.DownloadFileAsync(GetImageLink(url, quality), dialog.FileName, null, ex => {
                        FormMessage.Error("Image Download", "An error occurred while downloading the image: "+ex.Message, FormMessage.OK);
                    });
                }
            }
        }
    }
}
