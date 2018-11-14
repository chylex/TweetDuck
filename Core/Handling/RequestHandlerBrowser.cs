using System.Collections.Specialized;
using CefSharp;
using TweetDuck.Core.Handling.Filters;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Handling{
    sealed class RequestHandlerBrowser : RequestHandlerBase{
        private const string UrlVendorResource = "/dist/vendor";
        private const string UrlLoadingSpinner = "/backgrounds/spinner_blue";

        public string BlockNextUserNavUrl { get; set; }

        public RequestHandlerBrowser() : base(true){}

        public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback){
            if (request.ResourceType == ResourceType.Script){
                string url = request.Url;

                if (url.Contains("analytics.")){
                    callback.Dispose();
                    return CefReturnValue.Cancel;
                }
                else if (url.Contains(UrlVendorResource)){
                    NameValueCollection headers = request.Headers;
                    headers["Accept-Encoding"] = "identity";
                    request.Headers = headers;
                }
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
            if (request.ResourceType == ResourceType.Image && request.Url.Contains(UrlLoadingSpinner)){
                request.Url = TwitterUtils.LoadingSpinner.Url;
                return true;
            }

            return base.OnResourceResponse(browserControl, browser, frame, request, response);
        }

        public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response){
            if (request.ResourceType == ResourceType.Script && request.Url.Contains(UrlVendorResource) && int.TryParse(response.ResponseHeaders["Content-Length"], out int totalBytes)){
                return new ResponseFilterVendor(totalBytes);
            }

            return base.GetResourceResponseFilter(browserControl, browser, frame, request, response);
        }
    }
}
