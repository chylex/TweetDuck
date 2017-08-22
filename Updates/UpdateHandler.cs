using CefSharp;
using CefSharp.WinForms;
using System;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;
using TweetDuck.Resources;

namespace TweetDuck.Updates{
    sealed class UpdateHandler{
        private readonly ChromiumWebBrowser browser;
        private readonly UpdaterSettings settings;

        public event EventHandler<UpdateEventArgs> UpdateAccepted;
        public event EventHandler<UpdateEventArgs> UpdateDismissed;
        public event EventHandler<UpdateEventArgs> CheckFinished;

        private int lastEventId;
        private UpdateInfo lastUpdateInfo;

        public UpdateHandler(ChromiumWebBrowser browser, UpdaterSettings settings){
            this.browser = browser;
            this.settings = settings;

            browser.FrameLoadEnd += browser_FrameLoadEnd;
            browser.RegisterAsyncJsObject("$TDU", new Bridge(this));
        }

        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e){
            if (e.Frame.IsMain && TwitterUtils.IsTweetDeckWebsite(e.Frame)){
                ScriptLoader.ExecuteFile(e.Frame, "update.js");
                Check(false);
            }
        }

        public int Check(bool force){
            if (Program.UserConfig.EnableUpdateCheck || force){
                if (force){
                    settings.DismissedUpdate = null;
                }

                browser.ExecuteScriptAsync("TDUF_runUpdateCheck", ++lastEventId, Program.VersionTag, settings.DismissedUpdate ?? string.Empty, settings.AllowPreReleases);
                return lastEventId;
            }

            return 0;
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

        private void TriggerUpdateAcceptedEvent(UpdateEventArgs args){
            UpdateAccepted?.Invoke(this, args);
        }

        private void TriggerUpdateDismissedEvent(UpdateEventArgs args){
            settings.DismissedUpdate = args.UpdateInfo.VersionTag;
            UpdateDismissed?.Invoke(this, args);
        }

        private void TriggerCheckFinishedEvent(UpdateEventArgs args){
            CheckFinished?.Invoke(this, args);
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
                    owner.lastUpdateInfo = new UpdateInfo(owner.settings, eventId, versionTag, downloadUrl);
                    owner.lastUpdateInfo.BeginSilentDownload();
                }

                owner.TriggerCheckFinishedEvent(new UpdateEventArgs(eventId, owner.lastUpdateInfo));
            }

            public void OnUpdateAccepted(){
                if (owner.lastUpdateInfo != null){
                    owner.TriggerUpdateAcceptedEvent(new UpdateEventArgs(owner.lastUpdateInfo));
                }
            }

            public void OnUpdateDismissed(){
                if (owner.lastUpdateInfo != null){
                    owner.TriggerUpdateDismissedEvent(new UpdateEventArgs(owner.lastUpdateInfo));
                    owner.CleanupDownload();
                }
            }

            public void OpenBrowser(string url){
                BrowserUtils.OpenExternalBrowser(url);
            }
        }
    }
}
