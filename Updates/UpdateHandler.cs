using CefSharp;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other.Interfaces;
using TweetDuck.Data;
using TweetDuck.Resources;
using TweetDuck.Updates.Events;

namespace TweetDuck.Updates{
    sealed class UpdateHandler : IDisposable{
        public const int CheckCodeUpdatesDisabled = -1;
        public const int CheckCodeNotOnTweetDeck = -2;
        
        private readonly UpdaterSettings settings;
        private readonly UpdateCheckClient client;
        private readonly ITweetDeckBrowser browser;
        private readonly Timer timer;

        public event EventHandler<UpdateEventArgs> UpdateAccepted;
        public event EventHandler<UpdateEventArgs> UpdateDelayed;
        public event EventHandler<UpdateEventArgs> UpdateDismissed;
        public event EventHandler<UpdateCheckEventArgs> CheckFinished;

        private ushort lastEventId;
        private UpdateInfo lastUpdateInfo;

        public UpdateHandler(ITweetDeckBrowser browser, UpdaterSettings settings){
            this.settings = settings;
            this.client = new UpdateCheckClient(settings);
            
            this.browser = browser;
            this.browser.OnFrameLoaded(OnFrameLoaded);
            this.browser.RegisterBridge("$TDU", new Bridge(this));

            this.timer = new Timer();
            this.timer.Tick += timer_Tick;
        }

        public void Dispose(){
            timer.Dispose();
        }

        private void timer_Tick(object sender, EventArgs e){
            timer.Stop();
            Check(false);
        }

        public void StartTimer(){
            if (timer.Enabled){
                return;
            }

            timer.Stop();

            if (Program.UserConfig.EnableUpdateCheck){
                DateTime now = DateTime.Now;
                TimeSpan nextHour = now.AddSeconds(60*(60-now.Minute)-now.Second)-now;

                if (nextHour.TotalMinutes < 15){
                    nextHour = nextHour.Add(TimeSpan.FromHours(1));
                }

                timer.Interval = (int)Math.Ceiling(nextHour.TotalMilliseconds);
                timer.Start();
            }
        }

        private void OnFrameLoaded(IFrame frame){
            ScriptLoader.ExecuteFile(frame, "update.js"); // TODO can't show error on failure
        }

        public int Check(bool force){
            if (Program.UserConfig.EnableUpdateCheck || force){
                if (force){
                    settings.DismissedUpdate = null;
                }
                
                if (!browser.IsTweetDeckWebsite){
                    return CheckCodeNotOnTweetDeck;
                }
                
                int nextEventId = unchecked(++lastEventId);
                Task<UpdateInfo> checkTask = client.Check();

                checkTask.ContinueWith(task => HandleUpdateCheckSuccessful(nextEventId, task.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
                checkTask.ContinueWith(task => HandleUpdateCheckFailed(nextEventId, task.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted);

                return nextEventId;
            }

            return CheckCodeUpdatesDisabled;
        }

        public void BeginUpdateDownload(Form ownerForm, UpdateInfo updateInfo, Action<UpdateInfo> onFinished){
            UpdateDownloadStatus status = updateInfo.DownloadStatus;

            if (status == UpdateDownloadStatus.Done || status == UpdateDownloadStatus.AssetMissing){
                onFinished(updateInfo);
            }
            else{
                FormUpdateDownload downloadForm = new FormUpdateDownload(updateInfo);

                downloadForm.VisibleChanged += (sender, args) => {
                    downloadForm.MoveToCenter(ownerForm);
                    ownerForm.Hide();
                };

                downloadForm.FormClosed += (sender, args) => {
                    downloadForm.Dispose();
                    onFinished(updateInfo);
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

        private void HandleUpdateCheckSuccessful(int eventId, UpdateInfo info){
            if (info.IsUpdateNew && !info.IsUpdateDismissed){
                CleanupDownload();
                lastUpdateInfo = info;
                lastUpdateInfo.BeginSilentDownload();

                browser.ExecuteFunction("TDUF_displayNotification", lastUpdateInfo.VersionTag, Convert.ToBase64String(Encoding.GetEncoding("iso-8859-1").GetBytes(lastUpdateInfo.ReleaseNotes))); // TODO move browser stuff outside
            }
            
            CheckFinished?.Invoke(this, new UpdateCheckEventArgs(eventId, new Result<UpdateInfo>(info)));
        }

        private void HandleUpdateCheckFailed(int eventId, Exception exception){
            CheckFinished?.Invoke(this, new UpdateCheckEventArgs(eventId, new Result<UpdateInfo>(exception)));
        }

        private void TriggerUpdateAcceptedEvent(){
            if (lastUpdateInfo != null){
                UpdateAccepted?.Invoke(this, new UpdateEventArgs(lastUpdateInfo));
            }
        }

        private void TriggerUpdateDelayedEvent(){
            if (lastUpdateInfo != null){
                UpdateDelayed?.Invoke(this, new UpdateEventArgs(lastUpdateInfo));
            }
        }

        private void TriggerUpdateDismissedEvent(){
            if (lastUpdateInfo != null){
                settings.DismissedUpdate = lastUpdateInfo.VersionTag;
                UpdateDismissed?.Invoke(this, new UpdateEventArgs(lastUpdateInfo));

                CleanupDownload();
            }
        }

        public sealed class Bridge{
            private readonly UpdateHandler owner;

            public Bridge(UpdateHandler owner){
                this.owner = owner;
            }

            public void TriggerUpdateCheck(){
                owner.Check(false);
            }

            public void OnUpdateAccepted(){
                owner.TriggerUpdateAcceptedEvent();
            }

            public void OnUpdateDelayed(){
                owner.TriggerUpdateDelayedEvent();
            }

            public void OnUpdateDismissed(){
                owner.TriggerUpdateDismissedEvent();
            }
        }
    }
}
