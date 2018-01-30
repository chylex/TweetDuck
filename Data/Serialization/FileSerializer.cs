using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using TweetDuck.Core.Utils;

namespace TweetDuck.Data.Serialization{
    sealed class FileSerializer<T>{
        private const string NewLineReal = "\r\n";
        private const string NewLineCustom = "\r~\n";

        private static string EscapeLine(string input) => input.Replace("\\", "\\\\").Replace(Environment.NewLine, "\\\r\n");
        private static string UnescapeLine(string input) => input.Replace(NewLineCustom, Environment.NewLine);

        private static string UnescapeStream(StreamReader reader){
            string data = reader.ReadToEnd();

            StringBuilder build = new StringBuilder(data.Length);
            int index = 0;

            while(true){
                int nextIndex = data.IndexOf('\\', index);

                if (nextIndex == -1 || nextIndex+1 >= data.Length){
                    break;
                }
                else{
                    build.Append(data.Substring(index, nextIndex-index));

                    char next = data[nextIndex+1];

                    if (next == '\\'){ // convert double backslash to single backslash
                        build.Append('\\');
                        index = nextIndex+2;
                    }
                    else if (next == '\r' && nextIndex+2 < data.Length && data[nextIndex+2] == '\n'){ // convert backslash followed by CRLF to custom new line
                        build.Append(NewLineCustom);
                        index = nextIndex+3;
                    }
                    else{ // single backslash
                        build.Append('\\');
                        index = nextIndex+1;
                    }
                }
            }
            
            return build.Append(data.Substring(index)).ToString();
        }

        private static readonly ITypeConverter BasicSerializerObj = new BasicTypeConverter();
        
        public delegate void HandleUnknownPropertiesHandler(T obj, Dictionary<string, string> data);
        public HandleUnknownPropertiesHandler HandleUnknownProperties { get; set; }
        
        private readonly Dictionary<string, PropertyInfo> props;
        private readonly Dictionary<Type, ITypeConverter> converters;

        public FileSerializer(){
            this.props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.CanWrite).ToDictionary(prop => prop.Name);
            this.converters = new Dictionary<Type, ITypeConverter>();
        }

        public void RegisterTypeConverter(Type type, ITypeConverter converter){
            converters[type] = converter;
        }

        public void Write(string file, T obj){
            WindowsUtils.CreateDirectoryForFile(file);

            using(StreamWriter writer = new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))){
                foreach(KeyValuePair<string, PropertyInfo> prop in props){
                    Type type = prop.Value.PropertyType;
                    object value = prop.Value.GetValue(obj);
                    
                    if (!converters.TryGetValue(type, out ITypeConverter serializer)){
                        serializer = BasicSerializerObj;
                    }

                    if (serializer.TryWriteType(type, value, out string converted)){
                        if (converted != null){
                            writer.Write($"{prop.Key} {EscapeLine(converted)}");
                            writer.Write(NewLineReal);
                        }
                    }
                    else{
                        throw new SerializationException($"Invalid serialization type, conversion failed for: {type}");
                    }
                }
            }
        }

        public void Read(string file, T obj){
            Dictionary<string, string> unknownProperties = new Dictionary<string, string>(4);

            using(StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))){
                switch(reader.Peek()){
                    case -1:
                        throw new FormatException("File is empty.");
                    case 0:
                    case 1:
                        throw new FormatException("Input appears to be a binary file.");
                }
                
                string contents = UnescapeStream(reader);
                int currentPos = 0;
                
                do{
                    string line;
                    int nextPos = contents.IndexOf(NewLineReal, currentPos);

                    if (nextPos == -1){
                        line = contents.Substring(currentPos);
                        currentPos = -1;

                        if (string.IsNullOrEmpty(line)){
                            break;
                        }
                    }
                    else{
                        line = contents.Substring(currentPos, nextPos-currentPos);
                        currentPos = nextPos+NewLineReal.Length;
                    }
                    
                    int space = line.IndexOf(' ');

                    if (space == -1){
                        throw new SerializationException($"Invalid file format, missing separator: {line}");
                    }

                    string property = line.Substring(0, space);
                    string value = UnescapeLine(line.Substring(space+1));

                    if (props.TryGetValue(property, out PropertyInfo info)){
                        if (!converters.TryGetValue(info.PropertyType, out ITypeConverter serializer)){
                            serializer = BasicSerializerObj;
                        }

                        if (serializer.TryReadType(info.PropertyType, value, out object converted)){
                            info.SetValue(obj, converted);
                        }
                        else{
                            throw new SerializationException($"Invalid file format, cannot convert value: {value} (property: {property})");
                        }
                    }
                    else{
                        unknownProperties[property] = value;
                    }
                }while(currentPos != -1);
            }

            if (unknownProperties.Count > 0){
                HandleUnknownProperties?.Invoke(obj, unknownProperties);

                if (unknownProperties.Count > 0){
                    throw new SerializationException($"Invalid file format, unknown properties: {string.Join(", ", unknownProperties.Keys)}");
                }
            }
        }

        public void ReadIfExists(string file, T obj){
            try{
                Read(file, obj);
            }catch(FileNotFoundException){
            }catch(DirectoryNotFoundException){}
        }

        public static HandleUnknownPropertiesHandler IgnoreProperties(params string[] properties){
            return (obj, data) => {
                foreach(string property in properties){
                    data.Remove(property);
                }
            };
        }

        private sealed class BasicTypeConverter : ITypeConverter{
            bool ITypeConverter.TryWriteType(Type type, object value, out string converted){
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

            bool ITypeConverter.TryReadType(Type type, string value, out object converted){
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
}
