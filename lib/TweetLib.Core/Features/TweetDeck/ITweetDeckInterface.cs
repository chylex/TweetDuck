namespace TweetLib.Core.Features.TweetDeck {
	public interface ITweetDeckInterface : ICommonInterface {
		void OnIntroductionClosed(bool showGuide);
		void OpenContextMenu();
		void OpenProfileImport();
	}
}
