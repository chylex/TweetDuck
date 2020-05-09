using CefSharp;
using CefSharp.Handler;
using TweetDuck.Browser.Handling.General;

namespace TweetDuck.Browser.Handling{
    class RequestHandlerBase : RequestHandler{
        private readonly bool autoReload;

        public RequestHandlerBase(bool autoReload){
            this.autoReload = autoReload;
        }

        protected override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture){
            return LifeSpanHandler.HandleLinkClick(browserControl, targetDisposition, targetUrl);
        }
        
        protected override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status){
            if (autoReload){
                browser.Reload();
            }
        }
    }
}
