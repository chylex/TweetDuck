using CefSharp;
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
            if (request.ResourceType == ResourceType.Image && request.Url.Contains("backgrounds/spinner_blue")){
                request.Url = TwitterUtils.LoadingSpinner.Url;
                return true;
            }

            return base.OnResourceResponse(browserControl, browser, frame, request, response);
        }
    }
}
