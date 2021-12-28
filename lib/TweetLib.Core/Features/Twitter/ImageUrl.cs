using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TweetLib.Utils.Static;

namespace TweetLib.Core.Features.Twitter {
	sealed class ImageUrl {
		private static readonly Regex RegexImageUrlParams = new (@"(format|name)=(\w+)", RegexOptions.IgnoreCase);

		public static readonly string[] ValidExtensions = {
			".jpg", ".jpeg", ".png", ".gif"
		};

		public static bool TryParse(string url, out ImageUrl obj) {
			obj = default!;

			int slash = url.LastIndexOf('/');

			if (slash == -1) {
				return false;
			}

			int question = url.IndexOf('?', slash);

			if (question == -1) {
				var oldStyleUrl = StringUtils.SplitInTwo(url, ':', slash);

				if (oldStyleUrl.HasValue) {
					var (baseUrl, quality) = oldStyleUrl.Value;

					obj = new ImageUrl(baseUrl, quality);
					return true;
				}

				return false;
			}

			string? imageExtension = null;
			string? imageQuality = null;

			foreach (Match match in RegexImageUrlParams.Matches(url, question)) {
				string value = match.Groups[2].Value;

				switch (match.Groups[1].Value) {
					case "format":
						imageExtension = '.' + value;
						break;

					case "name":
						imageQuality = value;
						break;
				}
			}

			if (!ValidExtensions.Contains(imageExtension) || imageQuality == null) {
				return false;
			}

			string originalUrl = url.Substring(0, question);

			obj = new ImageUrl(Path.HasExtension(originalUrl) ? originalUrl : originalUrl + imageExtension, imageQuality);
			return true;
		}

		private readonly string baseUrl;
		private readonly string quality;

		private ImageUrl(string baseUrl, string quality) {
			this.baseUrl = baseUrl;
			this.quality = quality;
		}

		[SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
		public string WithNoQuality => baseUrl;

		public string WithQuality(ImageQuality newQuality) {
			if (newQuality == ImageQuality.Best) {
				if (baseUrl.Contains("://ton.twitter.com/") && baseUrl.Contains("/ton/data/dm/")) {
					return baseUrl + ":large";
				}
				else if (baseUrl.Contains("://pbs.twimg.com/media/")) {
					return baseUrl + ":orig";
				}
			}

			return baseUrl + ':' + quality;
		}
	}
}
