﻿using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;
using System.Linq;
using TweetDuck.Configuration;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Management;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Other;
using TweetDuck.Core.Other.Analytics;
using TweetDuck.Resources;
using TweetLib.Core.Utils;

namespace TweetDuck.Core.Handling{
    abstract class ContextMenuBase : IContextMenuHandler{
        protected static UserConfig Config => Program.Config.User;

        private static TwitterUtils.ImageQuality ImageQuality => Config.TwitterImageQuality;
        
        private const CefMenuCommand MenuOpenLinkUrl     = (CefMenuCommand)26500;
        private const CefMenuCommand MenuCopyLinkUrl     = (CefMenuCommand)26501;
        private const CefMenuCommand MenuCopyUsername    = (CefMenuCommand)26502;
        private const CefMenuCommand MenuViewImage       = (CefMenuCommand)26503;
        private const CefMenuCommand MenuOpenMediaUrl    = (CefMenuCommand)26504;
        private const CefMenuCommand MenuCopyMediaUrl    = (CefMenuCommand)26505;
        private const CefMenuCommand MenuSaveMedia       = (CefMenuCommand)26506;
        private const CefMenuCommand MenuSaveTweetImages = (CefMenuCommand)26507;
        private const CefMenuCommand MenuSearchInBrowser = (CefMenuCommand)26508;
        private const CefMenuCommand MenuReadApplyROT13  = (CefMenuCommand)26509;
        private const CefMenuCommand MenuOpenDevTools    = (CefMenuCommand)26599;
        
        protected ContextInfo.ContextData Context { get; private set; }

        private readonly AnalyticsFile.IProvider analytics;

        protected ContextMenuBase(AnalyticsFile.IProvider analytics){
            this.analytics = analytics;
        }

        public virtual void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            if (!TwitterUtils.IsTweetDeckWebsite(frame) || browser.IsLoading){
                Context = TweetDeckBridge.ContextInfo.Reset();
            }
            else{
                Context = TweetDeckBridge.ContextInfo.Create(parameters);
            }
            
            if (parameters.TypeFlags.HasFlag(ContextMenuType.Selection) && !parameters.TypeFlags.HasFlag(ContextMenuType.Editable)){
                model.AddItem(MenuSearchInBrowser, "Search in browser");
                model.AddSeparator();
                model.AddItem(MenuReadApplyROT13, "Apply ROT13");
                model.AddSeparator();
            }
            
            string TextOpen(string name) => "Open "+name+" in browser";
            string TextCopy(string name) => "Copy "+name+" address";
            string TextSave(string name) => "Save "+name+" as...";
            
            if (Context.Types.HasFlag(ContextInfo.ContextType.Link) && !Context.UnsafeLinkUrl.EndsWith("tweetdeck.twitter.com/#", StringComparison.Ordinal)){
                if (TwitterUtils.RegexAccount.IsMatch(Context.UnsafeLinkUrl)){
                    model.AddItem(MenuOpenLinkUrl, TextOpen("account"));
                    model.AddItem(MenuCopyLinkUrl, TextCopy("account"));
                    model.AddItem(MenuCopyUsername, "Copy account username");
                }
                else{
                    model.AddItem(MenuOpenLinkUrl, TextOpen("link"));
                    model.AddItem(MenuCopyLinkUrl, TextCopy("link"));
                }

                model.AddSeparator();
            }

            if (Context.Types.HasFlag(ContextInfo.ContextType.Video)){
                model.AddItem(MenuOpenMediaUrl, TextOpen("video"));
                model.AddItem(MenuCopyMediaUrl, TextCopy("video"));
                model.AddItem(MenuSaveMedia, TextSave("video"));
                model.AddSeparator();
            }
            else if (Context.Types.HasFlag(ContextInfo.ContextType.Image) && Context.MediaUrl != TweetNotification.AppLogo.Url){
                model.AddItem(MenuViewImage, "View image in photo viewer");
                model.AddItem(MenuOpenMediaUrl, TextOpen("image"));
                model.AddItem(MenuCopyMediaUrl, TextCopy("image"));
                model.AddItem(MenuSaveMedia, TextSave("image"));

                if (Context.Chirp.Images.Length > 1){
                    model.AddItem(MenuSaveTweetImages, TextSave("all images"));
                }

                model.AddSeparator();
            }
        }

        public virtual bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            Control control = browserControl.AsControl();

