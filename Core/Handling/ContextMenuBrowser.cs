using CefSharp;
using System.Windows.Forms;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Handling{
    sealed class ContextMenuBrowser : ContextMenuBase{
        private const CefMenuCommand MenuGlobal   = (CefMenuCommand)26600;
        private const CefMenuCommand MenuMute     = (CefMenuCommand)26601;
        private const CefMenuCommand MenuSettings = (CefMenuCommand)26602;
        private const CefMenuCommand MenuPlugins  = (CefMenuCommand)26003;
        private const CefMenuCommand MenuAbout    = (CefMenuCommand)26604;
        
        private const CefMenuCommand MenuOpenTweetUrl       = (CefMenuCommand)26610;
        private const CefMenuCommand MenuCopyTweetUrl       = (CefMenuCommand)26611;
        private const CefMenuCommand MenuOpenQuotedTweetUrl = (CefMenuCommand)26612;
        private const CefMenuCommand MenuCopyQuotedTweetUrl = (CefMenuCommand)26613;
        private const CefMenuCommand MenuScreenshotTweet    = (CefMenuCommand)26614;

        private const string TitleReloadBrowser = "Reload browser";
        private const string TitleMuteNotifications = "Mute notifications";
        private const string TitleSettings = "Options";
        private const string TitlePlugins = "Plugins";
        private const string TitleAboutProgram = "About "+Program.BrandName;

        private readonly FormBrowser form;

        private string lastHighlightedTweetUrl;
        private string lastHighlightedQuoteUrl;

        public ContextMenuBrowser(FormBrowser form) : base(form){
            this.form = form;
        }

        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            model.Remove(CefMenuCommand.Back);
            model.Remove(CefMenuCommand.Forward);
            model.Remove(CefMenuCommand.Print);
            model.Remove(CefMenuCommand.ViewSource);
            RemoveSeparatorIfLast(model);

            if (parameters.TypeFlags.HasFlag(ContextMenuType.Selection)){
                model.AddSeparator();
            }

            base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);

            lastHighlightedTweetUrl = TweetDeckBridge.LastHighlightedTweetUrl;
            lastHighlightedQuoteUrl = TweetDeckBridge.LastHighlightedQuoteUrl;

            if (!TwitterUtils.IsTweetDeckWebsite(frame) || browser.IsLoading){
                lastHighlightedTweetUrl = string.Empty;
                lastHighlightedQuoteUrl = string.Empty;
            }

            if (!string.IsNullOrEmpty(lastHighlightedTweetUrl) && (parameters.TypeFlags & (ContextMenuType.Editable | ContextMenuType.Selection)) == 0){
                model.AddItem(MenuOpenTweetUrl, "Open tweet in browser");
                model.AddItem(MenuCopyTweetUrl, "Copy tweet address");
                model.AddItem(MenuScreenshotTweet, "Screenshot tweet to clipboard");

                if (!string.IsNullOrEmpty(lastHighlightedQuoteUrl)){
                    model.AddSeparator();
                    model.AddItem(MenuOpenQuotedTweetUrl, "Open quoted tweet in browser");
                    model.AddItem(MenuCopyQuotedTweetUrl, "Copy quoted tweet address");
                }

                model.AddSeparator();
            }

            if ((parameters.TypeFlags & (ContextMenuType.Editable | ContextMenuType.Selection)) == 0){
                AddSeparator(model);

                IMenuModel globalMenu = model.Count == 0 ? model : model.AddSubMenu(MenuGlobal, Program.BrandName);
            
                globalMenu.AddItem(CefMenuCommand.Reload, TitleReloadBrowser);
                globalMenu.AddCheckItem(MenuMute, TitleMuteNotifications);
                globalMenu.SetChecked(MenuMute, Program.UserConfig.MuteNotifications);
                globalMenu.AddSeparator();

                globalMenu.AddItem(MenuSettings, TitleSettings);
                globalMenu.AddItem(MenuPlugins, TitlePlugins);
                globalMenu.AddItem(MenuAbout, TitleAboutProgram);

                if (HasDevTools){
                    globalMenu.AddSeparator();
                    AddDebugMenuItems(globalMenu);
                }
            }

            RemoveSeparatorIfLast(model);
        }

        public override bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags){
            if (base.OnContextMenuCommand(browserControl, browser, frame, parameters, commandId, eventFlags)){
                return true;
            }

            switch(commandId){
                case CefMenuCommand.Reload:
                    form.InvokeAsyncSafe(form.ReloadToTweetDeck);
                    return true;

                case MenuSettings:
                    form.InvokeAsyncSafe(form.OpenSettings);
                    return true;

                case MenuAbout:
                    form.InvokeAsyncSafe(form.OpenAbout);
                    return true;

                case MenuPlugins:
                    form.InvokeAsyncSafe(form.OpenPlugins);
                    return true;

                case MenuMute:
                    form.InvokeAsyncSafe(ToggleMuteNotifications);
                    return true;

                case MenuOpenTweetUrl:
                    BrowserUtils.OpenExternalBrowser(lastHighlightedTweetUrl);
                    return true;

                case MenuCopyTweetUrl:
                    SetClipboardText(lastHighlightedTweetUrl);
                    return true;

                case MenuScreenshotTweet:
                    form.InvokeAsyncSafe(form.TriggerTweetScreenshot);
                    return true;

                case MenuOpenQuotedTweetUrl:
                    BrowserUtils.OpenExternalBrowser(lastHighlightedQuoteUrl);
                    return true;

                case MenuCopyQuotedTweetUrl:
                    SetClipboardText(lastHighlightedQuoteUrl);
                    return true;
            }

            return false;
        }

        public static ContextMenu CreateMenu(FormBrowser form){
            ContextMenu menu = new ContextMenu();

            menu.MenuItems.Add(TitleReloadBrowser, (sender, args) => form.ReloadToTweetDeck());
            menu.MenuItems.Add(TitleMuteNotifications, (sender, args) => ToggleMuteNotifications());
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(TitleSettings, (sender, args) => form.OpenSettings());
            menu.MenuItems.Add(TitlePlugins, (sender, args) => form.OpenPlugins());
            menu.MenuItems.Add(TitleAboutProgram,  (sender, args) => form.OpenAbout());

            menu.Popup += (sender, args) => {
                menu.MenuItems[1].Checked = Program.UserConfig.MuteNotifications;
            };

            return menu;
        }

        private static void ToggleMuteNotifications(){
            Program.UserConfig.MuteNotifications = !Program.UserConfig.MuteNotifications;
            Program.UserConfig.Save();
        }
    }
}
