namespace TweetLib.Api.Data.Notification {
	public interface IScreen {
		ScreenBounds Bounds { get; }
		string Name { get; }
		bool IsPrimary { get; }
	}
}
