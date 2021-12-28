using System.Collections.Generic;
using System.Linq;
using TweetLib.Utils.Globalization;

namespace TweetLib.Core.Features.Twitter {
	public static class TweetDeckTranslations {
		// TD.languages.getSupportedTranslationDestinationLanguages() except for "ht", "in", "iw" which are missing/duplicates
		public static IEnumerable<Language> SupportedLanguages { get; } = new List<string> {
			"bg", "ca", "zh-cn", "zh-tw", "cs", "da", "nl",
			"en", "et", "fi", "fr", "de", "el", "he", "hi",
			"hu", "id", "it", "ja", "ko", "lv", "lt", "no",
			"pl", "pt", "ro", "ru", "sk", "sl", "es", "sv",
			"th", "tr", "uk", "vi", "ar", "fa"
		}.Select(code => new Language(code)).OrderBy(code => code).ToList();
	}
}
