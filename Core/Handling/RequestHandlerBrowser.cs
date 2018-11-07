using System.Collections.Specialized;
using CefSharp;
using TweetDuck.Core.Handling.Filters;
using TweetDuck.Core.Utils;

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

        public override bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response){
            if (request.ResourceType == ResourceType.Image && request.Url.Contains("/backgrounds/spinner_blue")){
                request.Url = TwitterUtils.LoadingSpinner.Url;
                return true;
            }

            return base.OnResourceResponse(browserControl, browser, frame, request, response);
        }

        public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response){
            if (request.ResourceType == ResourceType.Script && request.Url.Contains("/dist/vendor")){
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
