using System;
using System.IO;
using System.Text;

namespace TweetDck.Core.Other.Settings.Export{
    class CombinedFileStream : IDisposable{
        private readonly Stream stream;

        public CombinedFileStream(Stream stream){
            this.stream = stream;
        }

        public void WriteFile(string identifier, string path){
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

            byte[] name = Encoding.UTF8.GetBytes(identifier);
            byte[] contentsLength = BitConverter.GetBytes(contents.Length);

            stream.WriteByte((byte)name.Length);
            stream.Write(name, 0, name.Length);
            stream.Write(contentsLength, 0, 4);
            stream.Write(contents, 0, contents.Length);
        }

        public Entry ReadFile(){
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

        public void Flush(){
            stream.Flush();
        }

        void IDisposable.Dispose(){
            stream.Dispose();
        }

        public class Entry{
            public string Identifier { get; private set; }

            private readonly byte[] contents;

            public Entry(string identifier, byte[] contents){
                this.Identifier = identifier;
                this.contents = contents;
            }

            public void WriteToFile(string path){
                File.WriteAllBytes(path, contents);
            }
        }
    }
}
