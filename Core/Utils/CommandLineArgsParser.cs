using System.Text.RegularExpressions;

namespace TweetDck.Core.Utils{
    static class CommandLineArgsParser{
        private static Regex splitRegex;

        private static Regex SplitRegex{
            get{
                return splitRegex ?? (splitRegex = new Regex(@"([^=\s]+(?:=(?:""[^""]*?""|[^ ]*))?)", RegexOptions.Compiled));
            }
        }

        public static CommandLineArgs ReadCefArguments(string argumentString){
            CommandLineArgs args = new CommandLineArgs();

            if (string.IsNullOrWhiteSpace(argumentString)){
                return args;
            }

            foreach(Match match in SplitRegex.Matches(argumentString)){
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

                if (key.Length != 0){
                    args.SetValue(key, value);
                }
            }

            return args;
        }
    }
}
