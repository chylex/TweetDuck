using System;

namespace TweetDuck.Data.Serialization{
    sealed class SingleTypeConverter<T> : ITypeConverter{
        public delegate string FuncConvertToString<U>(U value);
        public delegate U FuncConvertToObject<U>(string value);

        public FuncConvertToString<T> ConvertToString { get; set; }
        public FuncConvertToObject<T> ConvertToObject { get; set; }

        bool ITypeConverter.TryWriteType(Type type, object value, out string converted){
            try{
                converted = ConvertToString((T)value);
                return true;
            }catch{
                converted = null;
                return false;
            }
        }

        bool ITypeConverter.TryReadType(Type type, string value, out object converted){
            try{
                converted = ConvertToObject(value);
                return true;
            }catch{
                converted = null;
                return false;
            }
        }
    }
}
