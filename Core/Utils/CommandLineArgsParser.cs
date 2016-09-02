using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TweetDck.Core.Utils{
    static class CommandLineArgsParser{
        private static Regex splitRegex;

        private static Regex SplitRegex{
            get{
                return splitRegex ?? (splitRegex = new Regex(@"([^=\s]+(?:=(?:""[^""]*?""|[^ ]*))?)", RegexOptions.Compiled));
            }
        }

        public static int AddToDictionary(string args, IDictionary<string, string> dictionary){
            if (string.IsNullOrWhiteSpace(args)){
                return 0;
            }

            int count = 0;

            foreach(Match match in SplitRegex.Matches(args)){
                string matchValue = match.Value;

                int indexEquals = matchValue.IndexOf('=');
                string key, value;

                if (indexEquals == -1){
                    key = matchValue.TrimStart('-');
                    value = "1";
                }
                else{
                    key = matchValue.Substring(0, indexEquals).TrimStart('-');
                    value = matchValue.Substring(indexEquals+1).Trim('"');
                }

                if (key != string.Empty){
                    dictionary[key] = value;
                    ++count;
                }
            }

            return count;
        }
    }
}
