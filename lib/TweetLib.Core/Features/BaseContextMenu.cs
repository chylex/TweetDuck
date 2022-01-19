using System.Text.RegularExpressions;
using TweetLib.Browser.Contexts;
using TweetLib.Browser.Interfaces;
using TweetLib.Core.Features.Twitter;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features {
	class BaseContextMenu : IContextMenuHandler {
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
				menu.AddAction("Apply ROT13", () => App.MessageDialogs.Information("ROT13", StringUtils.ConvertRot13(selection.Text)));
				menu.AddSeparator();
			}

			static string TextOpen(string name) => "Open " + name + " in browser";
			static string TextCopy(string name) => "Copy " + name + " address";
			static string TextSave(string name) => "Save " + name + " as...";

			if (context.Link is {} link && !link.CopyUrl.EndsWithOrdinal("tweetdeck.twitter.com/#") && !link.CopyUrl.StartsWithOrdinal("td://")) {
				Match match = TwitterUrls.RegexAccount.Match(link.CopyUrl);

				if (match.Success) {
					AddOpenAction(menu, TextOpen("account"), link.Url);
					AddCopyAction(menu, TextCopy("account"), link.CopyUrl);
					AddCopyAction(menu, "Copy account username", match.Groups[1].Value);
				}
				else {
					AddOpenAction(menu, TextOpen("link"), link.Url);
					AddCopyAction(menu, TextCopy("link"), link.CopyUrl);
				}

				menu.AddSeparator();
			}

			if (context.Media is {} media && !media.Url.StartsWithOrdinal("td://")) {
				var tweet = context.Tweet;

				switch (media.MediaType) {
					case Media.Type.Image:
						if (fileDownloadManager.SupportsViewingImage) {
							menu.AddAction("View image in photo viewer", () => fileDownloadManager.ViewImage(media.Url));
						}

						AddOpenAction(menu, TextOpen("image"), media.Url);
						AddCopyAction(menu, TextCopy("image"), media.Url);

						if (fileDownloadManager.SupportsCopyingImage) {
							menu.AddAction("Copy image", () => fileDownloadManager.CopyImage(media.Url));
						}

						if (fileDownloadManager.SupportsFileSaving) {
							menu.AddAction(TextSave("image"), () => fileDownloadManager.SaveImages(new string[] { media.Url }, tweet?.MediaAuthor));

							var imageUrls = tweet?.ImageUrls;
							if (imageUrls?.Length > 1) {
								menu.AddAction(TextSave("all images"), () => fileDownloadManager.SaveImages(imageUrls, tweet?.MediaAuthor));
							}
						}

						menu.AddSeparator();
						break;

					case Media.Type.Video:
						AddOpenAction(menu, TextOpen("video"), media.Url);
						AddCopyAction(menu, TextCopy("video"), media.Url);

						if (fileDownloadManager.SupportsFileSaving) {
							menu.AddAction(TextSave("video"), () => fileDownloadManager.SaveVideo(media.Url, tweet?.MediaAuthor));
						}

						menu.AddSeparator();
						break;
				}
			}
		}

		protected virtual void AddSearchSelectionItems(IContextMenuBuilder menu, string selectedText) {
			if (App.SystemHandler.SearchText is {} searchText) {
				menu.AddAction("Search in browser", () => {
					searchText(selectedText);
					DeselectAll();
				});
			}
		}

		protected void DeselectAll() {
			browser.RunScript("gen:deselect", "window.getSelection().removeAllRanges()");
		}

		protected static void AddOpenAction(IContextMenuBuilder menu, string title, string url) {
			menu.AddAction(title, () => App.SystemHandler.OpenBrowser(url));
		}

		protected static void AddCopyAction(IContextMenuBuilder menu, string title, string textToCopy) {
			if (App.SystemHandler.CopyText is {} copyText) {
				menu.AddAction(title, () => copyText(textToCopy));
			}
		}
	}
}
