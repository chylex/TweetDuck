namespace TweetLib.Browser.Contexts {
	public readonly struct Selection {
		public string Text { get; }
		public bool Editable { get; }

		public Selection(string text, bool editable) {
			Text = text;
			Editable = editable;
		}
	}
}
