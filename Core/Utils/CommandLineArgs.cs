using System.Collections.Generic;
using System.Text;

namespace TweetDck.Core.Utils{
    class CommandLineArgs{
        public static CommandLineArgs FromStringArray(char entryChar, string[] array){
            CommandLineArgs args = new CommandLineArgs();
            ReadStringArray(entryChar, array, args);
            return args;
        }

        public static void ReadStringArray(char entryChar, string[] array, CommandLineArgs targetArgs){
            for(int index = 0; index < array.Length; index++){
                string entry = array[index];

                if (entry.Length > 0 && entry[0] == entryChar){
                    if (index < array.Length-1){
                        string potentialValue = array[index+1];

                        if (potentialValue.Length > 0 && potentialValue[0] == entryChar){
                            targetArgs.AddFlag(entry);
                        }
                        else{
                            targetArgs.SetValue(entry, potentialValue);
                            ++index;
                        }
                    }
                    else{
                        targetArgs.AddFlag(entry);
                    }
                }
            }
        }

        private readonly HashSet<string> flags = new HashSet<string>();
        private readonly Dictionary<string, string> values = new Dictionary<string, string>();

        public int Count{
            get{
                return flags.Count+values.Count;
            }
        }

        public void AddFlag(string flag){
            flags.Add(flag.ToLowerInvariant());
        }

        public bool HasFlag(string flag){
            return flags.Contains(flag.ToLowerInvariant());
        }

        public void RemoveFlag(string flag){
            flags.Remove(flag.ToLowerInvariant());
        }

        public void SetValue(string key, string value){
            values[key.ToLowerInvariant()] = value;
        }

        public string GetValue(string key, string defaultValue){
            string val;
            return values.TryGetValue(key.ToLowerInvariant(), out val) ? val : defaultValue;
        }

        public void RemoveValue(string key){
            values.Remove(key.ToLowerInvariant());
        }

        public CommandLineArgs Clone(){
            CommandLineArgs copy = new CommandLineArgs();

            foreach(string flag in flags){
                copy.AddFlag(flag);
            }

            foreach(KeyValuePair<string, string> kvp in values){
                copy.SetValue(kvp.Key, kvp.Value);
            }

            return copy;
        }

        public void ToDictionary(IDictionary<string, string> target){
            foreach(string flag in flags){
                target[flag] = "1";
            }

            foreach(KeyValuePair<string, string> kvp in values){
                target[kvp.Key] = kvp.Value;
            }
        }

        public override string ToString(){
            StringBuilder build = new StringBuilder();

            foreach(string flag in flags){
                build.Append(flag).Append(' ');
            }

            foreach(KeyValuePair<string, string> kvp in values){
                build.Append(kvp.Key).Append(" \"").Append(kvp.Value).Append("\" ");
            }

            return build.Remove(build.Length-1, 1).ToString();
        }
    }
}
