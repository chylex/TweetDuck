using System;
using System.Text.RegularExpressions;
using TweetLib.Browser.Contexts;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Dialogs;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features {
	internal class BaseContextMenu : IContextMenuHandler {
		private readonly IBrowserComponent browser;
		private readonly FileDownloadManager fileDownloadManager;

		public BaseContextMenu(IBrowserComponent browser) {
			this.browser = browser;
			this.fileDownloadManager = new FileDownloadManager(browser);
		}

		public virtual void Show(IContextMenuBuilder menu, Context context) {
			if (context.Selection is { Editable: false } selection) {
				AddSearchSelectionItems(menu, selection.Text);
				menu.AddSeparator();
				menu.AddAction("Apply ROT13", () => App.DialogHandler.Information("ROT13", StringUtils.ConvertRot13(selection.Text), Dialogs.OK));
				menu.AddSeparator();
			}

			static string TextOpen(string name) => "Open " + name + " in browser";
			static string TextCopy(string name) => "Copy " + name + " address";
			static string TextSave(string name) => "Save " + name + " as...";

			if (context.Link is {} link && !link.CopyUrl.EndsWithOrdinal("tweetdeck.twitter.com/#") && !link.CopyUrl.StartsWithOrdinal("td://")) {
				Match match = TwitterUrls.RegexAccount.Match(link.CopyUrl);

				if (match.Success) {
					menu.AddAction(TextOpen("account"), OpenLink(link.Url));
					menu.AddAction(TextCopy("account"), CopyText(link.CopyUrl));
					menu.AddAction("Copy account username", CopyText(match.Groups[1].Value));
				}
				else {
					menu.AddAction(TextOpen("link"), OpenLink(link.Url));
					menu.AddAction(TextCopy("link"), CopyText(link.CopyUrl));
				}

				menu.AddSeparator();
			}

			if (context.Media is {} media && !media.Url.StartsWithOrdinal("td://")) {
				var tweet = context.Tweet;

				switch (media.MediaType) {
					case Media.Type.Image:
						menu.AddAction("View image in photo viewer", () => fileDownloadManager.ViewImage(media.Url));
						menu.AddAction(TextOpen("image"), OpenLink(media.Url));
						menu.AddAction(TextCopy("image"), CopyText(media.Url));
						menu.AddAction("Copy image", () => fileDownloadManager.CopyImage(media.Url));
						menu.AddAction(TextSave("image"), () => fileDownloadManager.SaveImages(new string[] { media.Url }, tweet?.MediaAuthor));

						var imageUrls = tweet?.ImageUrls;
						if (imageUrls?.Length > 1) {
							menu.AddAction(TextSave("all images"), () => fileDownloadManager.SaveImages(imageUrls, tweet?.MediaAuthor));
						}

						menu.AddSeparator();
						break;

					case Media.Type.Video:
						menu.AddAction(TextOpen("video"), OpenLink(media.Url));
						menu.AddAction(TextCopy("video"), CopyText(media.Url));
						menu.AddAction(TextSave("video"), () => fileDownloadManager.SaveVideo(media.Url, tweet?.MediaAuthor));
						menu.AddSeparator();
						break;
				}
			}
		}

		protected virtual void AddSearchSelectionItems(IContextMenuBuilder menu, string selectedText) {
			menu.AddAction("Search in browser", () => {
				App.SystemHandler.SearchText(selectedText);
				DeselectAll();
			});
		}

		protected void DeselectAll() {
			browser.RunScript("gen:deselect", "window.getSelection().removeAllRanges()");
		}

		protected static Action OpenLink(string url) {
			return () => App.SystemHandler.OpenBrowser(url);
		}

		protected static Action CopyText(string text) {
			return () => App.SystemHandler.CopyText(text);
		}
	}
}
