using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace TweetDuck.Core.Handling{
    abstract class ContextMenuBase : IContextMenuHandler{
        protected static readonly bool HasDevTools = File.Exists(Path.Combine(Program.ProgramPath, "devtools_resources.pak"));

        private static TwitterUtils.ImageQuality ImageQuality => Program.UserConfig.TwitterImageQuality;
        
        private static KeyValuePair<string, string> ContextInfo;
        private static bool IsLink => ContextInfo.Key == "link";
        private static bool IsImage => ContextInfo.Key == "image";
        private static bool IsVideo => ContextInfo.Key == "video";

        public static void SetContextInfo(string type, string link){
            ContextInfo = new KeyValuePair<string, string>(string.IsNullOrEmpty(link) ? null : type, link);
        }

        private static string GetMediaLink(IContextMenuParams parameters){
            return IsImage || IsVideo ? ContextInfo.Value : parameters.SourceUrl;
        }

        private const CefMenuCommand MenuOpenLinkUrl     = (CefMenuCommand)26500;
        private const CefMenuCommand MenuCopyLinkUrl     = (CefMenuCommand)26501;
        private const CefMenuCommand MenuCopyUsername    = (CefMenuCommand)26502;
        private const CefMenuCommand MenuOpenMediaUrl    = (CefMenuCommand)26503;
        private const CefMenuCommand MenuCopyMediaUrl    = (CefMenuCommand)26504;
        private const CefMenuCommand MenuSaveMedia       = (CefMenuCommand)26505;
        private const CefMenuCommand MenuSaveTweetImages = (CefMenuCommand)26506;
        private const CefMenuCommand MenuOpenDevTools    = (CefMenuCommand)26599;

        private readonly Form form;
        
        private string[] lastHighlightedTweetAuthors;
        private string[] lastHighlightedTweetImageList;

        protected ContextMenuBase(Form form){
            this.form = form;
        }

        public virtual void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            if (!TwitterUtils.IsTweetDeckWebsite(frame) || browser.IsLoading){
                lastHighlightedTweetAuthors = StringUtils.EmptyArray;
                lastHighlightedTweetImageList = StringUtils.EmptyArray;
                ContextInfo = default(KeyValuePair<string, string>);
            }
            else{
                lastHighlightedTweetAuthors = TweetDeckBridge.LastHighlightedTweetAuthorsArray;
                lastHighlightedTweetImageList = TweetDeckBridge.LastHighlightedTweetImagesArray;
            }

            bool hasTweetImage = IsImage;
            bool hasTweetVideo = IsVideo;

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
            else if ((parameters.TypeFlags.HasFlag(ContextMenuType.Media) && parameters.HasImageContents) || hasTweetImage){
                model.AddItem(MenuOpenMediaUrl, TextOpen("image"));
                model.AddItem(MenuCopyMediaUrl, TextCopy("image"));
                model.AddItem(MenuSaveMedia, TextSave("image"));

                if (lastHighlightedTweetImageList.Length > 1){
                    model.AddItem(MenuSaveTweetImages, TextSave("all images"));
                }

                model.AddSeparator();
            }
        }

        public virtual bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            switch(commandId){
                case MenuOpenLinkUrl:
                    BrowserUtils.OpenExternalBrowser(IsLink ? ContextInfo.Value : parameters.LinkUrl);
                    break;

                case MenuCopyLinkUrl:
                    SetClipboardText(IsLink ? ContextInfo.Value : parameters.UnfilteredLinkUrl);
                    break;

                case MenuCopyUsername:
                    Match match = TwitterUtils.RegexAccount.Match(parameters.UnfilteredLinkUrl);
                    SetClipboardText(match.Success ? match.Groups[1].Value : parameters.UnfilteredLinkUrl);
                    break;

                case MenuOpenMediaUrl:
                    BrowserUtils.OpenExternalBrowser(TwitterUtils.GetMediaLink(GetMediaLink(parameters), ImageQuality));
                    break;

                case MenuCopyMediaUrl:
                    SetClipboardText(TwitterUtils.GetMediaLink(GetMediaLink(parameters), ImageQuality));
                    break;

                case MenuSaveMedia:
                    if (IsVideo){
                        TwitterUtils.DownloadVideo(GetMediaLink(parameters));
                    }
                    else{
                        TwitterUtils.DownloadImage(GetMediaLink(parameters), lastHighlightedTweetAuthors.LastOrDefault(), ImageQuality);
                    }

                    break;

                case MenuSaveTweetImages:
                    TwitterUtils.DownloadImages(lastHighlightedTweetImageList, lastHighlightedTweetAuthors.LastOrDefault(), ImageQuality);
                    break;
                    
                case MenuOpenDevTools:
                    browserControl.ShowDevTools();
                    break;
            }

            return false;
        }

        public virtual void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame){
            ContextInfo = default(KeyValuePair<string, string>);
        }

        public virtual bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback){
            return false;
        }

        protected void SetClipboardText(string text){
            form.InvokeAsyncSafe(() => WindowsUtils.SetClipboard(text, TextDataFormat.UnicodeText));
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
