using CefSharp;
using TweetDuck.Browser.Handling.Filters;
using TweetDuck.Utils;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Browser.Handling{
    class ResourceRequestHandlerBrowser : ResourceRequestHandlerBase{
        private const string UrlVendorResource = "/dist/vendor";
        private const string UrlLoadingSpinner = "/backgrounds/spinner_blue";
        private const string UrlVersionCheck = "/web/dist/version.json";

        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback){
            if (request.ResourceType == ResourceType.MainFrame){
                if (request.Url.EndsWith("//twitter.com/")){
                    request.Url = TwitterUrls.TweetDeck; // redirect plain twitter.com requests, fixes bugs with login 2FA
                }
            }
            else if (request.ResourceType == ResourceType.Image){
                if (request.Url.Contains(UrlLoadingSpinner)){
                    request.Url = TwitterUtils.LoadingSpinner.Url;
                }
            }
            else if (request.ResourceType == ResourceType.Script){
                string url = request.Url;

                if (url.Contains("analytics.")){
                    callback.Dispose();
                    return CefReturnValue.Cancel;
                }
                else if (url.Contains(UrlVendorResource)){
                    request.SetHeaderByName("Accept-Encoding", "identity", overwrite: true);
                }
            }
            else if (request.ResourceType == ResourceType.Xhr){
                if (request.Url.Contains(UrlVersionCheck)){
                    callback.Dispose();
                    return CefReturnValue.Cancel;
                }
            }

            return base.OnBeforeResourceLoad(browserControl, browser, frame, request, callback);
        }

        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response){
            if (request.ResourceType == ResourceType.Script && request.Url.Contains(UrlVendorResource) && int.TryParse(response.Headers["Content-Length"], out int totalBytes)){
                return new ResponseFilterVendor(totalBytes);
            }

            return base.GetResourceResponseFilter(browserControl, browser, frame, request, response);
        }
    }
}
