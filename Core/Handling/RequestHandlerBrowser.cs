using CefSharp;

namespace TweetDuck.Core.Handling{
    class RequestHandlerBrowser : RequestHandler{
        public override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status){
            browser.Reload();
        }

        public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback){
            if (request.ResourceType == ResourceType.Script && request.Url.Contains("analytics.")){
                return CefReturnValue.Cancel;
            }

            return CefReturnValue.Continue;
        }
    }
}
