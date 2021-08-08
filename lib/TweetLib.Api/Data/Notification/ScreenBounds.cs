namespace TweetLib.Api.Data.Notification {
	public readonly struct ScreenBounds {
		public int X1 { get; }
		public int Y1 { get; }
		public int Width { get; }
		public int Height { get; }

		public int X2 => X1 + Width;
		public int Y2 => Y1 + Height;

		public ScreenBounds(int x1, int y1, int width, int height) {
			X1 = x1;
			Y1 = y1;
			Width = width;
			Height = height;
		}
	}
}
