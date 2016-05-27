﻿using CefSharp;
using System.Windows.Forms;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Handling{
    class ContextMenuBrowser : ContextMenuBase{
        private const int MenuSettings = 26600;
        private const int MenuAbout = 26601;
        private const int MenuMute = 26602;
        private const int MenuCopyTweetUrl = 26603;
        private const int MenuCopyTweetEmbeddedUrl = 26604;

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
                model.AddItem((CefMenuCommand)MenuCopyTweetUrl,"Copy tweet address");

                if (!string.IsNullOrEmpty(TweetDeckBridge.LastHighlightedTweetEmbedded)){
                    model.AddItem((CefMenuCommand)MenuCopyTweetEmbeddedUrl,"Copy quoted tweet address");
                }

                model.AddSeparator();
            }

            base.OnBeforeContextMenu(browserControl,browser,frame,parameters,model);

            if (model.Count > 0){
                RemoveSeparatorIfLast(model);
                model.AddSeparator();
            }
            
            model.AddItem(CefMenuCommand.Reload,"Reload");
            model.AddCheckItem((CefMenuCommand)MenuMute,"Mute notifications");
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

                case MenuCopyTweetUrl:
                    Clipboard.SetText(TweetDeckBridge.LastHighlightedTweet,TextDataFormat.UnicodeText);
                    return true;

                case MenuCopyTweetEmbeddedUrl:
                    Clipboard.SetText(TweetDeckBridge.LastHighlightedTweetEmbedded,TextDataFormat.UnicodeText);
                    return true;
            }

            return false;
        }
    }
}
