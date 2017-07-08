using System;

namespace TweetDuck.Data.Serialization{
    interface ITypeConverter{
        bool TryWriteType(Type type, object value, out string converted);
        bool TryReadType(Type type, string value, out object converted);
    }
}
