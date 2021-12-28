namespace TweetLib.Browser.Contexts {
	public sealed class Context {
		public Tweet? Tweet { get; set; }
		public Link? Link { get; set; }
		public Media? Media { get; set; }
		public Selection? Selection { get; set; }
		public Notification? Notification { get; set; }
	}
}
