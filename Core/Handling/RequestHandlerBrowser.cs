using System.Text.RegularExpressions;
using CefSharp;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Handling{
    sealed class RequestHandlerBrowser : RequestHandlerBase{
        private static readonly Regex TmpResourceRedirect = new Regex(@"dist\/(.*?)\.(.*?)\.js$", RegexOptions.Compiled);

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
            else if (request.ResourceType == ResourceType.Script){ // TODO delaying the apocalypse
                Match match = TmpResourceRedirect.Match(request.Url);

                if (match.Success){
                    string scriptType = match.Groups[1].Value;
                    string scriptHash = match.Groups[2].Value;

                    const string HashBundle = "1bd75b5854";
                    const string HashVendor = "942c0a20e8";

                    if (scriptType == "bundle" && scriptHash != HashBundle){
                        request.Url = TmpResourceRedirect.Replace(request.Url, "dist/$1."+HashBundle+".js");
                        System.Diagnostics.Debug.WriteLine("rewriting "+scriptType+" to "+request.Url);
                        return true;
                    }
                    else if (scriptType == "vendor" && scriptHash != HashVendor){
                        request.Url = TmpResourceRedirect.Replace(request.Url, "dist/$1."+HashVendor+".js");
                        System.Diagnostics.Debug.WriteLine("rewriting "+scriptType+" to "+request.Url);
                        return true;
                    }

                    System.Diagnostics.Debug.WriteLine("accepting "+scriptType+" as "+request.Url);
                }
            }

            return base.OnResourceResponse(browserControl, browser, frame, request, response);
        }
    }
}
