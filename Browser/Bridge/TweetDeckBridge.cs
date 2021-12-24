using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using TweetDuck.Browser.Handling;
using TweetDuck.Browser.Notification;
using TweetDuck.Controls;
using TweetDuck.Dialogs;
using TweetDuck.Management;
using TweetDuck.Utils;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Utils;

namespace TweetDuck.Browser.Bridge {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	class TweetDeckBridge {
		public static void ResetStaticProperties() {
			FormNotificationBase.FontSize = null;
			FormNotificationBase.HeadLayout = null;
		}

		private readonly FormBrowser form;
		private readonly FormNotificationMain notification;

		private TweetDeckBridge(FormBrowser form, FormNotificationMain notification) {
			this.form = form;
			this.notification = notification;
		}

		// Browser only

		public sealed class Browser : TweetDeckBridge {
			public Browser(FormBrowser form, FormNotificationMain notification) : base(form, notification) {}

			public void OnModulesLoaded(string moduleNamespace) {
				form.InvokeAsyncSafe(() => form.OnModulesLoaded(moduleNamespace));
			}

			public void OpenContextMenu() {
				form.InvokeAsyncSafe(form.OpenContextMenu);
			}

			public void OpenProfileImport() {
				form.InvokeAsyncSafe(form.OpenProfileImport);
			}

			public void OnIntroductionClosed(bool showGuide) {
				form.InvokeAsyncSafe(() => form.OnIntroductionClosed(showGuide));
			}

			public void LoadNotificationLayout(string fontSize, string headLayout) {
				form.InvokeAsyncSafe(() => {
					FormNotificationBase.FontSize = fontSize;
					FormNotificationBase.HeadLayout = headLayout;
				});
			}

			public void SetRightClickedLink(string type, string url) {
				ContextMenuBase.CurrentInfo.SetLink(type, url);
			}

			public void SetRightClickedChirp(string columnId, string chirpId, string tweetUrl, string quoteUrl, string chirpAuthors, string chirpImages) {
				ContextMenuBase.CurrentInfo.SetChirp(columnId, chirpId, tweetUrl, quoteUrl, chirpAuthors, chirpImages);
			}

			public void DisplayTooltip(string text) {
				form.InvokeAsyncSafe(() => form.DisplayTooltip(text));
			}
		}

		// Notification only

		public sealed class Notification : TweetDeckBridge {
			public Notification(FormBrowser form, FormNotificationMain notification) : base(form, notification) {}

			public void DisplayTooltip(string text) {
				notification.InvokeAsyncSafe(() => notification.DisplayTooltip(text));
			}

			public void LoadNextNotification() {
				notification.InvokeAsyncSafe(notification.FinishCurrentNotification);
			}

			public void ShowTweetDetail() {
				notification.InvokeAsyncSafe(notification.ShowTweetDetail);
			}
		}

		// Global

		public void OnTweetPopup(string columnId, string chirpId, string columnName, string tweetHtml, int tweetCharacters, string tweetUrl, string quoteUrl) {
			notification.InvokeAsyncSafe(() => {
				form.OnTweetNotification();
				notification.ShowNotification(new DesktopNotification(columnId, chirpId, columnName, tweetHtml, tweetCharacters, tweetUrl, quoteUrl));
			});
		}

		public void OnTweetSound() {
			form.InvokeAsyncSafe(() => {
				form.OnTweetNotification();
				form.OnTweetSound();
			});
		}

		public void ScreenshotTweet(string html, int width) {
			form.InvokeAsyncSafe(() => form.OnTweetScreenshotReady(html, width));
		}

		public void PlayVideo(string videoUrl, string tweetUrl, string username, IJavascriptCallback callShowOverlay) {
			form.InvokeAsyncSafe(() => form.PlayVideo(videoUrl, tweetUrl, username, callShowOverlay));
		}

		public void StopVideo() {
			form.InvokeAsyncSafe(form.StopVideo);
		}

		public void FixClipboard() {
			form.InvokeAsyncSafe(ClipboardManager.StripHtmlStyles);
		}

		public void OpenBrowser(string url) {
			form.InvokeAsyncSafe(() => BrowserUtils.OpenExternalBrowser(url));
		}

		public void MakeGetRequest(string url, IJavascriptCallback onSuccess, IJavascriptCallback onError) {
			Task.Run(async () => {
				var client = WebUtils.NewClient(BrowserUtils.UserAgentVanilla);

				try {
					var result = await client.DownloadStringTaskAsync(url);
					await onSuccess.ExecuteAsync(result);
				} catch (Exception e) {
					await onError.ExecuteAsync(e.Message);
				} finally {
					onSuccess.Dispose();
					onError.Dispose();
					client.Dispose();
				}
			});
		}

		public int GetIdleSeconds() {
			return NativeMethods.GetIdleSeconds();
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

		public void CrashDebug(string message) {
			#if DEBUG
			System.Diagnostics.Debug.WriteLine(message);
			System.Diagnostics.Debugger.Break();
			#endif
		}
	}
}
