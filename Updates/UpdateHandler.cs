using System;
using CefSharp;
using CefSharp.WinForms;
using TweetDck.Core;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;
using TweetDck.Resources;
using TweetDck.Updates.Events;

namespace TweetDck.Updates{
    class UpdateHandler{
        private static bool IsSystemSupported{
            get{
                return true; // Environment.OSVersion.Version >= new Version("6.1"); // 6.1 NT version = Windows 7
            }
        }

        public UpdaterSettings Settings{
            get{
                return settings;
            }
        }

        private readonly ChromiumWebBrowser browser;
        private readonly FormBrowser form;
        private readonly UpdaterSettings settings;

        public event EventHandler<UpdateAcceptedEventArgs> UpdateAccepted;
        public event EventHandler<UpdateDismissedEventArgs> UpdateDismissed;
        public event EventHandler<UpdateCheckEventArgs> CheckFinished;

        private int lastEventId;

        public UpdateHandler(ChromiumWebBrowser browser, FormBrowser form, UpdaterSettings settings){
            this.browser = browser;
            this.form = form;
            this.settings = settings;

            browser.FrameLoadEnd += browser_FrameLoadEnd;
            browser.RegisterAsyncJsObject("$TDU", new Bridge(this));
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && BrowserUtils.IsTweetDeckWebsite(e.Frame)){
                ScriptLoader.ExecuteFile(e.Frame, "update.js");
                Check(false);
            }
        }

        public int Check(bool force){
            if (IsSystemSupported){
                if (Program.UserConfig.EnableUpdateCheck || force){
                    string dismissedUpdate = force || settings.DismissedUpdate == null ? string.Empty : settings.DismissedUpdate;

                    browser.ExecuteScriptAsync("TDUF_runUpdateCheck", ++lastEventId, Program.VersionTag, dismissedUpdate, settings.AllowPreReleases);
                    return lastEventId;
                }

                return 0;
            }
            else if (settings.DismissedUpdate != "unsupported"){
                browser.ExecuteScriptAsync("TDUF_displayNotification", "unsupported");
            }
            
            return -1;
        }

        private void TriggerUpdateAcceptedEvent(UpdateAcceptedEventArgs args){
            if (UpdateAccepted != null){
                form.InvokeAsyncSafe(() => UpdateAccepted(this, args));
            }
        }

        private void TriggerUpdateDismissedEvent(UpdateDismissedEventArgs args){
            form.InvokeAsyncSafe(() => {
                settings.DismissedUpdate = args.VersionTag;
                
                if (UpdateDismissed != null){
                    UpdateDismissed(this, args);
                }
            });
        }

        private void TriggerCheckFinishedEvent(UpdateCheckEventArgs args){
            if (CheckFinished != null){
                form.InvokeAsyncSafe(() => CheckFinished(this, args));
            }
        }

        public class Bridge{
            private readonly UpdateHandler owner;

            public Bridge(UpdateHandler owner){
                this.owner = owner;
            }

            public void TriggerUpdateCheck(){
                owner.Check(false);
            }

            public void OnUpdateCheckFinished(int eventId, bool isUpdateAvailable, string latestVersion){
                owner.TriggerCheckFinishedEvent(new UpdateCheckEventArgs(eventId, isUpdateAvailable, latestVersion));
            }

            public void OnUpdateAccepted(string versionTag, string downloadUrl){
                owner.TriggerUpdateAcceptedEvent(new UpdateAcceptedEventArgs(new UpdateInfo(versionTag, downloadUrl)));
            }

            public void OnUpdateDismissed(string versionTag){
                owner.TriggerUpdateDismissedEvent(new UpdateDismissedEventArgs(versionTag));
            }

            public void OpenBrowser(string url){
                BrowserUtils.OpenExternalBrowser(url);
            }
        }
    }
}
