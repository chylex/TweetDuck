using CefSharp;
using System.Windows.Forms;
using TweetDck.Core.Bridge;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Handling{
    class ContextMenuBrowser : ContextMenuBase{
        private const int MenuGlobal = 26600;
        private const int MenuMute = 26601;
        private const int MenuSettings = 26602;
        private const int MenuPlugins = 26003;
        private const int MenuAbout = 26604;
        
        private const int MenuOpenTweetUrl = 26610;
        private const int MenuCopyTweetUrl = 26611;
        private const int MenuOpenQuotedTweetUrl = 26612;
        private const int MenuCopyQuotedTweetUrl = 26613;
        private const int MenuScreenshotTweet = 26614;

        private readonly FormBrowser form;

        private string lastHighlightedTweet;
        private string lastHighlightedQuotedTweet;

        public ContextMenuBrowser(FormBrowser form){
            this.form = form;
        }

        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Remove(CefMenuCommand.Back);
            model.Remove(CefMenuCommand.Forward);
            model.Remove(CefMenuCommand.Print);
            model.Remove(CefMenuCommand.ViewSource);
            RemoveSeparatorIfLast(model);

            base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);

            lastHighlightedTweet = TweetDeckBridge.LastHighlightedTweet;
            lastHighlightedQuotedTweet = TweetDeckBridge.LastHighlightedQuotedTweet;

            if (!BrowserUtils.IsTweetDeckWebsite(frame)){
                lastHighlightedTweet = string.Empty;
                lastHighlightedQuotedTweet = string.Empty;
            }

            if (!string.IsNullOrEmpty(lastHighlightedTweet) && (parameters.TypeFlags & (ContextMenuType.Editable | ContextMenuType.Selection)) == 0){
                model.AddItem((CefMenuCommand)MenuOpenTweetUrl, "Open tweet in browser");
                model.AddItem((CefMenuCommand)MenuCopyTweetUrl, "Copy tweet address");
                model.AddItem((CefMenuCommand)MenuScreenshotTweet, "Screenshot tweet to clipboard");

                if (!string.IsNullOrEmpty(lastHighlightedQuotedTweet)){
                    model.AddSeparator();
                    model.AddItem((CefMenuCommand)MenuOpenQuotedTweetUrl, "Open quoted tweet in browser");
                    model.AddItem((CefMenuCommand)MenuCopyQuotedTweetUrl, "Copy quoted tweet address");
                }

                model.AddSeparator();
            }

            if ((parameters.TypeFlags & (ContextMenuType.Editable | ContextMenuType.Selection)) == 0){
                AddSeparator(model);

                IMenuModel globalMenu = model.Count == 0 ? model : model.AddSubMenu((CefMenuCommand)MenuGlobal, Program.BrandName);
            
                globalMenu.AddItem(CefMenuCommand.Reload, "Reload browser");
                globalMenu.AddCheckItem((CefMenuCommand)MenuMute, "Mute notifications");
                globalMenu.SetChecked((CefMenuCommand)MenuMute, Program.UserConfig.MuteNotifications);
                globalMenu.AddSeparator();

                globalMenu.AddItem((CefMenuCommand)MenuSettings, "Settings");
                globalMenu.AddItem((CefMenuCommand)MenuPlugins, "Plugins");
                globalMenu.AddItem((CefMenuCommand)MenuAbout, "About "+Program.BrandName);

                #if DEBUG
                globalMenu.AddSeparator();
                AddDebugMenuItems(globalMenu);
                #endif
            }
        }

        public override bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            if (base.OnContextMenuCommand(browserControl, browser, frame, parameters, commandId, eventFlags)){
                return true;
            }

            switch((int)commandId){
                case (int)CefMenuCommand.Reload:
                    frame.ExecuteJavaScriptAsync("window.location.href = 'https://tweetdeck.twitter.com'");
                    return true;

                case MenuSettings:
                    form.InvokeSafe(form.OpenSettings);
                    return true;

                case MenuAbout:
                    form.InvokeSafe(form.OpenAbout);
                    return true;

                case MenuPlugins:
                    form.InvokeSafe(form.OpenPlugins);
                    return true;

                case MenuMute:
                    form.InvokeSafe(() => {
                        Program.UserConfig.MuteNotifications = !Program.UserConfig.MuteNotifications;
                        Program.UserConfig.Save();
                    });

                    return true;

                case MenuOpenTweetUrl:
                    BrowserUtils.OpenExternalBrowser(lastHighlightedTweet);
                    return true;

                case MenuCopyTweetUrl:
                    Clipboard.SetText(lastHighlightedTweet, TextDataFormat.UnicodeText);
                    return true;

                case MenuScreenshotTweet:
                    form.InvokeSafe(form.TriggerTweetScreenshot);
                    return true;

                case MenuOpenQuotedTweetUrl:
                    BrowserUtils.OpenExternalBrowser(lastHighlightedQuotedTweet);
                    return true;

                case MenuCopyQuotedTweetUrl:
                    Clipboard.SetText(lastHighlightedQuotedTweet, TextDataFormat.UnicodeText);
                    return true;
            }

            return false;
        }
    }
}
