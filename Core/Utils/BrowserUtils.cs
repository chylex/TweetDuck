﻿using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CefSharp.WinForms;
using TweetDuck.Core.Other;
using TweetDuck.Data;

namespace TweetDuck.Core.Utils{
    static class BrowserUtils{
        public static string UserAgentVanilla => Program.BrandName+" "+Application.ProductVersion;
        public static string UserAgentChrome => "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/"+Cef.ChromiumVersion+" Safari/537.36";
        
        public static void SetupCefArgs(IDictionary<string, string> args){
            if (!Program.SystemConfig.HardwareAcceleration){
                args["disable-gpu"] = "1";
                args["disable-gpu-vsync"] = "1";
            }

            if (!Program.UserConfig.EnableSmoothScrolling){
                args["disable-smooth-scrolling"] = "1";
            }
            
            args["disable-pdf-extension"] = "1";
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

        public static void SetupResourceHandler(this ChromiumWebBrowser browser, string url, IResourceHandler handler){
            DefaultResourceHandlerFactory factory = (DefaultResourceHandlerFactory)browser.ResourceHandlerFactory;

            if (handler == null){
                factory.UnregisterHandler(url);
            }
            else{
                factory.RegisterHandler(url, handler);
            }
        }

        public static void SetupResourceHandler(this ChromiumWebBrowser browser, ResourceLink resource){
            browser.SetupResourceHandler(resource.Url, resource.Handler);
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
                    if (FormGuide.CheckGuideUrl(url, out string hash)){
                        FormGuide.Show(hash);
                    }
                    else{
                        string browserPath = Program.UserConfig.BrowserPath;

                        if (browserPath == null || !File.Exists(browserPath)){
                            WindowsUtils.OpenAssociatedProgram(url);
                        }
                        else{
                            try{
                                using(Process.Start(browserPath, url)){}
                            }catch(Exception e){
                                Program.Reporter.HandleException("Error Opening Browser", "Could not open the browser.", true, e);
                            }
                        }
                    }

                    break;

                case UrlCheckResult.Tracking:
                    if (Program.UserConfig.IgnoreTrackingUrlWarning){
                        goto case UrlCheckResult.Fine;
                    }

                    using(FormMessage form = new FormMessage("Blocked URL", "TweetDuck has blocked a tracking url due to privacy concerns. Do you want to visit it anyway?\n"+url, MessageBoxIcon.Warning)){
                        form.AddButton(FormMessage.No, DialogResult.No, ControlType.Cancel | ControlType.Focused);
                        form.AddButton(FormMessage.Yes, DialogResult.Yes, ControlType.Accept);
                        form.AddButton("Always Visit", DialogResult.Ignore);

                        DialogResult result = form.ShowDialog();

                        if (result == DialogResult.Ignore){
                            Program.UserConfig.IgnoreTrackingUrlWarning = true;
                            Program.UserConfig.Save();
                        }

                        if (result == DialogResult.Ignore || result == DialogResult.Yes){
                            goto case UrlCheckResult.Fine;
                        }
                    }

                    break;

                case UrlCheckResult.Invalid:
                    FormMessage.Warning("Blocked URL", "A potentially malicious URL was blocked from opening:\n"+url, FormMessage.OK);
                    break;
            }
        }

        public static void OpenExternalSearch(string query){
            if (string.IsNullOrWhiteSpace(query))return;
            
            string searchUrl = Program.UserConfig.SearchEngineUrl;
            
            if (string.IsNullOrEmpty(searchUrl)){
                if (FormMessage.Question("Search Options", "You have not configured a default search engine yet, would you like to do it now?", FormMessage.Yes, FormMessage.No)){
                    bool wereSettingsOpen = FormManager.TryFind<FormSettings>() != null;

                    FormManager.TryFind<FormBrowser>()?.OpenSettings();
                    if (wereSettingsOpen)return;

                    FormSettings settings = FormManager.TryFind<FormSettings>();
                    if (settings == null)return;

                    settings.FormClosed += (sender, args) => {
                        if (args.CloseReason == CloseReason.UserClosing && Program.UserConfig.SearchEngineUrl != searchUrl){
                            OpenExternalSearch(query);
                        }
                    };
                }
            }
            else{
                OpenExternalBrowser(searchUrl+Uri.EscapeUriString(query));
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
            WindowsUtils.EnsureTLS12();

            WebClient client = new WebClient{ Proxy = null };
            client.Headers[HttpRequestHeader.UserAgent] = UserAgentVanilla;
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
    }
}
