using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using TweetLib.Core.Features.Notifications;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	internal class CommonBridge {
		private readonly ICommonInterface i;

		protected CommonBridge(ICommonInterface i) {
			this.i = i;
		}

		public void OnTweetPopup(string columnId, string chirpId, string columnName, string tweetHtml, int tweetCharacters, string tweetUrl, string quoteUrl) {
			i.ShowDesktopNotification(new DesktopNotification(columnId, chirpId, columnName, tweetHtml, tweetCharacters, tweetUrl, quoteUrl));
		}

		public void OnTweetSound() {
			i.OnSoundNotification();
		}

		public void ScreenshotTweet(string html, int width) {
			i.ScreenshotTweet(html, width);
		}

		public void PlayVideo(string videoUrl, string tweetUrl, string username, IDisposable callShowOverlay) {
			i.PlayVideo(videoUrl, tweetUrl, username, callShowOverlay);
		}

		public void StopVideo() {
			i.StopVideo();
		}

		public void FixClipboard() {
			i.FixClipboard();
		}

		public void OpenBrowser(string url) {
			App.SystemHandler.OpenBrowser(url);
		}

		public void MakeGetRequest(string url, IDisposable onSuccess, IDisposable onError) {
			Task.Run(async () => {
				var client = WebUtils.NewClient();

				try {
					var result = await client.DownloadStringTaskAsync(url);
					await i.ExecuteCallback(onSuccess, result);
				} catch (Exception e) {
					await i.ExecuteCallback(onError, e.Message);
				} finally {
					onSuccess.Dispose();
					onError.Dispose();
					client.Dispose();
				}
			});
		}

		public int GetIdleSeconds() {
			return i.GetIdleSeconds();
		}

		public void Alert(string type, string contents) {
			i.Alert(type, contents);
		}

		public void CrashDebug(string message) {
			#if DEBUG
			System.Diagnostics.Debug.WriteLine(message);
			System.Diagnostics.Debugger.Break();
			#endif
		}
	}
}
