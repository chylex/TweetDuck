using System;
using System.Text.RegularExpressions;

namespace TweetDuck.Core.Utils{
    static class CommandLineArgsParser{
        private static readonly Lazy<Regex> SplitRegex = new Lazy<Regex>(() => new Regex(@"([^=\s]+(?:=(?:[^ ]*""[^""]*?""[^ ]*|[^ ]*))?)", RegexOptions.Compiled), false);
        
        public static CommandLineArgs ReadCefArguments(string argumentString){
            CommandLineArgs args = new CommandLineArgs();

            if (string.IsNullOrWhiteSpace(argumentString)){
                return args;
            }

            foreach(Match match in SplitRegex.Value.Matches(argumentString)){
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
