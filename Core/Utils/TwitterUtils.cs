using System;
using CefSharp;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TweetDuck.Core.Management;
using TweetDuck.Core.Other;
using TweetDuck.Data;
using System.Linq;

namespace TweetDuck.Core.Utils{
    static class TwitterUtils{
        public const string TweetDeckURL = "https://tweetdeck.twitter.com";

        public static readonly Color BackgroundColor = Color.FromArgb(28, 99, 153);
        public const string BackgroundColorOverride = "setTimeout(function f(){let h=document.head;if(!h){setTimeout(f,5);return;}let e=document.createElement('style');e.innerHTML='body,body::before{background:#1c6399!important}';h.appendChild(e);},1)";

        public static readonly ResourceLink LoadingSpinner = new ResourceLink("https://ton.twimg.com/tduck/spinner", ResourceHandler.FromByteArray(Properties.Resources.spinner, "image/apng"));
        
        private static readonly Lazy<Regex> RegexAccountLazy = new Lazy<Regex>(() => new Regex(@"^https?://twitter\.com/(?!signup$|tos$|privacy$|search$|search-)([^/?]+)/?$", RegexOptions.Compiled), false);
        public static Regex RegexAccount => RegexAccountLazy.Value;

        public static readonly string[] DictionaryWords = {
            "tweetdeck", "TweetDeck", "tweetduck", "TweetDuck", "TD"
        };

        public static readonly string[] ValidImageExtensions = {
            ".jpg", ".jpeg", ".png", ".gif"
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
            int slash = url.LastIndexOf('/');
            return slash == -1 ? url : StringUtils.ExtractBefore(url, ':', slash);
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

        public static string GetImageFileName(string url){
            return BrowserUtils.GetFileNameFromUrl(ExtractMediaBaseLink(url));
        }

        public static void ViewImage(string url, ImageQuality quality){
            void ViewImageInternal(string path){
                string ext = Path.GetExtension(path);

                if (ValidImageExtensions.Contains(ext)){
                    WindowsUtils.OpenAssociatedProgram(path);
                }
                else{
                    FormMessage.Error("Image Download", "Invalid file extension "+ext, FormMessage.OK);
                }
            }

            string file = Path.Combine(BrowserCache.CacheFolder, GetImageFileName(url) ?? Path.GetRandomFileName());

            if (File.Exists(file)){
                ViewImageInternal(file);
            }
            else{
                BrowserUtils.DownloadFileAsync(GetMediaLink(url, quality), file, () => {
                    ViewImageInternal(file);
                }, ex => {
                    FormMessage.Error("Image Download", "An error occurred while downloading the image: "+ex.Message, FormMessage.OK);
                });
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

            string filename = GetImageFileName(firstImageLink);
            string ext = Path.GetExtension(filename); // includes dot
            
            using(SaveFileDialog dialog = new SaveFileDialog{
                AutoUpgradeEnabled = true,
                OverwritePrompt = urls.Length == 1,
                Title = "Save Image",
                FileName = qualityIndex == -1 ? filename : $"{username} {Path.ChangeExtension(filename, null)} {firstImageLink.Substring(qualityIndex+1)}".Trim()+ext,
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
                Title = "Save Video",
                FileName = string.IsNullOrEmpty(username) ? filename : $"{username} {filename}".TrimStart(),
                Filter = "Video"+(string.IsNullOrEmpty(ext) ? " (unknown)|*.*" : $" (*{ext})|*{ext}")
            }){
                if (dialog.ShowDialog() == DialogResult.OK){
                    BrowserUtils.DownloadFileAsync(url, dialog.FileName, null, ex => {
                        FormMessage.Error("Video Download", "An error occurred while downloading the video: "+ex.Message, FormMessage.OK);
                    });
                }
            }
        }
    }
}
