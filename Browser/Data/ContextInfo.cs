using System;
using CefSharp;
using TweetLib.Utils.Static;

namespace TweetDuck.Browser.Data {
	sealed class ContextInfo {
		private LinkInfo link;
		private ChirpInfo? chirp;

		public ContextInfo() {
			Reset();
		}

		public void SetLink(string type, string url) {
			link = string.IsNullOrEmpty(url) ? null : new LinkInfo(type, url);
		}

		public void SetChirp(string columnId, string chirpId, string tweetUrl, string quoteUrl, string chirpAuthors, string chirpImages) {
			chirp = string.IsNullOrEmpty(tweetUrl) ? (ChirpInfo?) null : new ChirpInfo(columnId, chirpId, tweetUrl, quoteUrl, chirpAuthors, chirpImages);
		}

		public ContextData Reset() {
			link = null;
			chirp = null;
			return ContextData.Empty;
		}

		public ContextData Create(IContextMenuParams parameters) {
			ContextData.Builder builder = new ContextData.Builder();
			builder.AddContext(parameters);

			if (link != null) {
				builder.AddOverride(link.Type, link.Url);
			}

			if (chirp.HasValue) {
				builder.AddChirp(chirp.Value);
			}

			return builder.Build();
		}

		// Data structures

		private sealed class LinkInfo {
			public string Type { get; }
			public string Url { get; }

			public LinkInfo(string type, string url) {
				this.Type = type;
				this.Url = url;
			}
		}

		public readonly struct ChirpInfo {
			public string ColumnId { get; }
			public string ChirpId { get; }

			public string TweetUrl { get; }
			public string QuoteUrl { get; }

			public string[] Authors => chirpAuthors?.Split(';') ?? StringUtils.EmptyArray;
			public string[] Images => chirpImages?.Split(';') ?? StringUtils.EmptyArray;

			private readonly string chirpAuthors;
			private readonly string chirpImages;

			public ChirpInfo(string columnId, string chirpId, string tweetUrl, string quoteUrl, string chirpAuthors, string chirpImages) {
				this.ColumnId = columnId;
				this.ChirpId = chirpId;
				this.TweetUrl = tweetUrl;
				this.QuoteUrl = quoteUrl;
				this.chirpAuthors = chirpAuthors;
				this.chirpImages = chirpImages;
			}
		}

		// Constructed context

		[Flags]
		public enum ContextType {
			Unknown = 0,
			Link    = 0b0001,
			Image   = 0b0010,
			Video   = 0b0100,
			Chirp   = 0b1000
		}

		public sealed class ContextData {
			public static readonly ContextData Empty = new Builder().Build();

			public ContextType Types { get; }

			public string LinkUrl { get; }
			public string UnsafeLinkUrl { get; }
			public string MediaUrl { get; }

			public ChirpInfo Chirp { get; }

			private ContextData(ContextType types, string linkUrl, string unsafeLinkUrl, string mediaUrl, ChirpInfo chirp) {
				Types = types;
				LinkUrl = linkUrl;
				UnsafeLinkUrl = unsafeLinkUrl;
				MediaUrl = mediaUrl;
				Chirp = chirp;
			}

			public sealed class Builder {
				private ContextType types = ContextType.Unknown;

				private string linkUrl = string.Empty;
				private string unsafeLinkUrl = string.Empty;
				private string mediaUrl = string.Empty;

				private ChirpInfo chirp = default;

				public void AddContext(IContextMenuParams parameters) {
					ContextMenuType flags = parameters.TypeFlags;

					if (flags.HasFlag(ContextMenuType.Media) && parameters.HasImageContents) {
						types |= ContextType.Image;
						types &= ~ContextType.Video;
						mediaUrl = parameters.SourceUrl;
					}

					if (flags.HasFlag(ContextMenuType.Link)) {
						types |= ContextType.Link;
						linkUrl = parameters.LinkUrl;
						unsafeLinkUrl = parameters.UnfilteredLinkUrl;
					}
				}

				public void AddOverride(string type, string url) {
					switch (type) {
						case "link":
							types |= ContextType.Link;
							linkUrl = url;
							unsafeLinkUrl = url;
							break;

						case "image":
							types |= ContextType.Image;
							types &= ~(ContextType.Video | ContextType.Link);
							mediaUrl = url;
							break;

						case "video":
							types |= ContextType.Video;
							types &= ~(ContextType.Image | ContextType.Link);
							mediaUrl = url;
							break;
					}
				}

				public void AddChirp(ChirpInfo chirp) {
					this.types |= ContextType.Chirp;
					this.chirp = chirp;
				}

				public ContextData Build() {
					return new ContextData(types, linkUrl, unsafeLinkUrl, mediaUrl, chirp);
				}
			}
		}
	}
}
