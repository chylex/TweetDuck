using System;
using System.IO;

namespace TweetLib.Core.Utils{
    public static class UrlUtils{
        private const string TwitterTrackingUrl = "t.co";

        public enum CheckResult{
            Invalid, Tracking, Fine
        }

        public static CheckResult Check(string url){
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri)){
                string scheme = uri.Scheme;

                if (scheme == Uri.UriSchemeHttps || scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeFtp || scheme == Uri.UriSchemeMailto){
                    return uri.Host == TwitterTrackingUrl ? CheckResult.Tracking : CheckResult.Fine;
                }
            }

            return CheckResult.Invalid;
        }

        public static string? GetFileNameFromUrl(string url){
            string file = Path.GetFileName(new Uri(url).AbsolutePath);
            return string.IsNullOrEmpty(file) ? null : file;
        }
    }
}
