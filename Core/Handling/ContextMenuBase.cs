using CefSharp;
using System.IO;
using System.Windows.Forms;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Handling{
    abstract class ContextMenuBase : IContextMenuHandler{
        private const int MenuOpenUrlInBrowser = 26500;
        private const int MenuCopyUrl = 26501;
        private const int MenuOpenImageInBrowser = 26502;
        private const int MenuSaveImage = 26503;
        private const int MenuCopyImageUrl = 26504;

        public virtual void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            RemoveSeparatorIfLast(model);

            if (parameters.TypeFlags.HasFlag(ContextMenuType.Link)){
                model.AddItem((CefMenuCommand)MenuOpenUrlInBrowser,"Open in browser");
                model.AddItem((CefMenuCommand)MenuCopyUrl,"Copy link address");
                model.AddSeparator();
            }

            if (parameters.TypeFlags.HasFlag(ContextMenuType.Media) && parameters.HasImageContents){
                model.AddItem((CefMenuCommand)MenuOpenImageInBrowser,"Open image in browser");
                model.AddItem((CefMenuCommand)MenuSaveImage,"Save image as...");
                model.AddItem((CefMenuCommand)MenuCopyImageUrl,"Copy image URL");
                model.AddSeparator();
            }
        }

        public virtual bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            switch((int)commandId){
                case MenuOpenUrlInBrowser:
                    BrowserUtils.OpenExternalBrowser(parameters.LinkUrl);
                    break;

                case MenuCopyUrl:
                    Clipboard.SetText(parameters.UnfilteredLinkUrl,TextDataFormat.Text);
                    break;

                case MenuOpenImageInBrowser:
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
                        BrowserUtils.DownloadFileAsync(parameters.SourceUrl,saveTarget,ex => {
                            MessageBox.Show("An error occurred while downloading the image: "+ex.Message,Program.BrandName+" Has Failed :(",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        });
                    }

                    break;

                case MenuCopyImageUrl:
                    Clipboard.SetText(parameters.SourceUrl,TextDataFormat.Text);
                    break;
            }

            return false;
        }

        public virtual void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame){}

        public virtual bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback){
            return false;
        }

        protected void RemoveSeparatorIfFirst(IMenuModel model){
            if (model.Count > 0 && model.GetTypeAt(model.Count-1) == MenuItemType.Separator){
                model.RemoveAt(model.Count-1);
            }
        }

        protected void RemoveSeparatorIfLast(IMenuModel model){
            if (model.Count > 0 && model.GetTypeAt(model.Count-1) == MenuItemType.Separator){
                model.RemoveAt(model.Count-1);
            }
        }

        private static string GetImageFileName(string url){
            // twimg adds a colon after file extension
            int dot = url.LastIndexOf('.');

            if (dot != -1){
                int colon = url.IndexOf(':',dot);
            
                if (colon != -1){
                    url = url.Substring(0,colon);
                }
            }

            // return file name
            return BrowserUtils.GetFileNameFromUrl(url) ?? "unknown";
        }
    }
}
