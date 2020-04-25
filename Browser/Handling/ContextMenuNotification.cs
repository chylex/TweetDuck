using CefSharp;
using TweetDuck.Browser.Notification;
using TweetDuck.Controls;

namespace TweetDuck.Browser.Handling{
    sealed class ContextMenuNotification : ContextMenuBase{
        private const CefMenuCommand MenuViewDetail         = (CefMenuCommand)26600;
        private const CefMenuCommand MenuSkipTweet          = (CefMenuCommand)26601;
        private const CefMenuCommand MenuFreeze             = (CefMenuCommand)26602;
        private const CefMenuCommand MenuCopyTweetUrl       = (CefMenuCommand)26603;
        private const CefMenuCommand MenuCopyQuotedTweetUrl = (CefMenuCommand)26604;

        private readonly FormNotificationBase form;
        private readonly bool enableCustomMenu;

        public ContextMenuNotification(FormNotificationBase form, bool enableCustomMenu) : base(form){
            this.form = form;
            this.enableCustomMenu = enableCustomMenu;
        }

        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Clear();

            if (parameters.TypeFlags.HasFlag(ContextMenuType.Selection)){
                model.AddItem(CefMenuCommand.Copy, "Copy");
                model.AddSeparator();
            }

            base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);

            if (enableCustomMenu){
                if (form.CanViewDetail){
                    model.AddItem(MenuViewDetail, "View detail");
                }

                model.AddItem(MenuSkipTweet, "Skip tweet");
                model.AddCheckItem(MenuFreeze, "Freeze");
                model.SetChecked(MenuFreeze, form.FreezeTimer);

                if (!string.IsNullOrEmpty(form.CurrentTweetUrl)){
                    model.AddSeparator();
                    model.AddItem(MenuCopyTweetUrl, "Copy tweet address");

                    if (!string.IsNullOrEmpty(form.CurrentQuoteUrl)){
                        model.AddItem(MenuCopyQuotedTweetUrl, "Copy quoted tweet address");
                    }
                }
            }
            
            AddDebugMenuItems(model);
            RemoveSeparatorIfLast(model);

            form.InvokeAsyncSafe(() => {
                form.ContextMenuOpen = true;
                form.AnalyticsFile.NotificationContextMenus.Trigger();
            });
        }

        public override bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            if (base.OnContextMenuCommand(browserControl, browser, frame, parameters, commandId, eventFlags)){
                return true;
            }

            switch(commandId){
                case MenuSkipTweet:
                    form.InvokeAsyncSafe(form.FinishCurrentNotification);
                    return true;

                case MenuFreeze:
                    form.InvokeAsyncSafe(() => form.FreezeTimer = !form.FreezeTimer);
                    return true;

                case MenuViewDetail:
                    form.InvokeSafe(form.ShowTweetDetail);
                    return true;

                case MenuCopyTweetUrl:
                    SetClipboardText(form, form.CurrentTweetUrl);
                    return true;

                case MenuCopyQuotedTweetUrl:
                    SetClipboardText(form, form.CurrentQuoteUrl);
                    return true;
            }

            return false;
        }

        public override void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame){
            base.OnContextMenuDismissed(browserControl, browser, frame);
            form.InvokeAsyncSafe(() => form.ContextMenuOpen = false);
        }
    }
}
