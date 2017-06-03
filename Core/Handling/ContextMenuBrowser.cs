using CefSharp;
using System.Windows.Forms;
using TweetDuck.Core.Bridge;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Handling{
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

        private const string TitleReloadBrowser = "Reload browser";
        private const string TitleMuteNotifications = "Mute notifications";
        private const string TitleSettings = "Options";
        private const string TitlePlugins = "Plugins";
        private const string TitleAboutProgram = "About "+Program.BrandName;

        private readonly FormBrowser form;

        private string lastHighlightedTweet;
        private string lastHighlightedQuotedTweet;

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

            lastHighlightedTweet = TweetDeckBridge.LastHighlightedTweet;
            lastHighlightedQuotedTweet = TweetDeckBridge.LastHighlightedQuotedTweet;

            if (!BrowserUtils.IsTweetDeckWebsite(frame) || browser.IsLoading){
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
            
                globalMenu.AddItem(CefMenuCommand.Reload, TitleReloadBrowser);
                globalMenu.AddCheckItem((CefMenuCommand)MenuMute, TitleMuteNotifications);
                globalMenu.SetChecked((CefMenuCommand)MenuMute, Program.UserConfig.MuteNotifications);
                globalMenu.AddSeparator();

                globalMenu.AddItem((CefMenuCommand)MenuSettings, TitleSettings);
                globalMenu.AddItem((CefMenuCommand)MenuPlugins, TitlePlugins);
                globalMenu.AddItem((CefMenuCommand)MenuAbout, TitleAboutProgram);

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

            switch((int)commandId){
                case (int)CefMenuCommand.Reload:
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
                    BrowserUtils.OpenExternalBrowser(lastHighlightedTweet);
                    return true;

                case MenuCopyTweetUrl:
                    SetClipboardText(lastHighlightedTweet);
                    return true;

                case MenuScreenshotTweet:
                    form.InvokeAsyncSafe(form.TriggerTweetScreenshot);
                    return true;

                case MenuOpenQuotedTweetUrl:
                    BrowserUtils.OpenExternalBrowser(lastHighlightedQuotedTweet);
                    return true;

                case MenuCopyQuotedTweetUrl:
                    SetClipboardText(lastHighlightedQuotedTweet);
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
