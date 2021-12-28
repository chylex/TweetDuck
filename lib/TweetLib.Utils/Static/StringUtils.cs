using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TweetLib.Utils.Static {
	public static class StringUtils {
		public static readonly string[] EmptyArray = Array.Empty<string>();

		/// <summary>
		/// Returns <paramref name="str"/>, or <code>null</code> if <paramref name="str"/> is null or empty.
		/// </summary>
		public static string? NullIfEmpty(string str) {
			return string.IsNullOrEmpty(str) ? null : str;
		}

		/// <summary>
		/// Returns whether the <paramref name="str"/> starts with <paramref name="search"/>, using ordinal comparison.
		/// </summary>
		public static bool StartsWithOrdinal(this string str, string search) {
			return str.StartsWith(search, StringComparison.Ordinal);
		}

		/// <summary>
		/// Returns whether the <paramref name="str"/> ends with <paramref name="search"/>, using ordinal comparison.
		/// </summary>
		public static bool EndsWithOrdinal(this string str, string search) {
			return str.EndsWith(search, StringComparison.Ordinal);
		}

		/// <summary>
		/// Splits <paramref name="str"/> into two parts:
		///  - A substring from the beginning until <paramref name="search"/> (exclusive)
		///  - A substring from the first character following <paramref name="search"/> until the end
		/// Returns both parts as a tuple, or <code>null</code> if the <paramref name="search"/> character was not found.
		/// </summary>
		public static (string before, string after)? SplitInTwo(string str, char search, int startIndex = 0) {
			int index = str.IndexOf(search, startIndex);

			if (index == -1) {
				return null;
			}

			return (str.Substring(0, index), str.Substring(index + 1));
		}

		/// <summary>
		/// Extracts a substring from the beginning of <paramref name="str"/> until <paramref name="search"/> (exclusive).
		/// If the <paramref name="search"/> character is not found, returns <paramref name="str"/> as-is.
		/// </summary>
		public static string ExtractBefore(string str, char search, int startIndex = 0) {
			int index = str.IndexOf(search, startIndex);
			return index == -1 ? str : str.Substring(0, index);
		}

		/// <summary>
		/// Splits <paramref name="str"/> by the <paramref name="separator"/>, and converts each non-empty part into an integer.
		/// Throws if any part cannot be converted into an integer.
		/// </summary>
		public static int[] ParseInts(string str, char separator) {
			return str.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
		}

		/// <summary>
		/// Converts text from "PascalCase" to "SCREAMING_SNAKE_CASE".
		/// </summary>
		public static string ConvertPascalCaseToScreamingSnakeCase(string str) {
			return Regex.Replace(str, @"(\p{Ll})(\P{Ll})|(\P{Ll})(\P{Ll}\p{Ll})", "$1$3_$2$4").ToUpper();
		}

		/// <summary>
		/// Applies the ROT13 cipher to <paramref name="str"/>.
		/// </summary>
		public static string ConvertRot13(string str) {
			return Regex.Replace(str, @"[a-zA-Z]", static match => {
				int code = match.Value[0];
				int start = code <= 90 ? 65 : 97;
				return ((char) (start + (code - start + 13) % 26)).ToString();
			});
		}
	}
}
