using System.Windows.Forms;
using CefSharp;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Handling{
    class ContextMenuNotification : ContextMenuBase{
        private const int MenuSkipTweet = 26600;
        private const int MenuFreeze = 26601;
        private const int MenuCopyTweetUrl = 26602;
        private const int MenuCopyQuotedTweetUrl = 26603;

        private readonly FormNotification form;
        private readonly bool enableCustomMenu;

        public ContextMenuNotification(FormNotification form, bool enableCustomMenu){
            this.form = form;
            this.enableCustomMenu = enableCustomMenu;
        }

        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Clear();
            base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);

            if (enableCustomMenu){
                model.AddItem((CefMenuCommand)MenuSkipTweet, "Skip tweet");
                model.AddCheckItem((CefMenuCommand)MenuFreeze, "Freeze");
                model.SetChecked((CefMenuCommand)MenuFreeze, form.FreezeTimer);
                model.AddSeparator();

                if (!string.IsNullOrEmpty(form.CurrentUrl)){
                    model.AddItem((CefMenuCommand)MenuCopyTweetUrl, "Copy tweet address");

                    if (!string.IsNullOrEmpty(form.CurrentQuotedTweetUrl)){
                        model.AddItem((CefMenuCommand)MenuCopyQuotedTweetUrl, "Copy quoted tweet address");
                    }

                    model.AddSeparator();
                }
            }

            #if DEBUG
            AddDebugMenuItems(model);
            #endif

            RemoveSeparatorIfLast(model);

            form.InvokeSafe(() => form.ContextMenuOpen = true);
        }

        public override bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            if (base.OnContextMenuCommand(browserControl, browser, frame, parameters, commandId, eventFlags)){
                return true;
            }

            switch((int)commandId){
                case MenuSkipTweet:
                    form.InvokeSafe(form.FinishCurrentTweet);
                    return true;

                case MenuFreeze:
                    form.InvokeSafe(() => form.FreezeTimer = !form.FreezeTimer);
                    return true;

                case MenuCopyTweetUrl:
                    Clipboard.SetText(form.CurrentUrl, TextDataFormat.UnicodeText);
                    return true;

                case MenuCopyQuotedTweetUrl:
                    Clipboard.SetText(form.CurrentQuotedTweetUrl, TextDataFormat.UnicodeText);
                    return true;
            }

            return false;
        }

        public override void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame){
            base.OnContextMenuDismissed(browserControl, browser, frame);
            form.InvokeSafe(() => form.ContextMenuOpen = false);
        }
    }
}
