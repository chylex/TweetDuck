using CefSharp;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Handling{
    class ContextMenuNotification : ContextMenuBase{
        private const int MenuSkipTweet = 26600;
        private const int MenuFreeze = 26601;

        private readonly FormNotification form;
        private readonly bool enableCustomMenu;

        public ContextMenuNotification(FormNotification form, bool enableCustomMenu){
            this.form = form;
            this.enableCustomMenu = enableCustomMenu;
        }

        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Clear();

            if (enableCustomMenu){
                model.AddItem((CefMenuCommand)MenuSkipTweet,"Skip tweet");
                model.AddCheckItem((CefMenuCommand)MenuFreeze,"Freeze");
                model.SetChecked((CefMenuCommand)MenuFreeze,form.FreezeTimer);
                model.AddSeparator();
            }

            base.OnBeforeContextMenu(browserControl,browser,frame,parameters,model);
            RemoveSeparatorIfLast(model);
        }

        public override bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            if (base.OnContextMenuCommand(browserControl,browser,frame,parameters,commandId,eventFlags)){
                return true;
            }

            switch((int)commandId){
                case MenuSkipTweet:
                    form.InvokeSafe(form.FinishCurrentTweet);
                    return true;

                case MenuFreeze:
                    form.InvokeSafe(() => form.FreezeTimer = !form.FreezeTimer);
                    return true;
            }

            return false;
        }
    }
}
