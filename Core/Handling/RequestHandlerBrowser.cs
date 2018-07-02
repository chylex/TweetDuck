// Uncomment to force TweetDeck to load a predefined version of the vendor/bundle scripts
// #define FREEZE_TWEETDECK_SCRIPTS

using System.Collections.Specialized;
using CefSharp;
using TweetDuck.Core.Handling.Filters;
using TweetDuck.Core.Utils;

#if FREEZE_TWEETDECK_SCRIPTS
using System.Collections.Generic;
using System.Text.RegularExpressions;
#endif

namespace TweetDuck.Core.Handling{
    sealed class RequestHandlerBrowser : RequestHandlerBase{
        public string BlockNextUserNavUrl { get; set; }

        public RequestHandlerBrowser() : base(true){}

        public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback){
            if (request.ResourceType == ResourceType.Script && request.Url.Contains("analytics.")){
                callback.Dispose();
                return CefReturnValue.Cancel;
            }

            return base.OnBeforeResourceLoad(browserControl, browser, frame, request, callback);
        }

        public override bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect){
            if (userGesture && request.TransitionType == TransitionType.LinkClicked){
                bool block = request.Url == BlockNextUserNavUrl;
                BlockNextUserNavUrl = string.Empty;
                return block;
            }

            return base.OnBeforeBrowse(browserControl, browser, frame, request, userGesture, isRedirect);
        }

        #if FREEZE_TWEETDECK_SCRIPTS
        private static readonly Regex TweetDeckScriptUrl = new Regex(@"dist\/(.*?)\.(.*?)\.js$", RegexOptions.Compiled);
        
        private static readonly SortedList<string, string> TweetDeckHashes = new SortedList<string, string>(2){
            { "vendor", "942c0a20e8" },
            { "bundle", "1bd75b5854" }
        };
        #endif

        public override bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response){
            if (request.ResourceType == ResourceType.Image && request.Url.Contains("backgrounds/spinner_blue")){
                request.Url = TwitterUtils.LoadingSpinner.Url;
                return true;
            }
            #if FREEZE_TWEETDECK_SCRIPTS
            else if (request.ResourceType == ResourceType.Script){
                Match match = TweetDeckScriptUrl.Match(request.Url);

                if (match.Success && TweetDeckHashes.TryGetValue(match.Groups[1].Value, out string hash)){
                    if (match.Groups[2].Value == hash){
                        System.Diagnostics.Debug.WriteLine($"accepting {request.Url}");
                    }
                    else{
                        System.Diagnostics.Debug.WriteLine($"rewriting {request.Url} to {hash}");
                        request.Url = TweetDeckScriptUrl.Replace(request.Url, "dist/$1."+hash+".js");
                        return true;
                    }
                }
            }
            #endif

            return base.OnResourceResponse(browserControl, browser, frame, request, response);
        }

        public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response){
            if (request.ResourceType == ResourceType.Script && request.Url.Contains("dist/vendor")){
                NameValueCollection headers = response.ResponseHeaders;

                if (int.TryParse(headers["x-ton-expected-size"], out int totalBytes)){
                    return new ResponseFilterVendor(totalBytes);
                }
                #if DEBUG
                else{
                    System.Diagnostics.Debug.WriteLine($"Missing uncompressed size header in {request.Url}");

                    foreach(string key in headers){
                        System.Diagnostics.Debug.WriteLine($" {key}: {headers[key]}");
                    }

                    System.Diagnostics.Debugger.Break();
                }
                #endif
            }

            return base.GetResourceResponseFilter(browserControl, browser, frame, request, response);
        }
    }
}
