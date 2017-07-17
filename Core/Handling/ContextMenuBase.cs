using CefSharp;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Handling{
    abstract class ContextMenuBase : IContextMenuHandler{
        private static readonly Lazy<Regex> RegexTwitterAccount = new Lazy<Regex>(() => new Regex(@"^https?://twitter\.com/([^/]+)/?$", RegexOptions.Compiled), false);
        protected static readonly bool HasDevTools = File.Exists(Path.Combine(Program.ProgramPath, "devtools_resources.pak"));

        private static TwitterUtils.ImageQuality ImageQuality => Program.UserConfig.TwitterImageQuality;

        private const int MenuOpenLinkUrl = 26500;
        private const int MenuCopyLinkUrl = 26501;
        private const int MenuCopyUsername = 26502;
        private const int MenuOpenImage = 26503;
        private const int MenuSaveImage = 26504;
        private const int MenuCopyImageUrl = 26505;
        private const int MenuOpenDevTools = 26599;

        private readonly Form form;

        protected ContextMenuBase(Form form){
            this.form = form;
        }

        public virtual void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            if (parameters.TypeFlags.HasFlag(ContextMenuType.Link) && !parameters.UnfilteredLinkUrl.EndsWith("tweetdeck.twitter.com/#", StringComparison.Ordinal)){
                if (RegexTwitterAccount.Value.IsMatch(parameters.UnfilteredLinkUrl)){
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

            if (parameters.TypeFlags.HasFlag(ContextMenuType.Media) && parameters.HasImageContents){
                model.AddItem((CefMenuCommand)MenuOpenImage, "Open image in browser");
                model.AddItem((CefMenuCommand)MenuSaveImage, "Save image as...");
                model.AddItem((CefMenuCommand)MenuCopyImageUrl, "Copy image address");
                model.AddSeparator();
            }
        }

        public virtual bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            switch((int)commandId){
                case MenuOpenLinkUrl:
                    BrowserUtils.OpenExternalBrowser(parameters.LinkUrl);
                    break;

                case MenuCopyLinkUrl:
                    SetClipboardText(string.IsNullOrEmpty(TweetDeckBridge.LastRightClickedLink) ? parameters.UnfilteredLinkUrl : TweetDeckBridge.LastRightClickedLink);
                    break;

                case MenuOpenImage:
                    BrowserUtils.OpenExternalBrowser(TwitterUtils.GetImageLink(parameters.SourceUrl, ImageQuality));
                    break;

                case MenuSaveImage:
                    TwitterUtils.DownloadImage(parameters.SourceUrl, ImageQuality);
                    break;

                case MenuCopyImageUrl:
                    SetClipboardText(TwitterUtils.GetImageLink(parameters.SourceUrl, ImageQuality));
                    break;

                case MenuCopyUsername:
                    Match match = RegexTwitterAccount.Value.Match(parameters.UnfilteredLinkUrl);
                    SetClipboardText(match.Success ? match.Groups[1].Value : parameters.UnfilteredLinkUrl);
                    break;
                    
                case MenuOpenDevTools:
                    browserControl.ShowDevTools();
                    break;
            }

            return false;
        }

        public virtual void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame){}

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
