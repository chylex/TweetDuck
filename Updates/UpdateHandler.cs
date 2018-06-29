using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other.Interfaces;
using TweetDuck.Data;
using TweetDuck.Updates.Events;
using Timer = System.Windows.Forms.Timer;

namespace TweetDuck.Updates{
    sealed class UpdateHandler : IDisposable{
        public const bool TemporarilyForceUpdateChecking = true;

        public const int CheckCodeUpdatesDisabled = -1;
        public const int CheckCodeNotOnTweetDeck = -2;
        
        private readonly UpdateCheckClient client;
        private readonly TaskScheduler scheduler;
        private readonly ITweetDeckBrowser browser;
        private readonly Timer timer;

        public event EventHandler<UpdateEventArgs> UpdateAccepted;
        public event EventHandler<UpdateEventArgs> UpdateDelayed;
        public event EventHandler<UpdateEventArgs> UpdateDismissed;
        public event EventHandler<UpdateCheckEventArgs> CheckFinished;

        private ushort lastEventId;
        private UpdateInfo lastUpdateInfo;

        public UpdateHandler(ITweetDeckBrowser browser, string installerFolder){
            this.client = new UpdateCheckClient(installerFolder);
            this.scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            
            this.browser = browser;
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

            if (Program.UserConfig.EnableUpdateCheck || TemporarilyForceUpdateChecking){
                DateTime now = DateTime.Now;
                TimeSpan nextHour = now.AddSeconds(60*(60-now.Minute)-now.Second)-now;

                if (nextHour.TotalMinutes < 15){
                    nextHour = nextHour.Add(TimeSpan.FromHours(1));
                }

                timer.Interval = (int)Math.Ceiling(nextHour.TotalMilliseconds);
                timer.Start();
            }
        }

        public int Check(bool force){
            if (Program.UserConfig.EnableUpdateCheck || TemporarilyForceUpdateChecking || force){
                if (!browser.IsTweetDeckWebsite){
                    return CheckCodeNotOnTweetDeck;
                }
                
                int nextEventId = unchecked(++lastEventId);
                Task<UpdateInfo> checkTask = client.Check();

                checkTask.ContinueWith(task => HandleUpdateCheckSuccessful(nextEventId, task.Result), CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, scheduler);
                checkTask.ContinueWith(task => HandleUpdateCheckFailed(nextEventId, task.Exception.InnerException), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, scheduler);

                return nextEventId;
            }

            return CheckCodeUpdatesDisabled;
        }

        public void PrepareUpdate(UpdateInfo info){
            CleanupDownload();
            lastUpdateInfo = info;
            lastUpdateInfo.BeginSilentDownload();
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
                    if (downloadForm.DialogResult != DialogResult.OK){
                        updateInfo.CancelDownload();
                    }

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
