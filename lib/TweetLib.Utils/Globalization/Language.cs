using System;
using System.Globalization;

namespace TweetLib.Utils.Globalization {
	public sealed class Language : IComparable<Language> {
		public string Code { get; }

		private string Name => cultureInfo?.NativeName ?? Code;

		private readonly CultureInfo? cultureInfo;

		public Language(string code, string? alt = null) {
			this.Code = code;

			try {
				this.cultureInfo = CultureInfo.GetCultureInfo(alt ?? code);
			} catch (CultureNotFoundException) {
				// ignore
			}
		}

		public override bool Equals(object obj) {
			return obj is Language other && Code.Equals(other.Code, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode() {
			return Code.GetHashCode();
		}

		public override string ToString() {
			if (cultureInfo == null) {
				return Code;
			}

			string capitalizedName = cultureInfo.TextInfo.ToTitleCase(cultureInfo.NativeName);
			return cultureInfo.DisplayName == cultureInfo.NativeName ? capitalizedName : $"{capitalizedName}, {cultureInfo.DisplayName}";
		}

		public int CompareTo(Language other) {
			return string.Compare(Name, other.Name, false, CultureInfo.InvariantCulture);
		}
	}
}
