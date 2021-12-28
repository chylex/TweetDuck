namespace TweetLib.Browser.Contexts {
	public readonly struct Media {
		public Type MediaType { get; }
		public string Url { get; }

		public Media(Type mediaType, string url) {
			MediaType = mediaType;
			Url = url;
		}

		public enum Type {
			Image,
			Video
		}
	}
}
