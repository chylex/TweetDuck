using System;
using CefSharp;
using CefSharp.WinForms;
using TweetDck.Core;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;
using TweetDck.Resources;

namespace TweetDck.Updates{
    class UpdateHandler{
        private readonly ChromiumWebBrowser browser;
        private readonly FormBrowser form;
        private readonly UpdaterSettings settings;

        public event EventHandler<UpdateAcceptedEventArgs> UpdateAccepted;
        public event EventHandler<UpdateCheckEventArgs> CheckFinished;

        private int lastEventId;

        public UpdateHandler(ChromiumWebBrowser browser, FormBrowser form, UpdaterSettings settings){
            this.browser = browser;
            this.form = form;
            this.settings = settings;

            browser.FrameLoadEnd += browser_FrameLoadEnd;
            browser.RegisterJsObject("$TDU", new Bridge(this));
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && BrowserUtils.IsTweetDeckWebsite(e.Frame)){
                ScriptLoader.ExecuteFile(e.Frame, "update.js");
            }
        }

        public int Check(bool force){
            browser.ExecuteScriptAsync("TDUF_runUpdateCheck", force, ++lastEventId);
            return lastEventId;
        }

        private void TriggerUpdateAcceptedEvent(UpdateAcceptedEventArgs args){
            if (UpdateAccepted != null){
                form.InvokeSafe(() => UpdateAccepted(this, args));
            }
        }

        private void TriggerCheckFinishedEvent(UpdateCheckEventArgs args){
            if (CheckFinished != null){
                form.InvokeSafe(() => CheckFinished(this, args));
            }
        }

        public class Bridge{
            public string BrandName{
                get{
                    return Program.BrandName;
                }
            }

            public string VersionTag{
                get{
                    return Program.VersionTag;
                }
            }

            public bool UpdateCheckEnabled{
                get{
                    return Program.UserConfig.EnableUpdateCheck;
                }
            }

            public string DismissedVersionTag{
                get{
                    return Program.UserConfig.DismissedUpdate ?? string.Empty;
                }
            }

            public bool AllowPreReleases{
                get{
                    return owner.settings.AllowPreReleases;
                }
            }

            public bool IsSystemSupported{
                get{
                    return true; // Environment.OSVersion.Version >= new Version("6.1"); // 6.1 NT version = Windows 7
                }
            }

            private readonly UpdateHandler owner;

            public Bridge(UpdateHandler owner){
                this.owner = owner;
            }

            public void OnUpdateCheckFinished(int eventId, bool isUpdateAvailable, string latestVersion){
                owner.TriggerCheckFinishedEvent(new UpdateCheckEventArgs(eventId, isUpdateAvailable, latestVersion));
            }

            public void OnUpdateAccepted(string versionTag, string downloadUrl){
                owner.TriggerUpdateAcceptedEvent(new UpdateAcceptedEventArgs(new UpdateInfo(versionTag, downloadUrl)));
            }

            public void OnUpdateDismissed(string versionTag){
                owner.form.InvokeSafe(() => {
                    Program.UserConfig.DismissedUpdate = versionTag;
                    Program.UserConfig.Save();
                });
            }

            public void OpenBrowser(string url){
                BrowserUtils.OpenExternalBrowser(url);
            }
        }
    }
}
