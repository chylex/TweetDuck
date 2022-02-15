using System.Collections.Generic;
using System.Linq;
using TweetLib.Utils.Globalization;
using TweetLib.Utils.Static;

namespace TweetLib.Browser.CEF.Utils {
	public static class SpellCheck {
		// https://cs.chromium.org/chromium/src/third_party/hunspell_dictionaries/
		public static IEnumerable<Language> SupportedLanguages { get; } = new List<string> {
			"af-ZA", "bg-BG", "ca-ES", "cs-CZ", "da-DK", "de-DE",
			"el-GR", "en-AU", "en-CA", "en-GB", "en-US", "es-ES",
			"et-EE", "fa-IR", "fo-FO", "fr-FR", "he-IL", "hi-IN",
			"hr-HR", "hu-HU", "id-ID", "it-IT", "ko"   , "lt-LT",
			"lv-LV", "nb-NO", "nl-NL", "pl-PL", "pt-BR", "pt-PT",
			"ro-RO", "ru-RU", "sk-SK", "sl-SI", "sq"   , "sr",
			"sv-SE", "ta-IN", "tg-TG", "tr"   , "uk-UA", "vi-VN"
		}.Select(static code => {
			string lang = StringUtils.ExtractBefore(code, '-', 2);
			return lang is "en" or "pt" ? new Language(code) : new Language(code, lang);
		}).OrderBy(static code => code).ToList();
	}
}
