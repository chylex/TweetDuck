using TweetLib.Browser.Contexts;
using TweetLib.Core.Features.Twitter;
using static TweetLib.Browser.Contexts.Media;

namespace TweetLib.Core.Features.TweetDeck {
	public sealed class TweetDeckExtraContext {
		public Link? Link { get; private set; }
		public Media? Media { get; private set; }
		public Tweet? Tweet { get; private set; }

		public void Reset() {
			Link = null;
			Media = null;
			Tweet = null;
		}

		public void SetLink(string type, string? url) {
			Link = null;
			Media = null;

			if (string.IsNullOrEmpty(url)) {
				return;
			}

			switch (type) {
				case "link":
					Link = new Link(url!, url!);
					break;

				case "image":
					Media = new Media(Type.Image, TwitterUrls.GetMediaLink(url!, App.UserConfiguration.TwitterImageQuality));
					break;

				case "video":
					Media = new Media(Type.Video, url!);
					break;
			}
		}

		public void SetTweet(Tweet? tweet) {
			Tweet = tweet;
		}
	}
}
