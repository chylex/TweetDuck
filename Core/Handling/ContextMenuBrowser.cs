using CefSharp;

namespace TweetDck.Core.Handling{
    class ContextMenuBrowser : ContextMenuBase{
        private const int MenuSettings = 26600;
        private const int MenuAbout = 26601;
        private const int MenuMute = 26602;

        private readonly FormBrowser form;

        public ContextMenuBrowser(FormBrowser form){
            this.form = form;
        }

        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Remove(CefMenuCommand.Back);
            model.Remove(CefMenuCommand.Forward);
            model.Remove(CefMenuCommand.Print);
            model.Remove(CefMenuCommand.ViewSource);

            RemoveSeparatorIfLast(model);
            base.OnBeforeContextMenu(browserControl,browser,frame,parameters,model);

            if (model.Count > 0){
                RemoveSeparatorIfLast(model);
                model.AddSeparator();
            }
            
            model.AddItem(CefMenuCommand.Reload,"Reload");
            model.AddCheckItem((CefMenuCommand)MenuMute,"Mute Notifications");
            model.SetChecked((CefMenuCommand)MenuMute,Program.UserConfig.MuteNotifications);
            model.AddSeparator();

            if (TweetNotification.IsReady){
                model.AddItem((CefMenuCommand)MenuSettings,"Settings");
            }

            model.AddItem((CefMenuCommand)MenuAbout,"About "+Program.BrandName);
        }

        public override bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            if (base.OnContextMenuCommand(browserControl,browser,frame,parameters,commandId,eventFlags)){
                return true;
            }

            switch((int)commandId){
                case MenuSettings:
                    form.InvokeSafe(form.OpenSettings);
                    return true;

                case MenuAbout:
                    form.InvokeSafe(form.OpenAbout);
                    return true;

                case MenuMute:
                    form.InvokeSafe(() => {
                        Program.UserConfig.MuteNotifications = !Program.UserConfig.MuteNotifications;
                        Program.UserConfig.Save();
                    });

                    return true;
            }

            return false;
        }
    }
}
