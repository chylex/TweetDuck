using System;
using System.Collections.Generic;

namespace TweetLib.Utils.Serialization {
	public sealed class TypeConverterRegistry {
		private readonly Dictionary<Type, ITypeConverter> converters = new ();

		public void Register(Type type, ITypeConverter converter) {
			converters[type] = converter;
		}

		public ITypeConverter? TryGet(Type type) {
			return converters.TryGetValue(type, out var converter) ? converter : null;
		}
	}
}
