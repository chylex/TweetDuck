using TweetLib.Browser.Base;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Core.Features {
	public sealed class BaseBrowser : BaseBrowser<BaseBrowser> {
		public BaseBrowser(IBrowserComponent browserComponent) : base(browserComponent, CreateSetupObject) {}

		internal static BrowserSetup CreateSetupObject(IBrowserComponent browserComponent, BrowserSetup setup) {
			setup.ContextMenuHandler ??= new BaseContextMenu(browserComponent);
			setup.ResourceRequestHandler ??= new BaseResourceRequestHandler();
			return setup;
		}

		internal static BrowserSetup CreateSetupObject(IBrowserComponent browserComponent) {
			return CreateSetupObject(browserComponent, new BrowserSetup());
		}

		private static BrowserSetup CreateSetupObject(BaseBrowser browser) {
			return CreateSetupObject(browser.browserComponent);
		}
	}
}
