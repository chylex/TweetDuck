using System.Collections.Generic;
using System.Linq;

namespace TweetDck.Core.Utils{
    class TwoKeyDictionary<K1, K2, V>{
        private readonly Dictionary<K1, Dictionary<K2, V>> dict;
        private readonly int innerCapacity;

        public TwoKeyDictionary() : this(16, 16){}

        public TwoKeyDictionary(int outerCapacity, int innerCapacity){
            this.dict = new Dictionary<K1, Dictionary<K2, V>>(outerCapacity);
            this.innerCapacity = innerCapacity;
        }

        // Properties

        public V this[K1 outerKey, K2 innerKey]{
            get{ // throws on missing key
                return dict[outerKey][innerKey];
            }

            set{
                Dictionary<K2, V> innerDict;

                if (!dict.TryGetValue(outerKey, out innerDict)){
                    dict.Add(outerKey, innerDict = new Dictionary<K2, V>(innerCapacity));
                }

                innerDict[innerKey] = value;
            }
        }

        // Members

        public void Add(K1 outerKey, K2 innerKey, V value){ // throws on duplicate
            Dictionary<K2, V> innerDict;

            if (!dict.TryGetValue(outerKey, out innerDict)){
                dict.Add(outerKey, innerDict = new Dictionary<K2, V>(innerCapacity));
            }

            innerDict.Add(innerKey, value);
        }
        
        public void Clear(){
            this.dict.Clear();
        }

        public void Clear(K1 outerKey){ // throws on missing key, but keeps the key unlike Remove(K1)
            dict[outerKey].Clear();
        }
        
        public bool Contains(K1 outerKey){
            return dict.ContainsKey(outerKey);
        }
        
        public bool Contains(K1 outerKey, K2 innerKey){
            Dictionary<K2, V> innerDict;
            return dict.TryGetValue(outerKey, out innerDict) && innerDict.ContainsKey(innerKey);
        }

        public int Count(){
            return dict.Values.Sum(d => d.Count);
        }

        public int Count(K1 outerKey){ // throws on missing key
            return dict[outerKey].Count;
        }

        public bool Remove(K1 outerKey){
            return dict.Remove(outerKey);
        }
        
        public bool Remove(K1 outerKey, K2 innerKey){
            Dictionary<K2, V> innerDict;

            if (dict.TryGetValue(outerKey, out innerDict) && innerDict.Remove(innerKey)){
                if (innerDict.Count == 0){
                    dict.Remove(outerKey);
                }

                return true;
            }
            else return false;
        }

        public bool TryGetValue(K1 outerKey, K2 innerKey, out V value){
            Dictionary<K2, V> innerDict;
            
            if (dict.TryGetValue(outerKey, out innerDict)){
                return innerDict.TryGetValue(innerKey, out value);
            }
            else{
                value = default(V);
                return false;
            }
        }
    }
}
