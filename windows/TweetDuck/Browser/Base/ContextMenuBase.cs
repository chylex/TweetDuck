using System.Drawing;
using CefSharp;
using TweetDuck.Configuration;
using TweetDuck.Utils;
using TweetImpl.CefSharp.Handlers;
using TweetLib.Browser.Contexts;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Features.Twitter;
using IContextMenuHandler = TweetLib.Browser.Interfaces.IContextMenuHandler;

namespace TweetDuck.Browser.Base {
	class ContextMenuBase : CefContextMenuHandler {
		private const CefMenuCommand MenuOpenDevTools = (CefMenuCommand) 26500;

		protected static UserConfig Config => Program.Config.User;

		public ContextMenuBase(IContextMenuHandler? handler) : base(handler) {}

		protected override Context CreateContext(IContextMenuParams parameters) {
			return CreateContext(parameters, null, Config.TwitterImageQuality);
		}

		public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) {
			base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);
			AddLastContextMenuItems(model);
		}

		protected virtual void AddLastContextMenuItems(IMenuModel model) {
			AddDebugMenuItems(model);
		}

		public override bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags) {
			if (base.OnContextMenuCommand(browserControl, browser, frame, parameters, commandId, eventFlags)) {
				return true;
			}

			if (commandId == MenuOpenDevTools) {
				browserControl.OpenDevToolsCustom(new Point(parameters.XCoord, parameters.YCoord));
				return true;
			}

			return false;
		}

		protected static void AddDebugMenuItems(IMenuModel model) {
			if (Config.DevToolsInContextMenu) {
				AddSeparator(model);
				model.AddItem(MenuOpenDevTools, "Open dev tools");
			}
		}

		protected static void AddSeparator(IMenuModel model) {
			if (model.Count > 0 && model.GetTypeAt(model.Count - 1) != MenuItemType.Separator) {
				model.AddSeparator();
			}
		}

		protected static Context CreateContext(IContextMenuParams parameters, TweetDeckExtraContext? extraContext, ImageQuality imageQuality) {
			var context = new Context();
			var flags = parameters.TypeFlags;

			var tweet = extraContext?.Tweet;
			if (tweet != null && !flags.HasFlag(ContextMenuType.Editable)) {
				context.Tweet = tweet;
			}

			context.Link = GetLink(parameters, extraContext);
			context.Media = GetMedia(parameters, extraContext, imageQuality);

			if (flags.HasFlag(ContextMenuType.Selection)) {
				context.Selection = new Selection(parameters.SelectionText, flags.HasFlag(ContextMenuType.Editable));
			}

			return context;
		}

		private static Link? GetLink(IContextMenuParams parameters, TweetDeckExtraContext? extraContext) {
			var link = extraContext?.Link;
			if (link != null) {
				return link;
			}

			if (parameters.TypeFlags.HasFlag(ContextMenuType.Link) && extraContext?.Media == null) {
				return new Link(parameters.LinkUrl, parameters.UnfilteredLinkUrl);
			}

			return null;
		}

		private static Media? GetMedia(IContextMenuParams parameters, TweetDeckExtraContext? extraContext, ImageQuality imageQuality) {
			var media = extraContext?.Media;
			if (media != null) {
				return media;
			}

			if (parameters.TypeFlags.HasFlag(ContextMenuType.Media) && parameters.HasImageContents) {
				return new Media(Media.Type.Image, TwitterUrls.GetMediaLink(parameters.SourceUrl, imageQuality));
			}

			return null;
		}
	}
}
