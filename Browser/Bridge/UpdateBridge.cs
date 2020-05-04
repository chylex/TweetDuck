using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using TweetDuck.Controls;
using TweetLib.Core.Systems.Updates;

namespace TweetDuck.Browser.Bridge{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    class UpdateBridge{
        private readonly UpdateHandler updates;
        private readonly Control sync;

        private UpdateInfo nextUpdate = null;

        public event EventHandler<UpdateInfo> UpdateAccepted;
        public event EventHandler<UpdateInfo> UpdateDismissed;

        public UpdateBridge(UpdateHandler updates, Control sync){
            this.sync = sync;

            this.updates = updates;
            this.updates.CheckFinished += updates_CheckFinished;
        }

        internal void Cleanup(){
            updates.CheckFinished -= updates_CheckFinished;
            nextUpdate?.DeleteInstaller();
        }

        private void updates_CheckFinished(object sender, UpdateCheckEventArgs e){
            UpdateInfo foundUpdate = e.Result.HasValue ? e.Result.Value : null;

            if (nextUpdate != null && !nextUpdate.Equals(foundUpdate)){
                nextUpdate.DeleteInstaller();
            }

            nextUpdate = foundUpdate;
        }

        private void HandleInteractionEvent(EventHandler<UpdateInfo> eventHandler){
            UpdateInfo tmpInfo = nextUpdate;

            if (tmpInfo != null){
                sync.InvokeAsyncSafe(() => eventHandler?.Invoke(this, tmpInfo));
            }
        }

        // Bridge methods

        public void TriggerUpdateCheck(){
            updates.Check(false);
        }

        public void OnUpdateAccepted(){
            HandleInteractionEvent(UpdateAccepted);
        }

        public void OnUpdateDismissed(){
            HandleInteractionEvent(UpdateDismissed);

            nextUpdate?.DeleteInstaller();
            nextUpdate = null;
        }
    }
}
