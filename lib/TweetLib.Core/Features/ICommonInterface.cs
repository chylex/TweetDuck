using System.Threading.Tasks;
using TweetLib.Core.Features.Notifications;

namespace TweetLib.Core.Features {
	public interface ICommonInterface {
		void Alert(string type, string contents);
		void DisplayTooltip(string? text);
		void FixClipboard();
		int GetIdleSeconds();
		void OnSoundNotification();
		void PlayVideo(string videoUrl, string tweetUrl, string username, object callShowOverlay);
		void ScreenshotTweet(string html, int width);
		void ShowDesktopNotification(DesktopNotification notification);
		void StopVideo();

		Task ExecuteCallback(object callback, params object[] parameters);
	}
}
