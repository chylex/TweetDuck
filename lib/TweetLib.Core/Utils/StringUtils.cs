using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TweetLib.Core.Utils{
    public static class StringUtils{
        public static readonly string[] EmptyArray = new string[0];

        public static (string before, string after)? SplitInTwo(string str, char search, int startIndex = 0){
            int index = str.IndexOf(search, startIndex);

            if (index == -1){
                return null;
            }

            return (str.Substring(0, index), str.Substring(index + 1));
        }

        public static string ExtractBefore(string str, char search, int startIndex = 0){
            int index = str.IndexOf(search, startIndex);
            return index == -1 ? str : str.Substring(0, index);
        }

        public static int[] ParseInts(string str, char separator){
            return str.Split(new char[]{ separator }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }
        
        public static string ConvertPascalCaseToScreamingSnakeCase(string str){
            return Regex.Replace(str, @"(\p{Ll})(\P{Ll})|(\P{Ll})(\P{Ll}\p{Ll})", "$1$3_$2$4").ToUpper();
        }

        public static string ConvertRot13(string str){
            return Regex.Replace(str, @"[a-zA-Z]", match => {
                int code = match.Value[0];
                int start = code <= 90 ? 65 : 97;
                return ((char)(start + (code - start + 13) % 26)).ToString();
            });
        }

        public static int CountOccurrences(string source, string substring){
            if (substring.Length == 0){
                throw new ArgumentOutOfRangeException(nameof(substring), "Searched substring must not be empty.");
            }

            int count = 0, index = 0, length = substring.Length;

            while((index = source.IndexOf(substring, index)) != -1){
                index += length;
                ++count;
            }

            return count;
        }
    }
}
