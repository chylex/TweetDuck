using System.Diagnostics.CodeAnalysis;

namespace TweetLib.Core.Features.Notifications {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	sealed class NotificationBridgeObject : CommonBridge {
		private readonly INotificationInterface i;

		public NotificationBridgeObject(INotificationInterface notificationInterface, ICommonInterface commonInterface) : base(commonInterface) {
			this.i = notificationInterface;
		}

		public void DisplayTooltip(string text) {
			i.DisplayTooltip(text);
		}

		public void LoadNextNotification() {
			i.FinishCurrentNotification();
		}

		public void ShowTweetDetail() {
			i.ShowTweetDetail();
		}
	}
}
