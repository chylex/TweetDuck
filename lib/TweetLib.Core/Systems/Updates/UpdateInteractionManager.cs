using System;
using System.Diagnostics.CodeAnalysis;

namespace TweetLib.Core.Systems.Updates {
	public sealed class UpdateInteractionManager : IDisposable {
		public event EventHandler<UpdateInfo>? UpdateAccepted;
		public event EventHandler<UpdateInfo>? UpdateDismissed;

		internal object BridgeObject { get; }

		private readonly UpdateChecker updates;
		private UpdateInfo? nextUpdate = null;

		internal UpdateInteractionManager(UpdateChecker updates) {
			this.updates = updates;
			this.updates.CheckFinished += updates_CheckFinished;
			this.BridgeObject = new Bridge(this);
		}

		public void Dispose() {
			updates.CheckFinished -= updates_CheckFinished;
		}

		public void ClearUpdate() {
			nextUpdate?.DeleteInstaller();
			nextUpdate = null;
		}

		private void updates_CheckFinished(object sender, UpdateCheckEventArgs e) {
			UpdateInfo? foundUpdate = e.Result.HasValue ? e.Result.Value : null;

			if (nextUpdate != null && !nextUpdate.Equals(foundUpdate)) {
				nextUpdate.DeleteInstaller();
			}

			nextUpdate = foundUpdate;
		}

		private void HandleInteractionEvent(EventHandler<UpdateInfo>? eventHandler) {
			UpdateInfo? updateInfo = nextUpdate;

			if (updateInfo != null) {
				eventHandler?.Invoke(this, updateInfo);
			}
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		private sealed class Bridge {
			private readonly UpdateInteractionManager owner;

			public Bridge(UpdateInteractionManager owner) {
				this.owner = owner;
			}

			public void TriggerUpdateCheck() {
				owner.updates.Check(false);
			}

			public void OnUpdateAccepted() {
				owner.HandleInteractionEvent(owner.UpdateAccepted);
			}

			public void OnUpdateDismissed() {
				owner.HandleInteractionEvent(owner.UpdateDismissed);
				owner.ClearUpdate();
			}
		}
	}
}
