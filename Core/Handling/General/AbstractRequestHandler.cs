using System.Security.Cryptography.X509Certificates;
using CefSharp;

namespace TweetDuck.Core.Handling.General{
    abstract class AbstractRequestHandler : IRequestHandler{
        // Browser

        public virtual void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser){}

        public virtual void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status){}

        public virtual bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect){
            return false;
        }

        public virtual bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture){
            return false;
        }

        public virtual bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url){
            return false;
        }

        // Resources

        public virtual CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback){
            return CefReturnValue.Continue;
        }

        public virtual void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl){}

        public virtual bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response){
            return false;
        }

        public virtual IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response){
            return null;
        }

        public virtual void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength){}

        // JavaScript & Plugins

        public virtual bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback){
            callback.Dispose();
            return false;
        }

        public virtual void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath){}

        // Auth

        public virtual bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback){
            callback.Dispose();
            return false;
        }

        public virtual bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback){
            callback.Dispose();
            return false;
        }

        public virtual bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback){
            callback.Dispose();
            return false;
        }
    }
}
