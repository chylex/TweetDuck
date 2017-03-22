using CefSharp;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TweetDck.Core.Bridge;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Handling{
    abstract class ContextMenuBase : IContextMenuHandler{
        private static readonly Regex RegexTwitterAccount = new Regex(@"^https?://twitter\.com/([^/]+)/?$", RegexOptions.Compiled);

        private const int MenuOpenLinkUrl = 26500;
        private const int MenuCopyLinkUrl = 26501;
        private const int MenuCopyUsername = 26502;
        private const int MenuOpenImage = 26503;
        private const int MenuSaveImage = 26504;
        private const int MenuCopyImageUrl = 26505;

        #if DEBUG
        private const int MenuOpenDevTools = 26599;

        protected void AddDebugMenuItems(IMenuModel model){
            model.AddItem((CefMenuCommand)MenuOpenDevTools, "Open dev tools");
        }
        #endif

        private readonly Form form;

        protected ContextMenuBase(Form form){
            this.form = form;
        }

        public virtual void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            if (parameters.TypeFlags.HasFlag(ContextMenuType.Link) && !parameters.UnfilteredLinkUrl.EndsWith("tweetdeck.twitter.com/#", StringComparison.Ordinal)){
                if (RegexTwitterAccount.IsMatch(parameters.UnfilteredLinkUrl)){
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
                    BrowserUtils.OpenExternalBrowser(parameters.SourceUrl);
                    break;

                case MenuSaveImage:
                    string fileName = GetImageFileName(parameters.SourceUrl);
                    string extension = Path.GetExtension(fileName);
                    string saveTarget;

                    using(SaveFileDialog dialog = new SaveFileDialog{
                        AutoUpgradeEnabled = true,
                        OverwritePrompt = true,
                        Title = "Save image",
                        FileName = fileName,
                        Filter = "Image ("+(string.IsNullOrEmpty(extension) ? "unknown" : extension)+")|*.*"
                    }){
                        saveTarget = dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
                    }

                    if (saveTarget != null){
                        BrowserUtils.DownloadFileAsync(parameters.SourceUrl, saveTarget, ex => {
                            MessageBox.Show("An error occurred while downloading the image: "+ex.Message, Program.BrandName+" Has Failed :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }

                    break;

                case MenuCopyImageUrl:
                    SetClipboardText(parameters.SourceUrl);
                    break;

                case MenuCopyUsername:
                    Match match = RegexTwitterAccount.Match(parameters.UnfilteredLinkUrl);
                    SetClipboardText(match.Success ? match.Groups[1].Value : parameters.UnfilteredLinkUrl);
                    break;

                #if DEBUG
                case MenuOpenDevTools:
                    browserControl.ShowDevTools();
                    break;
                #endif
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

        private static string GetImageFileName(string url){
            // twimg adds a colon after file extension
            int dot = url.LastIndexOf('.');

            if (dot != -1){
                int colon = url.IndexOf(':', dot);
            
                if (colon != -1){
                    url = url.Substring(0, colon);
                }
            }

            // return file name
            return BrowserUtils.GetFileNameFromUrl(url) ?? "unknown";
        }
    }
}
