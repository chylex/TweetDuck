﻿using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CefSharp.WinForms;
using TweetDuck.Core.Other;

namespace TweetDuck.Core.Utils{
    static class BrowserUtils{
        public static string HeaderAcceptLanguage{
            get{
                string culture = Program.Culture.Name;

                if (culture == "en"){
                    return "en-us,en";
                }
                else{
                    return culture.ToLower()+",en;q=0.9";
                }
            }
        }

        public static string HeaderUserAgent => Program.BrandName+" "+Application.ProductVersion;

        public static void SetupCefArgs(IDictionary<string, string> args){
            if (!Program.SystemConfig.HardwareAcceleration){
                args["disable-gpu"] = "1";
                args["disable-gpu-vsync"] = "1";
            }
            
            args["disable-extensions"] = "1";
            args["disable-plugins-discovery"] = "1";
            args["enable-system-flash"] = "0";

            if (args.TryGetValue("js-flags", out string jsFlags)){
                args["js-flags"] = "--expose-gc "+jsFlags;
            }
            else{
                args["js-flags"] = "--expose-gc";
            }
        }

        public static ChromiumWebBrowser AsControl(this IWebBrowser browserControl){
            return (ChromiumWebBrowser)browserControl;
        }

        private const string TwitterTrackingUrl = "t.co";

        public enum UrlCheckResult{
            Invalid, Tracking, Fine
        }

        public static UrlCheckResult CheckUrl(string url){
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri)){
                string scheme = uri.Scheme;

                if (scheme == Uri.UriSchemeHttps || scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeFtp || scheme == Uri.UriSchemeMailto){
                    return uri.Host == TwitterTrackingUrl ? UrlCheckResult.Tracking : UrlCheckResult.Fine;
                }
            }

            return UrlCheckResult.Invalid;
        }

        public static void OpenExternalBrowser(string url){
            if (string.IsNullOrWhiteSpace(url))return;

            switch(CheckUrl(url)){
                case UrlCheckResult.Fine:
                    WindowsUtils.OpenAssociatedProgram(url);
                    break;

                case UrlCheckResult.Tracking:
                    if (FormMessage.Warning("Blocked URL", "TweetDuck has blocked a tracking url due to privacy concerns. Do you want to visit it anyway?\n"+url, FormMessage.Yes, FormMessage.No)){
                        WindowsUtils.OpenAssociatedProgram(url);
                    }

                    break;

                case UrlCheckResult.Invalid:
                    FormMessage.Warning("Blocked URL", "A potentially malicious URL was blocked from opening:\n"+url, FormMessage.OK);
                    break;
            }
        }

        public static string GetFileNameFromUrl(string url){
            string file = Path.GetFileName(new Uri(url).AbsolutePath);
            return string.IsNullOrEmpty(file) ? null : file;
        }

        public static string GetErrorName(CefErrorCode code){
            return StringUtils.ConvertPascalCaseToScreamingSnakeCase(Enum.GetName(typeof(CefErrorCode), code) ?? string.Empty);
        }

        public static WebClient CreateWebClient(){
            WebClient client = new WebClient{ Proxy = null };
            client.Headers[HttpRequestHeader.UserAgent] = HeaderUserAgent;
            return client;
        }

        public static WebClient DownloadFileAsync(string url, string target, Action onSuccess, Action<Exception> onFailure){
            WebClient client = CreateWebClient();

            client.DownloadFileCompleted += (sender, args) => {
                if (args.Cancelled){
                    try{
                        File.Delete(target);
                    }catch{
                        // didn't want it deleted anyways
                    }
                }
                else if (args.Error != null){
                    onFailure?.Invoke(args.Error);
                }
                else{
                    onSuccess?.Invoke();
                }
            };

            client.DownloadFileAsync(new Uri(url), target);
            return client;
        }

        public static int Scale(int baseValue, double scaleFactor){
            return (int)Math.Round(baseValue*scaleFactor);
        }

        public static void SetZoomLevel(IBrowser browser, int percentage){
            browser.GetHost().SetZoomLevel(Math.Log(percentage/100.0, 1.2));
        }

        #if DEBUG
        public static void HandleConsoleMessage(object sender, ConsoleMessageEventArgs e){
            Debug.WriteLine("[Console] {0} ({1}:{2})", e.Message, e.Source, e.Line);
        }
        #endif
    }
}
