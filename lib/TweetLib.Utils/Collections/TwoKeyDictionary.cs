using System.Collections.Generic;
using System.Linq;

namespace TweetLib.Utils.Collections {
	/// <summary>
	/// A <see cref="Dictionary{TKey,TValue}"/> with nested keys.
	/// </summary>
	/// <typeparam name="K1">The type of the outer key.</typeparam>
	/// <typeparam name="K2">The type of the inner key.</typeparam>
	/// <typeparam name="V">The type of the values.</typeparam>
	public sealed class TwoKeyDictionary<K1, K2, V> {
		private readonly Dictionary<K1, Dictionary<K2, V>> dict;
		private readonly int innerCapacity;

		/// <summary>
		/// Initializes a new <see cref="TwoKeyDictionary{K1,K2,V}"/> with the default initial capacity.
		/// </summary>
		public TwoKeyDictionary() : this(16, 16) {}

		/// <summary>
		/// Initializes a new <see cref="TwoKeyDictionary{K1,K2,V}"/> with the provided initial capacity.
		/// </summary>
		/// <param name="outerCapacity">The initial number of outer keys the dictionary can contain.</param>
		/// <param name="innerCapacity">The initial number of inner keys every outer key in the dictionary can contain.</param>
		public TwoKeyDictionary(int outerCapacity, int innerCapacity) {
			this.dict = new Dictionary<K1, Dictionary<K2, V>>(outerCapacity);
			this.innerCapacity = innerCapacity;
		}

		/// <summary>
		/// Gets or sets the value associated with the two keys.
		/// A get operation throws if either key is not present in the dictionary.
		/// A set operation creates both keys.
		/// </summary>
		public V this[K1 outerKey, K2 innerKey] {
			get {
				return dict[outerKey][innerKey];
			}

			set {
				if (!dict.TryGetValue(outerKey, out Dictionary<K2, V> innerDict)) {
					dict.Add(outerKey, innerDict = new Dictionary<K2, V>(innerCapacity));
				}

				innerDict[innerKey] = value;
			}
		}

		/// <summary>
		/// Enumerates all values in the dictionary.
		/// </summary>
		public IEnumerable<V> InnerValues {
			get {
				foreach (Dictionary<K2, V> innerDict in dict.Values) {
					foreach (V value in innerDict.Values) {
						yield return value;
					}
				}
			}
		}

		/// <summary>
		/// Adds the specified keys and <paramref name="value"/> to the dictionary.
		/// Throws if the key pair already exists.
		/// </summary>
		public void Add(K1 outerKey, K2 innerKey, V value) {
			if (!dict.TryGetValue(outerKey, out Dictionary<K2, V> innerDict)) {
				dict.Add(outerKey, innerDict = new Dictionary<K2, V>(innerCapacity));
			}

			innerDict.Add(innerKey, value);
		}

		/// <summary>
		/// Removes all keys and values from the dictionary.
		/// </summary>
		public void Clear() {
			dict.Clear();
		}

		/// <summary>
		/// Removes all inner keys and values associated with the <paramref name="outerKey"/>, but does not remove the <paramref name="outerKey"/>.
		/// Throws if the <paramref name="outerKey"/> is not present in the dictionary.
		/// </summary>
		public void Clear(K1 outerKey) {
			dict[outerKey].Clear();
		}

		/// <summary>
		/// Determines whether the dictionary contains the <paramref name="outerKey"/>.
		/// </summary>
		public bool Contains(K1 outerKey) {
			return dict.ContainsKey(outerKey);
		}

		/// <summary>
		/// Determines whether the dictionary contains the key pair.
		/// </summary>
		public bool Contains(K1 outerKey, K2 innerKey) {
			return dict.TryGetValue(outerKey, out Dictionary<K2, V> innerDict) && innerDict.ContainsKey(innerKey);
		}

		/// <summary>
		/// Returns the number of values in the dictionary.
		/// </summary>
		public int Count() {
			return dict.Values.Sum(d => d.Count);
		}

		/// <summary>
		/// Returns the number of values that belong to the <paramref name="outerKey"/> in the dictionary.
		/// Throws if the <paramref name="outerKey"/> is not present in the dictionary.
		/// </summary>
		public int Count(K1 outerKey) {
			return dict[outerKey].Count;
		}

		/// <summary>
		/// Gets the value associated with the key pair.
		/// Returns true if the key pair was present.
		/// </summary>
		public bool TryGetValue(K1 outerKey, K2 innerKey, out V value) {
			if (dict.TryGetValue(outerKey, out Dictionary<K2, V> innerDict)) {
				return innerDict.TryGetValue(innerKey, out value);
			}
			else {
				value = default!;
				return false;
			}
		}

		/// <summary>
		/// Removes all inner keys and values associated with the <paramref name="outerKey"/>, including the <paramref name="outerKey"/> itself.
		/// Returns true if the <paramref name="outerKey"/> was present.
		/// </summary>
		public bool Remove(K1 outerKey) {
			return dict.Remove(outerKey);
		}

		/// <summary>
		/// Removes the value associated with the key pair.
		/// Returns true if the key pair was present.
		/// </summary>
		public bool Remove(K1 outerKey, K2 innerKey) {
			if (dict.TryGetValue(outerKey, out Dictionary<K2, V> innerDict) && innerDict.Remove(innerKey)) {
				if (innerDict.Count == 0) {
					dict.Remove(outerKey);
				}

				return true;
			}

			return false;
		}
	}
}
