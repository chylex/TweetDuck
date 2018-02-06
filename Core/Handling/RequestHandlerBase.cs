using System.Collections.Specialized;
using CefSharp;
using CefSharp.Handler;
using TweetDuck.Core.Handling.General;

namespace TweetDuck.Core.Handling{
    class RequestHandlerBase : DefaultRequestHandler{
        public override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture){
            return LifeSpanHandler.HandleLinkClick(browserControl, targetDisposition, targetUrl);
        }

        public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback){
            if (ContextMenuBase.HasDevTools){
                NameValueCollection headers = request.Headers;
                headers.Remove("x-devtools-emulate-network-conditions-client-id");
                request.Headers = headers;
            }

            return base.OnBeforeResourceLoad(browserControl, browser, frame, request, callback);
        }
    }
}
