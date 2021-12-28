namespace TweetLib.Core.Features.Notifications {
	public interface INotificationInterface {
		bool FreezeTimer { get; set; }
		bool IsHovered { get; }

		void DisplayTooltip(string text);
		void FinishCurrentNotification();
		void ShowTweetDetail();
	}
}
