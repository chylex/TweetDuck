namespace TweetLib.Browser.Contexts {
	public readonly struct Link {
		public string Url { get; }
		public string CopyUrl { get; }

		public Link(string url, string copyUrl) {
			Url = url;
			CopyUrl = copyUrl;
		}
	}
}
