using CefSharp;

namespace TweetDick.Core.Handling{
    class ContextMenuHandler : IContextMenuHandler{
        private const int MenuSettings = 26500;
        private const int MenuAbout = 26501;

        private readonly FormBrowser form;

        public ContextMenuHandler(FormBrowser form){
            this.form = form;
        }

        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Remove(CefMenuCommand.Back);
            model.Remove(CefMenuCommand.Forward);
            model.Remove(CefMenuCommand.Print);
            model.Remove(CefMenuCommand.ViewSource);

            if (model.Count > 0 && model.GetTypeAt(model.Count-1) == MenuItemType.Separator){
                model.RemoveAt(model.Count-1);
            }
            
            model.AddItem(CefMenuCommand.Reload,"Reload");
            model.AddSeparator();
            model.AddItem((CefMenuCommand)MenuSettings,"Settings");
            model.AddSeparator();
            model.AddItem((CefMenuCommand)MenuAbout,"About TweetDick");
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            switch((int)commandId){
                case MenuSettings:
                    form.OpenSettings();
                    return true;

                case MenuAbout:
                    form.OpenAbout();
                    return true;
            }

            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame){}

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback){
            return false;
        }
    }
}
