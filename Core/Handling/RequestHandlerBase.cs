using CefSharp;
using CefSharp.Handler;
using TweetDuck.Core.Handling.General;

namespace TweetDuck.Core.Handling{
    class RequestHandlerBase : DefaultRequestHandler{
        public override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture){
            return LifeSpanHandler.HandleLinkClick(browserControl, targetDisposition, targetUrl);
        }
    }
}
