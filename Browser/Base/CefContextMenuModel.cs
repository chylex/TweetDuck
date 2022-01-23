using System;
using CefSharp;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.Contexts;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Features.Twitter;

namespace TweetDuck.Browser.Base {
	sealed class CefContextMenuModel : IContextMenuBuilder {
		private readonly IMenuModel model;
		private readonly ContextMenuActionRegistry<CefMenuCommand> actionRegistry;

		public CefContextMenuModel(IMenuModel model, ContextMenuActionRegistry<CefMenuCommand> actionRegistry) {
			this.model = model;
			this.actionRegistry = actionRegistry;
		}

		public void AddAction(string name, Action action) {
			var id = actionRegistry.AddAction(action);
			model.AddItem(id, name);
		}

		public void AddActionWithCheck(string name, bool isChecked, Action action) {
			var id = actionRegistry.AddAction(action);
			model.AddCheckItem(id, name);
			model.SetChecked(id, isChecked);
		}

		public void AddSeparator() {
			if (model.Count > 0 && model.GetTypeAt(model.Count - 1) != MenuItemType.Separator) { // do not add separators if there is nothing to separate
				model.AddSeparator();
			}
		}

		public static Context CreateContext(IContextMenuParams parameters, TweetDeckExtraContext extraContext, ImageQuality imageQuality) {
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

		private static Link? GetLink(IContextMenuParams parameters, TweetDeckExtraContext extraContext) {
			var link = extraContext?.Link;
			if (link != null) {
				return link;
			}

			if (parameters.TypeFlags.HasFlag(ContextMenuType.Link) && extraContext?.Media == null) {
				return new Link(parameters.LinkUrl, parameters.UnfilteredLinkUrl);
			}

			return null;
		}

		private static Media? GetMedia(IContextMenuParams parameters, TweetDeckExtraContext extraContext, ImageQuality imageQuality) {
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
