using System.Collections.Specialized;
using CefSharp;
using CefSharp.Handler;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Utils;

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
    }
}