            switch(commandId){
                case MenuOpenLinkUrl:
                    OpenBrowser(control, Context.LinkUrl);
                    break;

                case MenuCopyLinkUrl:
                    SetClipboardText(control, Context.UnsafeLinkUrl);
                    break;

                case MenuCopyUsername: {
                    string url = Context.UnsafeLinkUrl;
                    Match match = TwitterUtils.RegexAccount.Match(url);

                    SetClipboardText(control, match.Success ? match.Groups[1].Value : url);
                    control.InvokeAsyncSafe(analytics.AnalyticsFile.CopiedUsernames.Trigger);
                    break;
                }

                case MenuOpenMediaUrl:
                    OpenBrowser(control, TwitterUtils.GetMediaLink(Context.MediaUrl, ImageQuality));
                    break;

                case MenuCopyMediaUrl:
                    SetClipboardText(control, TwitterUtils.GetMediaLink(Context.MediaUrl, ImageQuality));
                    break;

                case MenuViewImage: {
                    string url = Context.MediaUrl;

                    control.InvokeAsyncSafe(() => {
                        TwitterUtils.ViewImage(url, ImageQuality);
                        analytics.AnalyticsFile.ViewedImages.Trigger();
                    });
                    
                    break;
                }

                case MenuSaveMedia: {
                    bool isVideo = Context.Types.HasFlag(ContextInfo.ContextType.Video);
                    string url = Context.MediaUrl;
                    string username = Context.Chirp.Authors.LastOrDefault();
                    
                    control.InvokeAsyncSafe(() => {
                        if (isVideo){
                            TwitterUtils.DownloadVideo(url, username);
                            analytics.AnalyticsFile.DownloadedVideos.Trigger();
                        }
                        else{
                            TwitterUtils.DownloadImage(url, username, ImageQuality);
                            analytics.AnalyticsFile.DownloadedImages.Trigger();
                        }
                    });

                    break;
                }

                case MenuSaveTweetImages: {
                    string[] urls = Context.Chirp.Images;
                    string username = Context.Chirp.Authors.LastOrDefault();
                    
                    control.InvokeAsyncSafe(() => {
                        TwitterUtils.DownloadImages(urls, username, ImageQuality);
                        analytics.AnalyticsFile.DownloadedImages.Trigger();
                    });

                    break;
                }

                case MenuReadApplyROT13:
                    string selection = parameters.SelectionText;
                    control.InvokeAsyncSafe(() => FormMessage.Information("ROT13", StringUtils.ConvertRot13(selection), FormMessage.OK));
                    control.InvokeAsyncSafe(analytics.AnalyticsFile.UsedROT13.Trigger);
                    return true;

                case MenuSearchInBrowser:
                    string query = parameters.SelectionText;
                    control.InvokeAsyncSafe(() => BrowserUtils.OpenExternalSearch(query));
                    DeselectAll(frame);
                    break;
                    
                case MenuOpenDevTools:
                    browserControl.ShowDevTools();
                    break;
            }

            return false;
        }

        public virtual void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame){
            Context = TweetDeckBridge.ContextInfo.Reset();
        }

        public virtual bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback){
            return false;
        }

        protected static void DeselectAll(IFrame frame){
            ScriptLoader.ExecuteScript(frame, "window.getSelection().removeAllRanges()", "gen:deselect");
        }

        protected static void OpenBrowser(Control control, string url){
            control.InvokeAsyncSafe(() => BrowserUtils.OpenExternalBrowser(url));
        }

        protected static void SetClipboardText(Control control, string text){
            control.InvokeAsyncSafe(() => WindowsUtils.SetClipboard(text, TextDataFormat.UnicodeText));
        }

        protected static void InsertSelectionSearchItem(IMenuModel model, CefMenuCommand insertCommand, string insertLabel){
            model.InsertItemAt(model.GetIndexOf(MenuSearchInBrowser)+1, insertCommand, insertLabel);
        }
        
        protected static void AddDebugMenuItems(IMenuModel model){
            if (BrowserUtils.HasDevTools){
                AddSeparator(model);
                model.AddItem(MenuOpenDevTools, "Open dev tools");
            }
        }

        protected static void RemoveSeparatorIfLast(IMenuModel model){
            if (model.Count > 0 && model.GetTypeAt(model.Count-1) == MenuItemType.Separator){
                model.RemoveAt(model.Count-1);
            }
        }

        protected static void AddSeparator(IMenuModel model){
            if (model.Count > 0 && model.GetTypeAt(model.Count-1) != MenuItemType.Separator){ // do not add separators if there is nothing to separate
                model.AddSeparator();
            }
        }
    }
}
