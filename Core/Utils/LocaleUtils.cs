using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TweetDuck.Core.Utils{
    static class LocaleUtils{
        // https://cs.chromium.org/chromium/src/third_party/hunspell_dictionaries/
        public static IEnumerable<Item> SpellCheckLanguages { get; } = new List<string>{
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
        public static IEnumerable<Item> TweetDeckTranslationLocales { get; } = new List<string>{
            "bg", "ca", "zh-cn", "zh-tw", "cs", "da", "nl",
            "en", "et", "fi", "fr", "de", "el", "he", "hi",
            "hu", "id", "it", "ja", "ko", "lv", "lt", "no",
            "pl", "pt", "ro", "ru", "sk", "sl", "es", "sv",
            "th", "tr", "uk", "vi", "ar", "fa"
        }.Select(code => new Item(code)).OrderBy(code => code).ToList();

        public sealed class Item : IComparable<Item>{
            public string Code { get; }
            public CultureInfo Info { get; }

            public Item(string code, string alt = null){
                this.Code = code;
                this.Info = CultureInfo.GetCultureInfo(alt ?? code);
            }

            public override bool Equals(object obj){
                return obj is Item other && Code.Equals(other.Code, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode(){
                return Code.GetHashCode();
            }

            public override string ToString(){
                string capitalizedName = Info.TextInfo.ToTitleCase(Info.NativeName);
                return Info.DisplayName == Info.NativeName ? capitalizedName : $"{capitalizedName}, {Info.DisplayName}";
            }

            public int CompareTo(Item other){
                return string.Compare(Info.NativeName, other.Info.NativeName, false, CultureInfo.InvariantCulture);
            }
        }
    }
}
