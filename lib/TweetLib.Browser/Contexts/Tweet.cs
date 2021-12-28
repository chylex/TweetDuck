namespace TweetLib.Browser.Contexts {
	public readonly struct Tweet {
		public string Url { get; }
		public string? QuoteUrl { get; }
		public string? Author { get; }
		public string? QuoteAuthor { get; }
		public string[] ImageUrls { get; }

		public string ColumnId { get; }
		public string ChirpId { get; }

		public string? MediaAuthor => QuoteAuthor ?? Author;

		public Tweet(string url, string? quoteUrl, string? author, string? quoteAuthor, string[] imageUrls, string columnId, string chirpId) {
			Url = url;
			QuoteUrl = quoteUrl;
			Author = author;
			QuoteAuthor = quoteAuthor;
			ImageUrls = imageUrls;
			ColumnId = columnId;
			ChirpId = chirpId;
		}
	}
}
