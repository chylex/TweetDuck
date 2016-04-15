using CefSharp;

namespace TweetDck.Core.Handling{
    class ContextMenuBrowser : IContextMenuHandler{
        private const int MenuSettings = 26500;
        private const int MenuAbout = 26501;

        private readonly FormBrowser form;

        public ContextMenuBrowser(FormBrowser form){
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

            if (TweetNotification.IsReady){
                model.AddItem((CefMenuCommand)MenuSettings,"Settings");
                model.AddSeparator();
            }

            model.AddItem((CefMenuCommand)MenuAbout,"About "+Program.BrandName);
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            switch((int)commandId){
                case MenuSettings:
                    form.InvokeSafe(() => {
                        form.OpenSettings();
                    });

                    return true;

                case MenuAbout:
                    form.InvokeSafe(() => {
                        form.OpenAbout();
                    });

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
