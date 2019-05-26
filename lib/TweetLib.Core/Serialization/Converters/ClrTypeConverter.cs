using System;

namespace TweetLib.Core.Serialization.Converters{
    internal sealed class ClrTypeConverter : ITypeConverter{
        public static ITypeConverter Instance { get; } = new ClrTypeConverter();

        private ClrTypeConverter(){}

        bool ITypeConverter.TryWriteType(Type type, object value, out string? converted){
            switch(Type.GetTypeCode(type)){
                case TypeCode.Boolean:
                    converted = value.ToString();
                    return true;

                case TypeCode.Int32:
                    converted = ((int)value).ToString(); // cast required for enums
                    return true;

                case TypeCode.String:
                    converted = value?.ToString();
                    return true;

                default:
                    converted = null;
                    return false;
            }
        }

        bool ITypeConverter.TryReadType(Type type, string value, out object? converted){
            switch(Type.GetTypeCode(type)){
                case TypeCode.Boolean:
                    if (bool.TryParse(value, out bool b)){
                        converted = b;
                        return true;
                    }
                    else goto default;

                case TypeCode.Int32:
                    if (int.TryParse(value, out int i)){
                        converted = i;
                        return true;
                    }
                    else goto default;

                case TypeCode.String:
                    converted = value;
                    return true;

                default:
                    converted = null;
                    return false;
            }
        }
    }
}