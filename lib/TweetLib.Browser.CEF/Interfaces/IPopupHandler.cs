namespace TweetLib.Browser.CEF.Interfaces {
	public interface IPopupHandler {
		bool IsPopupAllowed(string url);
		void OpenExternalBrowser(string url);
	}
}
