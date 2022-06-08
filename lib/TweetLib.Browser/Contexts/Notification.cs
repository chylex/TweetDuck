namespace TweetLib.Browser.Contexts {
	public struct Notification {
		public string? TweetUrl { get; }
		public string? QuoteUrl { get; }

		public Notification(string? tweetUrl, string? quoteUrl) {
			TweetUrl = tweetUrl;
			QuoteUrl = quoteUrl;
		}
	}
}
