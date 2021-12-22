using System;
using System.Collections.Generic;
using System.Text;
using TweetLib.Core.Data;

namespace TweetLib.Core.Features.Notifications {
	public sealed class DesktopNotification {
		private const string DefaultHeadLayout = @"<html class=""scroll-v os-windows dark txt-size--14"" lang=""en-US"" id=""tduck"" data-td-font=""medium"" data-td-theme=""dark""><head><meta charset=""utf-8""><link href=""https://ton.twimg.com/tweetdeck-web/web/dist/bundle.4b1f87e09d.css"" rel=""stylesheet""><style type='text/css'>body { background: rgb(34, 36, 38) !important }</style>";

		public enum Position {
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight,
			Custom
		}

		public enum Size {
			Auto,
			Custom
		}

		public string ColumnId { get; }
		public string ChirpId { get; }

		public string ColumnTitle { get; }
		public string TweetUrl { get; }
		public string QuoteUrl { get; }

		private readonly string html;
		private readonly int characters;

		public DesktopNotification(string columnId, string chirpId, string title, string html, int characters, string tweetUrl, string quoteUrl) {
			this.ColumnId = columnId;
			this.ChirpId = chirpId;

			this.ColumnTitle = title;
			this.TweetUrl = tweetUrl;
			this.QuoteUrl = quoteUrl;

			this.html = html;
			this.characters = characters;
		}

		public int GetDisplayDuration(int value) {
			return 2000 + Math.Max(1000, value * characters);
		}

		public string GenerateHtml(string bodyClasses, string? headLayout, string? customStyles, IEnumerable<InjectedHTML> injections, string[] scripts) { // TODO
			headLayout ??= DefaultHeadLayout;
			customStyles ??= string.Empty;

			string mainCSS = App.ResourceHandler.Load("styles/notification.css") ?? string.Empty;

			StringBuilder build = new StringBuilder(1000);
			build.Append("<!DOCTYPE html>");
			build.Append(headLayout);
			build.Append("<style type='text/css'>").Append(mainCSS).Append("</style>");

			if (!string.IsNullOrWhiteSpace(customStyles)) {
				build.Append("<style type='text/css'>").Append(customStyles).Append("</style>");
			}

			build.Append("</head><body class='scroll-styled-v");

			if (!string.IsNullOrEmpty(bodyClasses)) {
				build.Append(' ').Append(bodyClasses);
			}

			build.Append("'><div class='column' style='width:100%!important;min-height:100vh!important;height:auto!important;overflow:initial!important;'>");
			build.Append(html);
			build.Append("</div>");
			build.Append("<tweetduck-script-placeholder></body></html>");

			string result = build.ToString();

			foreach (var injection in injections) {
				result = injection.InjectInto(result);
			}

			return result.Replace("<tweetduck-script-placeholder>", GenerateScripts(scripts));
		}

		private string GenerateScripts(string[] scripts) {
			if (scripts.Length == 0) {
				return "";
			}

			StringBuilder build = new StringBuilder();

			foreach (string script in scripts) {
				build.Append("<script type='text/javascript'>");
				build.Append(script);
				build.Append("</script>");
			}

			return build.ToString();
		}
	}
}
