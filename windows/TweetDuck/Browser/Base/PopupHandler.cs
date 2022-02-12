using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Core;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Browser.Base {
	sealed class PopupHandler : IPopupHandler {
		public static PopupHandler Instance { get; } = new PopupHandler();

		private PopupHandler() {}

		public bool IsPopupAllowed(string url) {
			return TwitterUrls.IsAllowedPopupUrl(url);
		}

		public void OpenExternalBrowser(string url) {
			App.SystemHandler.OpenBrowser(url);
		}
	}
}
