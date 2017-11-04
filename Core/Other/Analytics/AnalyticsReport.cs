using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace TweetDuck.Core.Other.Analytics{
    sealed class AnalyticsReport : IEnumerable{
        private OrderedDictionary data = new OrderedDictionary(32);
        private int separators;

        public void Add(int ignored){ // adding separators to pretty print
            data.Add((++separators).ToString(), null);
        }

        public void Add(string key, string value){
            data.Add(key, value);
        }

        public AnalyticsReport FinalizeReport(){
            if (!data.IsReadOnly){
                data = data.AsReadOnly();
            }

            return this;
        }

        public IEnumerator GetEnumerator(){
            return data.GetEnumerator();
        }

        public NameValueCollection ToNameValueCollection(){
            NameValueCollection collection = new NameValueCollection();

            foreach(DictionaryEntry entry in data){
                if (entry.Value != null){
                    collection.Add(((string)entry.Key).ToLower().Replace(' ', '_'), (string)entry.Value);
                }
            }

            return collection;
        }

        public override string ToString(){
            StringBuilder build = new StringBuilder();

            foreach(DictionaryEntry entry in data){
                if (entry.Value == null){
                    build.AppendLine();
                }
                else{
                    build.AppendLine(entry.Key+": "+entry.Value);
                }
            }

            return build.ToString();
        }
    }
}
