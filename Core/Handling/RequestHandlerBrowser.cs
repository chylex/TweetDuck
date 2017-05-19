using CefSharp;

namespace TweetDuck.Core.Handling{
    class RequestHandlerBrowser : RequestHandler{
        public override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status){
            browser.Reload();
        }
    }
}
