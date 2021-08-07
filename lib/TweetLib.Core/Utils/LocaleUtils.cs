using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TweetLib.Core.Utils {
	public static class LocaleUtils {
		// https://cs.chromium.org/chromium/src/third_party/hunspell_dictionaries/
		public static IEnumerable<Item> SpellCheckLanguages { get; } = new List<string> {
			"af-ZA", "bg-BG", "ca-ES", "cs-CZ", "da-DK", "de-DE",
			"el-GR", "en-AU", "en-CA", "en-GB", "en-US", "es-ES",
			"et-EE", "fa-IR", "fo-FO", "fr-FR", "he-IL", "hi-IN",
			"hr-HR", "hu-HU", "id-ID", "it-IT", "ko"   , "lt-LT",
			"lv-LV", "nb-NO", "nl-NL", "pl-PL", "pt-BR", "pt-PT",
			"ro-RO", "ru-RU", "sk-SK", "sl-SI", "sq"   , "sr",
			"sv-SE", "ta-IN", "tg-TG", "tr"   , "uk-UA", "vi-VN"
		}.Select(code => {
			string lang = StringUtils.ExtractBefore(code, '-', 2);
			return lang == "en" || lang == "pt" ? new Item(code) : new Item(code, lang);
		}).OrderBy(code => code).ToList();

		// TD.languages.getSupportedTranslationDestinationLanguages() except for "ht", "in", "iw" which are missing/duplicates
		public static IEnumerable<Item> TweetDeckTranslationLocales { get; } = new List<string> {
			"bg", "ca", "zh-cn", "zh-tw", "cs", "da", "nl",
			"en", "et", "fi", "fr", "de", "el", "he", "hi",
			"hu", "id", "it", "ja", "ko", "lv", "lt", "no",
			"pl", "pt", "ro", "ru", "sk", "sl", "es", "sv",
			"th", "tr", "uk", "vi", "ar", "fa"
		}.Select(code => new Item(code)).OrderBy(code => code).ToList();

		public static int GetJQueryDayOfWeek(DayOfWeek dow) {
			return dow switch {
				DayOfWeek.Monday    => 1,
				DayOfWeek.Tuesday   => 2,
				DayOfWeek.Wednesday => 3,
				DayOfWeek.Thursday  => 4,
				DayOfWeek.Friday    => 5,
				DayOfWeek.Saturday  => 6,
				_                   => 0
			};
		}

		public sealed class Item : IComparable<Item> {
			public string Code { get; }

			private string Name => info?.NativeName ?? Code;

			private readonly CultureInfo? info;

			public Item(string code, string? alt = null) {
				this.Code = code;

				try {
					this.info = CultureInfo.GetCultureInfo(alt ?? code);
				} catch (CultureNotFoundException) {
					// ignore
				}
			}

			public override bool Equals(object obj) {
				return obj is Item other && Code.Equals(other.Code, StringComparison.OrdinalIgnoreCase);
			}

			public override int GetHashCode() {
				return Code.GetHashCode();
			}

			public override string ToString() {
				if (info == null) {
					return Code;
				}

				string capitalizedName = info.TextInfo.ToTitleCase(info.NativeName);
				return info.DisplayName == info.NativeName ? capitalizedName : $"{capitalizedName}, {info.DisplayName}";
			}

			public int CompareTo(Item other) {
				return string.Compare(Name, other.Name, false, CultureInfo.InvariantCulture);
			}
		}
	}
}
