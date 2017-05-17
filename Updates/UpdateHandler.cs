using CefSharp;
using CefSharp.WinForms;
using System;
using System.Windows.Forms;
using TweetDuck.Core;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;
using TweetDuck.Resources;
using TweetDuck.Updates.Events;

namespace TweetDuck.Updates{
    sealed class UpdateHandler{
        private static bool IsSystemSupported => true; // Environment.OSVersion.Version >= new Version("6.1"); // 6.1 NT version = Windows 7

        private readonly ChromiumWebBrowser browser;
        private readonly FormBrowser form;
        private readonly UpdaterSettings settings;

        public event EventHandler<UpdateAcceptedEventArgs> UpdateAccepted;
        public event EventHandler<UpdateDismissedEventArgs> UpdateDismissed;
        public event EventHandler<UpdateCheckEventArgs> CheckFinished;

        private int lastEventId;
        private UpdateInfo lastUpdateInfo;

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

        public void BeginUpdateDownload(Form ownerForm, UpdateInfo updateInfo, Action<UpdateInfo> onSuccess){
            if (updateInfo.DownloadStatus == UpdateDownloadStatus.Done){
                onSuccess(updateInfo);
            }
            else{
                FormUpdateDownload downloadForm = new FormUpdateDownload(updateInfo);

                downloadForm.VisibleChanged += (sender, args) => {
                    downloadForm.MoveToCenter(ownerForm);
                    ownerForm.Hide();
                };

                downloadForm.FormClosed += (sender, args) => {
                    downloadForm.Dispose();
                    
                    if (downloadForm.DialogResult == DialogResult.OK){ // success or manual download
                        onSuccess(updateInfo);
                    }
                    else{
                        ownerForm.Show();
                    }
                };

                downloadForm.Show();
            }
        }

        public void CleanupDownload(){
            if (lastUpdateInfo != null){
                lastUpdateInfo.DeleteInstaller();
                lastUpdateInfo = null;
            }
        }

        public void DismissUpdate(string tag){
            settings.DismissedUpdate = tag;
            UpdateDismissed?.Invoke(this, new UpdateDismissedEventArgs(tag));
        }

        private void TriggerUpdateAcceptedEvent(UpdateAcceptedEventArgs args){
            if (UpdateAccepted != null){
                form.InvokeAsyncSafe(() => UpdateAccepted(this, args));
            }
        }

        private void TriggerUpdateDismissedEvent(UpdateDismissedEventArgs args){
            form.InvokeAsyncSafe(() => {
                settings.DismissedUpdate = args.VersionTag;
                UpdateDismissed?.Invoke(this, args);
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

            public void OnUpdateCheckFinished(int eventId, string versionTag, string downloadUrl){
                if (versionTag != null && (owner.lastUpdateInfo == null || owner.lastUpdateInfo.VersionTag != versionTag)){
                    owner.CleanupDownload();
                    owner.lastUpdateInfo = new UpdateInfo(owner.settings, versionTag, downloadUrl);
                    owner.lastUpdateInfo.BeginSilentDownload();
                }

                owner.TriggerCheckFinishedEvent(new UpdateCheckEventArgs(eventId, owner.lastUpdateInfo));
            }

            public void OnUpdateAccepted(){
                if (owner.lastUpdateInfo != null){
                    owner.TriggerUpdateAcceptedEvent(new UpdateAcceptedEventArgs(owner.lastUpdateInfo));
                }
            }

            public void OnUpdateDismissed(){
                if (owner.lastUpdateInfo != null){
                    owner.TriggerUpdateDismissedEvent(new UpdateDismissedEventArgs(owner.lastUpdateInfo.VersionTag));
                    owner.CleanupDownload();
                }
            }

            public void OpenBrowser(string url){
                BrowserUtils.OpenExternalBrowser(url);
            }
        }
    }
}
