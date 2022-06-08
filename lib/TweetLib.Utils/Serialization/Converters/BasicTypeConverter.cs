using System;

namespace TweetLib.Utils.Serialization.Converters {
	public sealed class BasicTypeConverter<T> : ITypeConverter {
		public Func<T, string>? ConvertToString { get; init; }
		public Func<string, T>? ConvertToObject { get; init; }

		bool ITypeConverter.TryWriteType(Type type, object? value, out string? converted) {
			try {
				converted = ConvertToString!((T) value!);
				return true;
			} catch {
				converted = null;
				return false;
			}
		}

		bool ITypeConverter.TryReadType(Type type, string value, out object? converted) {
			try {
				converted = ConvertToObject!(value);
				return true;
			} catch {
				converted = null;
				return false;
			}
		}
	}
}
