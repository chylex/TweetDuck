﻿using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CefSharp.WinForms;
using TweetDuck.Configuration;
using TweetDuck.Core.Other;
using TweetLib.Core.Utils;

namespace TweetDuck.Core.Utils{
    static class BrowserUtils{
        public static string UserAgentVanilla => Program.BrandName + " " + Application.ProductVersion;
        public static string UserAgentChrome => "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/" + Cef.ChromiumVersion + " Safari/537.36";

        public static readonly bool HasDevTools = File.Exists(Path.Combine(Program.ProgramPath, "devtools_resources.pak"));

        private static UserConfig Config => Program.Config.User;
        private static SystemConfig SysConfig => Program.Config.System;
        
        public static void SetupCefArgs(IDictionary<string, string> args){
            if (!SysConfig.HardwareAcceleration){
                args["disable-gpu"] = "1";
                args["disable-gpu-vsync"] = "1";
            }

            if (Config.EnableSmoothScrolling){
                args["disable-threaded-scrolling"] = "1";

                if (args.TryGetValue("disable-features", out string disabledFeatures)){
                    args["disable-features"] = "TouchpadAndWheelScrollLatching,"+disabledFeatures;
                }
                else{
                    args["disable-features"] = "TouchpadAndWheelScrollLatching";
                }
            }
            else{
                args["disable-smooth-scrolling"] = "1";
            }

            if (!Config.EnableTouchAdjustment){
                args["disable-touch-adjustment"] = "1";
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

        public static void SetupZoomEvents(this ChromiumWebBrowser browser){
            void UpdateZoomLevel(object sender, EventArgs args){
                SetZoomLevel(browser.GetBrowser(), Config.ZoomLevel);
            }

            Config.ZoomLevelChanged += UpdateZoomLevel;
            browser.Disposed += (sender, args) => Config.ZoomLevelChanged -= UpdateZoomLevel;

            browser.FrameLoadStart += (sender, args) => {
                if (args.Frame.IsMain && Config.ZoomLevel != 100){
                    SetZoomLevel(args.Browser, Config.ZoomLevel);
                }
            };
        }

        public static void OpenExternalBrowser(string url){
            if (string.IsNullOrWhiteSpace(url))return;

            switch(UrlUtils.Check(url)){
                case UrlUtils.CheckResult.Fine:
                    if (FormGuide.CheckGuideUrl(url, out string hash)){
                        FormGuide.Show(hash);
                    }
                    else{
                        string browserPath = Config.BrowserPath;

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

                case UrlUtils.CheckResult.Tracking:
                    if (Config.IgnoreTrackingUrlWarning){
                        goto case UrlUtils.CheckResult.Fine;
                    }

                    using(FormMessage form = new FormMessage("Blocked URL", "TweetDuck has blocked a tracking url due to privacy concerns. Do you want to visit it anyway?\n"+url, MessageBoxIcon.Warning)){
                        form.AddButton(FormMessage.No, DialogResult.No, ControlType.Cancel | ControlType.Focused);
                        form.AddButton(FormMessage.Yes, DialogResult.Yes, ControlType.Accept);
                        form.AddButton("Always Visit", DialogResult.Ignore);

                        DialogResult result = form.ShowDialog();

                        if (result == DialogResult.Ignore){
                            Config.IgnoreTrackingUrlWarning = true;
                            Config.Save();
                        }

                        if (result == DialogResult.Ignore || result == DialogResult.Yes){
                            goto case UrlUtils.CheckResult.Fine;
                        }
                    }

                    break;

                case UrlUtils.CheckResult.Invalid:
                    FormMessage.Warning("Blocked URL", "A potentially malicious URL was blocked from opening:\n"+url, FormMessage.OK);
                    break;
            }
        }

        public static void OpenExternalSearch(string query){
            if (string.IsNullOrWhiteSpace(query))return;
            
            string searchUrl = Config.SearchEngineUrl;
            
            if (string.IsNullOrEmpty(searchUrl)){
                if (FormMessage.Question("Search Options", "You have not configured a default search engine yet, would you like to do it now?", FormMessage.Yes, FormMessage.No)){
                    bool wereSettingsOpen = FormManager.TryFind<FormSettings>() != null;

                    FormManager.TryFind<FormBrowser>()?.OpenSettings();
                    if (wereSettingsOpen)return;

                    FormSettings settings = FormManager.TryFind<FormSettings>();
                    if (settings == null)return;

                    settings.FormClosed += (sender, args) => {
                        if (args.CloseReason == CloseReason.UserClosing && Config.SearchEngineUrl != searchUrl){
                            OpenExternalSearch(query);
                        }
                    };
                }
            }
            else{
                OpenExternalBrowser(searchUrl+Uri.EscapeUriString(query));
            }
        }

        public static string GetErrorName(CefErrorCode code){
            return StringUtils.ConvertPascalCaseToScreamingSnakeCase(Enum.GetName(typeof(CefErrorCode), code) ?? string.Empty);
        }

        public static int Scale(int baseValue, double scaleFactor){
            return (int)Math.Round(baseValue*scaleFactor);
        }

        public static void SetZoomLevel(IBrowser browser, int percentage){
            browser.GetHost().SetZoomLevel(Math.Log(percentage/100.0, 1.2));
        }
    }
}
