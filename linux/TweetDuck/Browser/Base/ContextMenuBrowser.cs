using TweetLib.Browser.Contexts;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Features.Twitter;
using Xilium.CefGlue;

namespace TweetDuck.Browser.Base {
	sealed class ContextMenuBrowser : ContextMenuBase {
		private readonly TweetDeckExtraContext extraContext;

		public ContextMenuBrowser(IContextMenuHandler? handler, TweetDeckExtraContext extraContext) : base(handler) {
			this.extraContext = extraContext;
		}

		protected override Context CreateContext(CefContextMenuParams parameters) {
			return CreateContext(parameters, extraContext, ImageQuality.Best);
		}

		protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model) {
			if (!TwitterUrls.IsTweetDeck(frame.Url) || browser.IsLoading) {
				extraContext.Reset();
			}

			base.OnBeforeContextMenu(browser, frame, state, model);
		}

		protected override void OnContextMenuDismissed(CefBrowser browser, CefFrame frame) {
			base.OnContextMenuDismissed(browser, frame);
			extraContext.Reset();
		}
	}
}
