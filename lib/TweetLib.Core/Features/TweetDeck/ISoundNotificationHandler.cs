namespace TweetLib.Core.Features.TweetDeck {
	public interface ISoundNotificationHandler {
		void Unregister(string url);
		void Register(string url, string path);
	}
}
