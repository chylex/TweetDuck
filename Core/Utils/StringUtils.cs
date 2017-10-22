using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TweetDuck.Core.Utils{
    static class StringUtils{
        public static readonly string[] EmptyArray = new string[0];

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

        public static int CountOccurrences(string source, string substring){
            int count = 0, index = 0;

            while((index = source.IndexOf(substring, index)) != -1){
                index += substring.Length;
                ++count;
            }

            return count;
        }
    }
}
