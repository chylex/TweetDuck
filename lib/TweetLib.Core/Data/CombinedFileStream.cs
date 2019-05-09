using System;
using System.IO;
using System.Text;
using TweetLib.Core.Utils;

namespace TweetLib.Core.Data{
    public sealed class CombinedFileStream : IDisposable{
        private const char KeySeparator = '|';

        private readonly Stream stream;

        public CombinedFileStream(Stream stream){
            this.stream = stream;
        }

        public void WriteFile(string[] identifier, string path){
            WriteFile(string.Join(KeySeparator.ToString(), identifier), path);
        }

        public void WriteFile(string identifier, string path){
            byte[] name = Encoding.UTF8.GetBytes(identifier);

            if (name.Length > 255){
                throw new ArgumentOutOfRangeException("Identifier cannot be 256 or more characters long: "+identifier);
            }

            byte[] contents;

            using(FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)){
                int index = 0;
                int left = (int)fileStream.Length;

                contents = new byte[left];

                while(left > 0){
                    int read = fileStream.Read(contents, index, left);
                    index += read;
                    left -= read;
                }
            }

            stream.WriteByte((byte)name.Length);
            stream.Write(name, 0, name.Length);
            stream.Write(BitConverter.GetBytes(contents.Length), 0, 4);
            stream.Write(contents, 0, contents.Length);
        }

        public Entry? ReadFile(){
            int nameLength = stream.ReadByte();

            if (nameLength == -1){
                return null;
            }

            byte[] name = new byte[nameLength];
            stream.Read(name, 0, nameLength);

            byte[] contentLength = new byte[4];
            stream.Read(contentLength, 0, 4);

            byte[] contents = new byte[BitConverter.ToInt32(contentLength, 0)];
            stream.Read(contents, 0, contents.Length);

            return new Entry(Encoding.UTF8.GetString(name), contents);
        }

        public string? SkipFile(){
            int nameLength = stream.ReadByte();

            if (nameLength == -1){
                return null;
            }

            byte[] name = new byte[nameLength];
            stream.Read(name, 0, nameLength);

            byte[] contentLength = new byte[4];
            stream.Read(contentLength, 0, 4);

            stream.Position += BitConverter.ToInt32(contentLength, 0);

            string keyName = Encoding.UTF8.GetString(name);
            return StringUtils.ExtractBefore(keyName, KeySeparator);
        }

        public void Flush(){
            stream.Flush();
        }

        void IDisposable.Dispose(){
            stream.Dispose();
        }

        public sealed class Entry{
            public string Identifier { get; }

            public string KeyName{
                get{
                    return StringUtils.ExtractBefore(Identifier, KeySeparator);
                }
            }

            public string[] KeyValue{
                get{
                    int index = Identifier.IndexOf(KeySeparator);
                    return index == -1 ? StringUtils.EmptyArray : Identifier.Substring(index+1).Split(KeySeparator);
                }
            }

            private readonly byte[] contents;

            public Entry(string identifier, byte[] contents){
                this.Identifier = identifier;
                this.contents = contents;
            }

            public void WriteToFile(string path){
                File.WriteAllBytes(path, contents);
            }

            public void WriteToFile(string path, bool createDirectory){
                if (createDirectory){
                    FileUtils.CreateDirectoryForFile(path);
                }

                File.WriteAllBytes(path, contents);
            }
        }
    }
}
