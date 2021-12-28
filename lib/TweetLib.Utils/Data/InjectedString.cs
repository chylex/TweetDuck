using System;

namespace TweetLib.Utils.Data {
	public sealed class InjectedString {
		public enum Position {
			Before,
			After
		}

		private readonly Position position;
		private readonly string search;
		private readonly string html;

		public InjectedString(Position position, string search, string html) {
			this.position = position;
			this.search = search;
			this.html = html;
		}

		public string InjectInto(string targetHTML) {
			int index = targetHTML.IndexOf(search, StringComparison.Ordinal);

			if (index == -1) {
				return targetHTML;
			}

			int cutIndex;

			switch (position) {
				case Position.Before:
					cutIndex = index;
					break;
				case Position.After:
					cutIndex = index + search.Length;
					break;
				default: return targetHTML;
			}

			return targetHTML.Insert(cutIndex, html);
		}
	}
}
