using System;

namespace TweetLib.Core.Serialization.Converters{
    public sealed class SingleTypeConverter<T> : ITypeConverter{
        public Func<T, string>? ConvertToString { get; set; }
        public Func<string, T>? ConvertToObject { get; set; }

        bool ITypeConverter.TryWriteType(Type type, object value, out string? converted){
            try{
                converted = ConvertToString!((T)value);
                return true;
            }catch{
                converted = null;
                return false;
            }
        }

        bool ITypeConverter.TryReadType(Type type, string value, out object? converted){
            try{
                converted = ConvertToObject!(value);
                return true;
            }catch{
                converted = null;
                return false;
            }
        }
    }
}
