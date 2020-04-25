using System.Windows.Forms;
using CefSharp;
using TweetDuck.Browser.Data;
using TweetDuck.Controls;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Browser.Handling{
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
        private const CefMenuCommand MenuWriteApplyROT13    = (CefMenuCommand)26615;
        private const CefMenuCommand MenuSearchInColumn     = (CefMenuCommand)26616;

        private const string TitleReloadBrowser = "Reload browser";
        private const string TitleMuteNotifications = "Mute notifications";
        private const string TitleSettings = "Options";
        private const string TitlePlugins = "Plugins";
        private const string TitleAboutProgram = "About " + Program.BrandName;

        private readonly FormBrowser form;
        
        public ContextMenuBrowser(FormBrowser form) : base(form){
            this.form = form;
        }

        public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model){
            bool isSelecting = parameters.TypeFlags.HasFlag(ContextMenuType.Selection);
            bool isEditing = parameters.TypeFlags.HasFlag(ContextMenuType.Editable);

            model.Remove(CefMenuCommand.Back);
            model.Remove(CefMenuCommand.Forward);
            model.Remove(CefMenuCommand.Print);
            model.Remove(CefMenuCommand.ViewSource);
            RemoveSeparatorIfLast(model);

            if (isSelecting){
                if (isEditing){
                    model.AddSeparator();
                    model.AddItem(MenuWriteApplyROT13, "Apply ROT13");
                }

                model.AddSeparator();
            }

            base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);

            if (isSelecting && !isEditing && TwitterUrls.IsTweetDeck(frame.Url)){
                InsertSelectionSearchItem(model, MenuSearchInColumn, "Search in a column");
            }

            if (Context.Types.HasFlag(ContextInfo.ContextType.Chirp) && !isSelecting && !isEditing){
                model.AddItem(MenuOpenTweetUrl, "Open tweet in browser");
                model.AddItem(MenuCopyTweetUrl, "Copy tweet address");
                model.AddItem(MenuScreenshotTweet, "Screenshot tweet to clipboard");

                if (!string.IsNullOrEmpty(Context.Chirp.QuoteUrl)){
                    model.AddSeparator();
                    model.AddItem(MenuOpenQuotedTweetUrl, "Open quoted tweet in browser");
                    model.AddItem(MenuCopyQuotedTweetUrl, "Copy quoted tweet address");
                }

                model.AddSeparator();
            }

            if (!isSelecting && !isEditing){
                AddSeparator(model);

                IMenuModel globalMenu = model.Count == 0 ? model : model.AddSubMenu(MenuGlobal, Program.BrandName);
            
                globalMenu.AddItem(CefMenuCommand.Reload, TitleReloadBrowser);
                globalMenu.AddCheckItem(MenuMute, TitleMuteNotifications);
                globalMenu.SetChecked(MenuMute, Config.MuteNotifications);
                globalMenu.AddSeparator();

                globalMenu.AddItem(MenuSettings, TitleSettings);
                globalMenu.AddItem(MenuPlugins, TitlePlugins);
                globalMenu.AddItem(MenuAbout, TitleAboutProgram);
                
                AddDebugMenuItems(globalMenu);
            }

            RemoveSeparatorIfLast(model);
            
            form.InvokeAsyncSafe(form.AnalyticsFile.BrowserContextMenus.Trigger);
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
                    OpenBrowser(form, Context.Chirp.TweetUrl);
                    return true;

                case MenuCopyTweetUrl:
                    SetClipboardText(form, Context.Chirp.TweetUrl);
                    return true;

                case MenuScreenshotTweet:
                    form.InvokeAsyncSafe(form.TriggerTweetScreenshot);
                    return true;

                case MenuOpenQuotedTweetUrl:
                    OpenBrowser(form, Context.Chirp.QuoteUrl);
                    return true;

                case MenuCopyQuotedTweetUrl:
                    SetClipboardText(form, Context.Chirp.QuoteUrl);
                    return true;

                case MenuWriteApplyROT13:
                    form.InvokeAsyncSafe(form.ApplyROT13);
                    return true;

                case MenuSearchInColumn:
                    string query = parameters.SelectionText;
                    form.InvokeAsyncSafe(() => form.AddSearchColumn(query));
                    DeselectAll(frame);
                    break;
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
                menu.MenuItems[1].Checked = Config.MuteNotifications;
                form.AnalyticsFile.BrowserContextMenus.Trigger();
            };

            return menu;
        }

        private static void ToggleMuteNotifications(){
            Config.MuteNotifications = !Config.MuteNotifications;
            Config.Save();
        }
    }
}
