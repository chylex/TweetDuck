using System;

namespace TweetLib.Core.Serialization{
    public interface ITypeConverter{
        bool TryWriteType(Type type, object value, out string? converted);
        bool TryReadType(Type type, string value, out object? converted);
    }
}
