using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;
using System.Linq;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Management;
using TweetDuck.Core.Notification;
using TweetDuck.Core.Other;
using TweetDuck.Core.Other.Analytics;
using TweetDuck.Resources;

namespace TweetDuck.Core.Handling{
    abstract class ContextMenuBase : IContextMenuHandler{
        public static readonly bool HasDevTools = File.Exists(Path.Combine(Program.ProgramPath, "devtools_resources.pak"));

        private static TwitterUtils.ImageQuality ImageQuality => Program.UserConfig.TwitterImageQuality;
        
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
        
        protected ContextInfo.LinkInfo LastLink { get; private set; }
        protected ContextInfo.ChirpInfo LastChirp { get; private set; }

        private readonly AnalyticsFile.IProvider analytics;

        protected ContextMenuBase(AnalyticsFile.IProvider analytics){
            this.analytics = analytics;
        }

        private void ResetContextInfo(){
            LastLink = default(ContextInfo.LinkInfo);
            LastChirp = default(ContextInfo.ChirpInfo);
            TweetDeckBridge.ContextInfo.Reset();
        }

        public virtual void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            if (!TwitterUtils.IsTweetDeckWebsite(frame) || browser.IsLoading){
                ResetContextInfo();
            }
            else{
                LastLink = TweetDeckBridge.ContextInfo.Link;
                LastChirp = TweetDeckBridge.ContextInfo.Chirp;
            }

            if (parameters.TypeFlags.HasFlag(ContextMenuType.Selection) && !parameters.TypeFlags.HasFlag(ContextMenuType.Editable)){
                model.AddItem(MenuSearchInBrowser, "Search in browser");
                model.AddSeparator();
                model.AddItem(MenuReadApplyROT13, "Apply ROT13");
                model.AddSeparator();
            }

            bool hasTweetImage = LastLink.IsImage;
            bool hasTweetVideo = LastLink.IsVideo;

            string TextOpen(string name) => "Open "+name+" in browser";
            string TextCopy(string name) => "Copy "+name+" address";
            string TextSave(string name) => "Save "+name+" as...";
            
            if (parameters.TypeFlags.HasFlag(ContextMenuType.Link) && !parameters.UnfilteredLinkUrl.EndsWith("tweetdeck.twitter.com/#", StringComparison.Ordinal) && !hasTweetImage && !hasTweetVideo){
                if (TwitterUtils.RegexAccount.IsMatch(parameters.UnfilteredLinkUrl)){
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

            if (hasTweetVideo){
                model.AddItem(MenuOpenMediaUrl, TextOpen("video"));
                model.AddItem(MenuCopyMediaUrl, TextCopy("video"));
                model.AddItem(MenuSaveMedia, TextSave("video"));
                model.AddSeparator();
            }
            else if (((parameters.TypeFlags.HasFlag(ContextMenuType.Media) && parameters.HasImageContents) || hasTweetImage) && parameters.SourceUrl != TweetNotification.AppLogo.Url){
                model.AddItem(MenuViewImage, "View image in photo viewer");
                model.AddItem(MenuOpenMediaUrl, TextOpen("image"));
                model.AddItem(MenuCopyMediaUrl, TextCopy("image"));
                model.AddItem(MenuSaveMedia, TextSave("image"));

                if (LastChirp.Images.Length > 1){
                    model.AddItem(MenuSaveTweetImages, TextSave("all images"));
                }

                model.AddSeparator();
            }
        }

        public virtual bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            Control control = browserControl.AsControl();

            switch(commandId){
                case MenuOpenLinkUrl:
                    OpenBrowser(control, LastLink.GetUrl(parameters, true));
                    break;

                case MenuCopyLinkUrl:
                    SetClipboardText(control, LastLink.GetUrl(parameters, false));
                    break;

                case MenuCopyUsername:
                    Match match = TwitterUtils.RegexAccount.Match(parameters.UnfilteredLinkUrl);
                    SetClipboardText(control, match.Success ? match.Groups[1].Value : parameters.UnfilteredLinkUrl);
                    control.InvokeAsyncSafe(analytics.AnalyticsFile.CopiedUsernames.Trigger);
                    break;

                case MenuOpenMediaUrl:
                    OpenBrowser(control, TwitterUtils.GetMediaLink(LastLink.GetMediaSource(parameters), ImageQuality));
                    break;

                case MenuCopyMediaUrl:
                    SetClipboardText(control, TwitterUtils.GetMediaLink(LastLink.GetMediaSource(parameters), ImageQuality));
                    break;

                case MenuViewImage:
                    void ViewImage(string path){
                        string ext = Path.GetExtension(path);

                        if (TwitterUtils.ValidImageExtensions.Contains(ext)){
                            WindowsUtils.OpenAssociatedProgram(path);
                        }
                        else{
                            FormMessage.Error("Image Download", "Invalid file extension "+ext, FormMessage.OK);
                        }
                    }

                    string url = LastLink.GetMediaSource(parameters);
                    string file = Path.Combine(BrowserCache.CacheFolder, TwitterUtils.GetImageFileName(url) ?? Path.GetRandomFileName());

                    if (File.Exists(file)){
                        ViewImage(file);
                    }
                    else{
                        control.InvokeAsyncSafe(analytics.AnalyticsFile.ViewedImages.Trigger);

                        BrowserUtils.DownloadFileAsync(TwitterUtils.GetMediaLink(url, ImageQuality), file, () => {
                            ViewImage(file);
                        }, ex => {
                            FormMessage.Error("Image Download", "An error occurred while downloading the image: "+ex.Message, FormMessage.OK);
                        });
                    }

                    break;

                case MenuSaveMedia:
                    if (LastLink.IsVideo){
                        control.InvokeAsyncSafe(analytics.AnalyticsFile.DownloadedVideos.Trigger);
                        TwitterUtils.DownloadVideo(LastLink.GetMediaSource(parameters), LastChirp.Authors.LastOrDefault());
                    }
                    else{
                        control.InvokeAsyncSafe(analytics.AnalyticsFile.DownloadedImages.Trigger);
                        TwitterUtils.DownloadImage(LastLink.GetMediaSource(parameters), LastChirp.Authors.LastOrDefault(), ImageQuality);
                    }

                    break;

                case MenuSaveTweetImages:
                    control.InvokeAsyncSafe(analytics.AnalyticsFile.DownloadedImages.Trigger);
                    TwitterUtils.DownloadImages(LastChirp.Images, LastChirp.Authors.LastOrDefault(), ImageQuality);
                    break;

                case MenuReadApplyROT13:
                    string selection = parameters.SelectionText;
                    control.InvokeAsyncSafe(() => FormMessage.Information("ROT13", StringUtils.ConvertRot13(selection), FormMessage.OK));
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
            ResetContextInfo();
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
            model.AddItem(MenuOpenDevTools, "Open dev tools");
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
