using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Management;
using TweetDuck.Utils;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.TweetDeck;

namespace TweetDuck.Browser {
	sealed class TweetDeckInterfaceImpl : ITweetDeckInterface {
		private readonly FormBrowser form;

		public TweetDeckInterfaceImpl(FormBrowser form) {
			this.form = form;
		}

		public void Alert(string type, string contents) {
			MessageBoxIcon icon = type switch {
				"error"   => MessageBoxIcon.Error,
				"warning" => MessageBoxIcon.Warning,
				"info"    => MessageBoxIcon.Information,
				_         => MessageBoxIcon.None
			};

			FormMessage.Show("TweetDuck Browser Message", contents, icon, FormMessage.OK);
		}

		public void DisplayTooltip(string text) {
			form.InvokeAsyncSafe(() => form.DisplayTooltip(text));
		}

		public void FixClipboard() {
			form.InvokeAsyncSafe(ClipboardManager.StripHtmlStyles);
		}

		public int GetIdleSeconds() {
			return NativeMethods.GetIdleSeconds();
		}

		public void OnIntroductionClosed(bool showGuide) {
			form.InvokeAsyncSafe(() => form.OnIntroductionClosed(showGuide));
		}

		public void OnSoundNotification() {
			form.InvokeAsyncSafe(form.OnTweetNotification);
		}

		public void OpenContextMenu() {
			form.InvokeAsyncSafe(form.OpenContextMenu);
		}

		public void OpenProfileImport() {
			form.InvokeAsyncSafe(form.OpenProfileImport);
		}

		public void PlayVideo(string videoUrl, string tweetUrl, string username, object callShowOverlay) {
			form.InvokeAsyncSafe(() => form.PlayVideo(videoUrl, tweetUrl, username, (IJavascriptCallback) callShowOverlay));
		}

		public void ScreenshotTweet(string html, int width) {
			form.InvokeAsyncSafe(() => form.OnTweetScreenshotReady(html, width));
		}

		public void ShowDesktopNotification(DesktopNotification notification) {
			form.InvokeAsyncSafe(() => {
				form.OnTweetNotification();
				form.ShowDesktopNotification(notification);
			});
		}

		public void StopVideo() {
			form.InvokeAsyncSafe(form.StopVideo);
		}

		public Task ExecuteCallback(object callback, params object[] parameters) {
			return ((IJavascriptCallback) callback).ExecuteAsync(parameters);
		}
	}
}
