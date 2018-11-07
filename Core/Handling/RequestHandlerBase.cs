// Uncomment to force TweetDeck to load a predefined version of the vendor/bundle scripts and stylesheets
// #define FREEZE_TWEETDECK_RESOURCES

using System.Collections.Specialized;
using CefSharp;
using CefSharp.Handler;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Utils;

#if FREEZE_TWEETDECK_RESOURCES
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
#endif

namespace TweetDuck.Core.Handling{
    class RequestHandlerBase : DefaultRequestHandler{
        private readonly bool autoReload;

        public RequestHandlerBase(bool autoReload){
            this.autoReload = autoReload;
        }

        public override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture){
            return LifeSpanHandler.HandleLinkClick(browserControl, targetDisposition, targetUrl);
        }

        public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback){
            if (BrowserUtils.HasDevTools){
                NameValueCollection headers = request.Headers;
                headers.Remove("x-devtools-emulate-network-conditions-client-id");
                request.Headers = headers;
            }

            return base.OnBeforeResourceLoad(browserControl, browser, frame, request, callback);
        }
        
        public override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status){
            if (autoReload){
                browser.Reload();
            }
        }

        #if FREEZE_TWEETDECK_RESOURCES
        private static readonly Regex TweetDeckResourceUrl = new Regex(@"/dist/(.*?)\.(.*?)\.(css|js)$", RegexOptions.Compiled);
        
        private static readonly SortedList<string, string> TweetDeckHashes = new SortedList<string, string>(2){
            { "vendor.js", "d897f6b9ed" },
            { "bundle.js", "851d3877b9" },
            { "vendor.css", "ce7cdd10b6" },
            { "bundle.css", "c339f07047" }
        };

        public override bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response){
            if (request.ResourceType == ResourceType.Script || request.ResourceType == ResourceType.Stylesheet){
                string url = request.Url;
                Match match = TweetDeckResourceUrl.Match(url);

                if (match.Success && TweetDeckHashes.TryGetValue($"{match.Groups[1]}.{match.Groups[3]}", out string hash)){
                    if (match.Groups[2].Value == hash){
                        Debug.WriteLine($"Accepting {url}");
                    }
                    else{
                        Debug.WriteLine($"Rewriting {url} hash to {hash}");
                        request.Url = TweetDeckResourceUrl.Replace(url, $"/dist/$1.{hash}.$3");
                        return true;
                    }
                }
            }

            return base.OnResourceResponse(browserControl, browser, frame, request, response);
        }
        #endif
    }
}
