// Uncomment to force TweetDeck to load a predefined version of the vendor/bundle scripts
#define FREEZE_TWEETDECK_SCRIPTS // TODO delaying the apocalypse

using System.Text.RegularExpressions;
using CefSharp;
using TweetDuck.Core.Utils;

#if FREEZE_TWEETDECK_SCRIPTS
using System.Collections.Generic;
using System.Diagnostics;
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
                        Debug.WriteLine($"accepting {request.Url}");
                    }
                    else{
                        Debug.WriteLine($"rewriting {request.Url} to {hash}");
                        request.Url = TweetDeckScriptUrl.Replace(request.Url, "dist/$1."+hash+".js");
                        return true;
                    }
                }
            }
            #endif

            return base.OnResourceResponse(browserControl, browser, frame, request, response);
        }
    }
}
