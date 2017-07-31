using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Handling{
    abstract class ContextMenuBase : IContextMenuHandler{
        protected static readonly bool HasDevTools = File.Exists(Path.Combine(Program.ProgramPath, "devtools_resources.pak"));

        private static TwitterUtils.ImageQuality ImageQuality => Program.UserConfig.TwitterImageQuality;

        private static string GetLink(IContextMenuParams parameters){
            return string.IsNullOrEmpty(TweetDeckBridge.LastRightClickedLink) ? parameters.UnfilteredLinkUrl : TweetDeckBridge.LastRightClickedLink;
        }

        private static string GetImage(IContextMenuParams parameters){
            return string.IsNullOrEmpty(TweetDeckBridge.LastRightClickedImage) ? parameters.SourceUrl : TweetDeckBridge.LastRightClickedImage;
        }

        private const int MenuOpenLinkUrl = 26500;
        private const int MenuCopyLinkUrl = 26501;
        private const int MenuCopyUsername = 26502;
        private const int MenuOpenImageUrl = 26503;
        private const int MenuCopyImageUrl = 26504;
        private const int MenuSaveImage = 26505;
        private const int MenuSaveAllImages = 26506;
        private const int MenuOpenDevTools = 26599;

        private readonly Form form;
        
        private string[] lastHighlightedTweetImageList;

        protected ContextMenuBase(Form form){
            this.form = form;
        }

        public virtual void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            bool hasTweetImage = !string.IsNullOrEmpty(TweetDeckBridge.LastRightClickedImage);
            lastHighlightedTweetImageList = TweetDeckBridge.LastHighlightedTweetImages;

            if (!TwitterUtils.IsTweetDeckWebsite(frame) || browser.IsLoading){
                lastHighlightedTweetImageList = StringUtils.EmptyArray;
            }

            if (parameters.TypeFlags.HasFlag(ContextMenuType.Link) && !parameters.UnfilteredLinkUrl.EndsWith("tweetdeck.twitter.com/#", StringComparison.Ordinal) && !hasTweetImage){
                if (TwitterUtils.RegexAccount.IsMatch(parameters.UnfilteredLinkUrl)){
                    model.AddItem((CefMenuCommand)MenuOpenLinkUrl, "Open account in browser");
                    model.AddItem((CefMenuCommand)MenuCopyLinkUrl, "Copy account address");
                    model.AddItem((CefMenuCommand)MenuCopyUsername, "Copy account username");
                }
                else{
                    model.AddItem((CefMenuCommand)MenuOpenLinkUrl, "Open link in browser");
                    model.AddItem((CefMenuCommand)MenuCopyLinkUrl, "Copy link address");
                }

                model.AddSeparator();
            }

            if ((parameters.TypeFlags.HasFlag(ContextMenuType.Media) && parameters.HasImageContents) || hasTweetImage){
                model.AddItem((CefMenuCommand)MenuOpenImageUrl, "Open image in browser");
                model.AddItem((CefMenuCommand)MenuCopyImageUrl, "Copy image address");
                model.AddItem((CefMenuCommand)MenuSaveImage, "Save image as...");

                if (lastHighlightedTweetImageList.Length > 1){
                    model.AddItem((CefMenuCommand)MenuSaveAllImages, "Save all images as...");
                }

                model.AddSeparator();
            }
        }

        public virtual bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            switch((int)commandId){
                case MenuOpenLinkUrl:
                    BrowserUtils.OpenExternalBrowser(parameters.LinkUrl);
                    break;

                case MenuCopyLinkUrl:
                    SetClipboardText(GetLink(parameters));
                    break;

                case MenuOpenImageUrl:
                    BrowserUtils.OpenExternalBrowser(TwitterUtils.GetImageLink(GetImage(parameters), ImageQuality));
                    break;

                case MenuSaveImage:
                    TwitterUtils.DownloadImage(GetImage(parameters), ImageQuality);
                    break;

                case MenuSaveAllImages:
                    TwitterUtils.DownloadImages(lastHighlightedTweetImageList, ImageQuality);
                    break;

                case MenuCopyImageUrl:
                    SetClipboardText(TwitterUtils.GetImageLink(GetImage(parameters), ImageQuality));
                    break;

                case MenuCopyUsername:
                    Match match = TwitterUtils.RegexAccount.Match(parameters.UnfilteredLinkUrl);
                    SetClipboardText(match.Success ? match.Groups[1].Value : parameters.UnfilteredLinkUrl);
                    break;
                    
                case MenuOpenDevTools:
                    browserControl.ShowDevTools();
                    break;
            }

            return false;
        }

        public virtual void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame){
            TweetDeckBridge.LastRightClickedLink = string.Empty;
            TweetDeckBridge.LastRightClickedImage = string.Empty;
        }

        public virtual bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback){
            return false;
        }

        protected void SetClipboardText(string text){
            form.InvokeAsyncSafe(() => WindowsUtils.SetClipboard(text, TextDataFormat.UnicodeText));
        }
        
        protected static void AddDebugMenuItems(IMenuModel model){
            model.AddItem((CefMenuCommand)MenuOpenDevTools, "Open dev tools");
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
