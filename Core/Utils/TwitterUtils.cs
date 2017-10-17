using System;
using CefSharp;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TweetDuck.Core.Other;

namespace TweetDuck.Core.Utils{
    static class TwitterUtils{
        public const string TweetDeckURL = "https://tweetdeck.twitter.com";

        public static readonly Color BackgroundColor = Color.FromArgb(28, 99, 153);
        public const string BackgroundColorFix = "let e=document.createElement('style');document.head.appendChild(e);e.innerHTML='body::before{background:#1c6399!important}'";
        
        private static readonly Lazy<Regex> RegexAccountLazy = new Lazy<Regex>(() => new Regex(@"^https?://twitter\.com/(?!signup$|tos$|privacy$)([^/]+)/?$", RegexOptions.Compiled), false);
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

        private static string ExtractMediaBaseLink(string url){
            int dot = url.LastIndexOf('/');
            return dot == -1 ? url : StringUtils.ExtractBefore(url, ':', dot);
        }

        public static string GetMediaLink(string url, ImageQuality quality){
            if (quality == ImageQuality.Orig){
                string result = ExtractMediaBaseLink(url);

                if (result != url || url.Contains("//pbs.twimg.com/media/")){
                    result += ":orig";
                }

                return result;
            }
            else{
                return url;
            }
        }
        
        public static void DownloadImage(string url, string username, ImageQuality quality){
            DownloadImages(new string[]{ url }, username, quality);
        }

        public static void DownloadImages(string[] urls, string username, ImageQuality quality){
            if (urls.Length == 0){
                return;
            }

            string firstImageLink = GetMediaLink(urls[0], quality);
            int qualityIndex = firstImageLink.IndexOf(':', firstImageLink.LastIndexOf('/'));

            string file = BrowserUtils.GetFileNameFromUrl(ExtractMediaBaseLink(firstImageLink));
            string ext = Path.GetExtension(file); // includes dot

            string[] fileNameParts = qualityIndex == -1 ? new string[]{
                Path.ChangeExtension(file, null)
            } : new string[]{
                username,
                Path.ChangeExtension(file, null),
                firstImageLink.Substring(qualityIndex+1)
            };
            
            using(SaveFileDialog dialog = new SaveFileDialog{
                AutoUpgradeEnabled = true,
                OverwritePrompt = urls.Length == 1,
                Title = "Save image",
                FileName = $"{string.Join(" ", fileNameParts.Where(part => part.Length > 0))}{ext}",
                Filter = (urls.Length == 1 ? "Image" : "Images")+(string.IsNullOrEmpty(ext) ? " (unknown)|*.*" : $" (*{ext})|*{ext}")
            }){
                if (dialog.ShowDialog() == DialogResult.OK){
                    void OnFailure(Exception ex){
                        FormMessage.Error("Image Download", "An error occurred while downloading the image: "+ex.Message, FormMessage.OK);
                    }

                    if (urls.Length == 1){
                        BrowserUtils.DownloadFileAsync(firstImageLink, dialog.FileName, null, OnFailure);
                    }
                    else{
                        string pathBase = Path.ChangeExtension(dialog.FileName, null);
                        string pathExt = Path.GetExtension(dialog.FileName);

                        for(int index = 0; index < urls.Length; index++){
                            BrowserUtils.DownloadFileAsync(GetMediaLink(urls[index], quality), $"{pathBase} {index+1}{pathExt}", null, OnFailure);
                        }
                    }
                }
            }
        }

        public static void DownloadVideo(string url, string username){
            string filename = BrowserUtils.GetFileNameFromUrl(url);
            string ext = Path.GetExtension(filename);

            using(SaveFileDialog dialog = new SaveFileDialog{
                AutoUpgradeEnabled = true,
                OverwritePrompt = true,
                Title = "Save video",
                FileName = string.IsNullOrEmpty(username) ? filename : $"{username} {filename}",
                Filter = "Video"+(string.IsNullOrEmpty(ext) ? " (unknown)|*.*" : $" (*{ext})|*{ext}")
            }){
                if (dialog.ShowDialog() == DialogResult.OK){
                    BrowserUtils.DownloadFileAsync(url, dialog.FileName, null, ex => {
                        FormMessage.Error("Image Download", "An error occurred while downloading the image: "+ex.Message, FormMessage.OK);
                    });
                }
            }
        }
    }
}
