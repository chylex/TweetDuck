using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TweetLib.Browser.Contexts;
using TweetLib.Core.Features.Notifications;
using TweetLib.Core.Features.Twitter;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features.TweetDeck {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	sealed class TweetDeckBridgeObject : CommonBridge {
		private readonly ITweetDeckInterface i;
		private readonly TweetDeckBrowser browser;
		private readonly TweetDeckExtraContext extraContext;

		public TweetDeckBridgeObject(ITweetDeckInterface tweetDeckInterface, TweetDeckBrowser browser, TweetDeckExtraContext extraContext) : base(tweetDeckInterface) {
			this.i = tweetDeckInterface;
			this.browser = browser;
			this.extraContext = extraContext;
		}

		public void OnModulesLoaded(string moduleNamespace) {
			browser.OnModulesLoaded(moduleNamespace);
		}

		public void OpenContextMenu() {
			i.OpenContextMenu();
		}

		public void OpenProfileImport() {
			i.OpenProfileImport();
		}

		public void OnIntroductionClosed(bool showGuide) {
			i.OnIntroductionClosed(showGuide);
		}

		public void LoadNotificationLayout(string fontSize, string headLayout) {
			NotificationBrowser.SetNotificationLayout(fontSize, headLayout);
		}

		public void DisplayTooltip(string? text) {
			i.DisplayTooltip(text);
		}

		public void SetRightClickedChirp(string columnId, string chirpId, string? tweetUrl, string? quoteUrl, string? chirpAuthors, string? chirpImages) {
			if (tweetUrl == null) {
				extraContext.SetTweet(null);
				return;
			}

			var authors = chirpAuthors?.Split(';') ?? StringUtils.EmptyArray;
			var images = chirpImages?.Split(';') ?? StringUtils.EmptyArray;
			var tweetAuthor = authors.Length >= 1 ? authors[0] : null;
			var quoteAuthor = authors.Length >= 2 ? authors[1] : null;
			var imageUrls = images.Length > 0 ? images.Select(static url => TwitterUrls.GetMediaLink(url, App.UserConfiguration.TwitterImageQuality)).ToArray() : StringUtils.EmptyArray;
			extraContext.SetTweet(new Tweet(tweetUrl, quoteUrl, tweetAuthor, quoteAuthor, imageUrls, columnId, chirpId));
		}

		public void SetRightClickedLink(string type, string? url) {
			extraContext.SetLink(type, url);
		}
	}
}
