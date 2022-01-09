using System;
using TweetImpl.CefGlue.Handlers;
using TweetLib.Browser.Contexts;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Features.Twitter;
using Xilium.CefGlue;

namespace TweetDuck.Browser.Base {
	class ContextMenuBase : ContextMenuHandler {
		private DevTools? devTools;

		protected ContextMenuBase(IContextMenuHandler? handler) : base(handler) {}

		protected override Context CreateContext(CefContextMenuParams parameters) {
			return CreateContext(parameters, null, ImageQuality.Best);
		}

		protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model) {
			base.OnBeforeContextMenu(browser, frame, state, model);
			AddSeparator(model);
			model.AddItem(26500, "Open dev tools");
		}

		protected override bool OnContextMenuCommand(CefBrowser browser, CefFrame frame, CefContextMenuParams state, int commandId, CefEventFlags eventFlags) {
			if (base.OnContextMenuCommand(browser, frame, state, commandId, eventFlags)) {
				return true;
			}

			if (commandId == 26500) {
				devTools ??= new DevTools(browser.GetHost());
				devTools.Disposed += OnDevToolsDisposed;
				devTools.Show(state.X, state.Y);
				return true;
			}

			return false;
		}

		private void OnDevToolsDisposed(object? sender, EventArgs e) {
			devTools = null;
		}

		private static void AddSeparator(CefMenuModel model) {
			if (model.Count > 0 && model.GetItemTypeAt(model.Count - 1) != CefMenuItemType.Separator) {
				model.AddSeparator();
			}
		}

		protected static Context CreateContext(CefContextMenuParams parameters, TweetDeckExtraContext? extraContext, ImageQuality imageQuality) {
			var context = new Context();
			var flags = parameters.ContextMenuType;

			var tweet = extraContext?.Tweet;
			if (tweet != null && !flags.HasFlag(CefContextMenuTypeFlags.Editable)) {
				context.Tweet = tweet;
			}

			context.Link = GetLink(parameters, extraContext);
			context.Media = GetMedia(parameters, extraContext, imageQuality);

			if (flags.HasFlag(CefContextMenuTypeFlags.Selection)) {
				context.Selection = new Selection(parameters.SelectionText, flags.HasFlag(CefContextMenuTypeFlags.Editable));
			}

			return context;
		}

		private static Link? GetLink(CefContextMenuParams parameters, TweetDeckExtraContext? extraContext) {
			var link = extraContext?.Link;
			if (link != null) {
				return link;
			}

			if (parameters.ContextMenuType.HasFlag(CefContextMenuTypeFlags.Link) && extraContext?.Media == null) {
				return new Link(parameters.LinkUrl, parameters.UnfilteredLinkUrl);
			}

			return null;
		}

		private static Media? GetMedia(CefContextMenuParams parameters, TweetDeckExtraContext? extraContext, ImageQuality imageQuality) {
			var media = extraContext?.Media;
			if (media != null) {
				return media;
			}

			if (parameters.ContextMenuType.HasFlag(CefContextMenuTypeFlags.Media) && parameters.HasImageContents) {
				return new Media(Media.Type.Image, TwitterUrls.GetMediaLink(parameters.SourceUrl, imageQuality));
			}

			return null;
		}
	}
}
