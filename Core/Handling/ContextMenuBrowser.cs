using CefSharp;
using System.Windows.Forms;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Handling{
    class ContextMenuBrowser : ContextMenuBase{
        private const int MenuGlobal = 26600;
        private const int MenuMute = 26601;
        private const int MenuSettings = 26602;
        private const int MenuPlugins = 26003;
        private const int MenuAbout = 26604;

        private const int MenuCopyTweetUrl = 26610;
        private const int MenuCopyTweetEmbeddedUrl = 26611;

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

            if (!string.IsNullOrEmpty(TweetDeckBridge.LastHighlightedTweet)){
                model.AddItem((CefMenuCommand)MenuCopyTweetUrl, "Copy tweet address");

                if (!string.IsNullOrEmpty(TweetDeckBridge.LastHighlightedTweetEmbedded)){
                    model.AddItem((CefMenuCommand)MenuCopyTweetEmbeddedUrl, "Copy quoted tweet address");
                }

                model.AddSeparator();
            }

            base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);

            if (model.Count > 0){
                RemoveSeparatorIfLast(model);
                model.AddSeparator();
            }

            IMenuModel globalMenu = model.Count == 0 ? model : model.AddSubMenu((CefMenuCommand)MenuGlobal, Program.BrandName);
            
            globalMenu.AddItem(CefMenuCommand.Reload, "Reload browser");
            globalMenu.AddCheckItem((CefMenuCommand)MenuMute, "Mute notifications");
            globalMenu.SetChecked((CefMenuCommand)MenuMute, Program.UserConfig.MuteNotifications);
            globalMenu.AddSeparator();

            globalMenu.AddItem((CefMenuCommand)MenuSettings, "Settings");
            globalMenu.AddItem((CefMenuCommand)MenuPlugins, "Plugins");
            globalMenu.AddItem((CefMenuCommand)MenuAbout, "About "+Program.BrandName);
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

                case MenuCopyTweetUrl:
                    Clipboard.SetText(TweetDeckBridge.LastHighlightedTweet, TextDataFormat.UnicodeText);
                    return true;

                case MenuCopyTweetEmbeddedUrl:
                    Clipboard.SetText(TweetDeckBridge.LastHighlightedTweetEmbedded, TextDataFormat.UnicodeText);
                    return true;
            }

            return false;
        }
    }
}
