using System;
using System.IO;
using System.Text.RegularExpressions;
using TweetLib.Core.Utils;

namespace TweetLib.Core.Features.Twitter{
    public static class TwitterUrls{
        public const string TweetDeck = "https://tweetdeck.twitter.com";
        private const string TwitterTrackingUrl = "t.co";

        public static Regex RegexAccount { get; } = new Regex(@"^https?://twitter\.com/(?!signup$|tos$|privacy$|search$|search-)([^/?]+)/?$");

        public static bool IsTweetDeck(string url){
            return url.Contains("//tweetdeck.twitter.com/");
        }

        public static bool IsTwitter(string url){
            return url.Contains("//twitter.com/") || url.Contains("//mobile.twitter.com/");
        }

        public static bool IsTwitterLogin2Factor(string url){
            return url.Contains("//twitter.com/account/login_verification") || url.Contains("//mobile.twitter.com/account/login_verification");
        }

        public static string? GetFileNameFromUrl(string url){
            return StringUtils.NullIfEmpty(Path.GetFileName(new Uri(url).AbsolutePath));
        }

        public static string GetMediaLink(string url, ImageQuality quality){
            return ImageUrl.TryParse(url, out var obj) ? obj.WithQuality(quality) : url;
        }

        public static string? GetImageFileName(string url){
            return GetFileNameFromUrl(ImageUrl.TryParse(url, out var obj) ? obj.WithNoQuality : url);
        }

        public enum UrlType{
            Invalid, Tracking, Fine
        }

        public static UrlType Check(string url){
            if (url.Contains("\"")){
                return UrlType.Invalid;
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri)){
                string scheme = uri.Scheme;

                if (scheme == Uri.UriSchemeHttps || scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeFtp || scheme == Uri.UriSchemeMailto){
                    return uri.Host == TwitterTrackingUrl ? UrlType.Tracking : UrlType.Fine;
                }
            }

            return UrlType.Invalid;
        }
    }
}
