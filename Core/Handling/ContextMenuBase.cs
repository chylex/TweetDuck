using CefSharp;
using TweetDck.Core.Utils;
using System.Windows.Forms;

namespace TweetDck.Core.Handling{
    abstract class ContextMenuBase : IContextMenuHandler{
        private const int MenuOpenUrlInBrowser = 26500;
        private const int MenuCopyUrl = 26501;
        private const int MenuOpenImageInBrowser = 26502;
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
    }
}
