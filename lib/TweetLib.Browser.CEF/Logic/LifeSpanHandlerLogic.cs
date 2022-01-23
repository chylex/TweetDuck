using TweetLib.Browser.CEF.Interfaces;

namespace TweetLib.Browser.CEF.Logic {
	public sealed class LifeSpanHandlerLogic {
		public enum TargetDisposition {
			NewBackgroundTab,
			NewForegroundTab,
			NewPopup,
			NewWindow,
			Other
		}

		private readonly IPopupHandler popupHandler;

		public LifeSpanHandlerLogic(IPopupHandler popupHandler) {
			this.popupHandler = popupHandler;
		}

		public bool OnBeforePopup(string targetUrl, TargetDisposition targetDisposition) {
			switch (targetDisposition) {
				case TargetDisposition.NewBackgroundTab:
				case TargetDisposition.NewForegroundTab:
				case TargetDisposition.NewPopup when !popupHandler.IsPopupAllowed(targetUrl):
				case TargetDisposition.NewWindow:
					popupHandler.OpenExternalBrowser(targetUrl);
					return true;

				default:
					return false;
			}
		}

		public bool DoClose() {
			return false;
		}
	}
}
