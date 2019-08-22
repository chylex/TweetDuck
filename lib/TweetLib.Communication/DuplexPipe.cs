using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace TweetLib.Communication{
    public abstract class DuplexPipe : IDisposable{
        private const string Separator = "\x1F";

        public static Server CreateServer(){
            return new Server();
        }

        public static Client CreateClient(string token){
            int space = token.IndexOf(' ');
            return new Client(token.Substring(0, space), token.Substring(space + 1));
        }
        
        protected readonly PipeStream pipeIn;
        protected readonly PipeStream pipeOut;

        private readonly Thread readerThread;
        private readonly StreamWriter writerStream;

        public event EventHandler<PipeReadEventArgs> DataIn;

        protected DuplexPipe(PipeStream pipeIn, PipeStream pipeOut){
            this.pipeIn = pipeIn;
            this.pipeOut = pipeOut;

            this.readerThread = new Thread(ReaderThread){
                IsBackground = true
            };

            this.readerThread.Start();
            this.writerStream = new StreamWriter(this.pipeOut);
        }

        private void ReaderThread(){
            using(StreamReader read = new StreamReader(pipeIn)){
                string data;

                while((data = read.ReadLine()) != null){
                    DataIn?.Invoke(this, new PipeReadEventArgs(data));
                }
            }
        }

        public void Write(string key){
            writerStream.WriteLine(key);
            writerStream.Flush();
        }

        public void Write(string key, string data){
            writerStream.WriteLine(string.Concat(key, Separator, data));
            writerStream.Flush();
        }

        public void Dispose(){
            try{
                readerThread.Abort();
            }catch{
                // /shrug
            }

            pipeIn.Dispose();
            writerStream.Dispose();
        }

        public sealed class Server : DuplexPipe{
            private AnonymousPipeServerStream ServerPipeIn => (AnonymousPipeServerStream)pipeIn;
            private AnonymousPipeServerStream ServerPipeOut => (AnonymousPipeServerStream)pipeOut;

            internal Server() : base(new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable), new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable)){}

            public string GenerateToken(){
                return ServerPipeIn.GetClientHandleAsString() + " " + ServerPipeOut.GetClientHandleAsString();
            }

            public void DisposeToken(){
                ServerPipeIn.DisposeLocalCopyOfClientHandle();
                ServerPipeOut.DisposeLocalCopyOfClientHandle();
            }
        }

        public sealed class Client : DuplexPipe{
            internal Client(string handleOut, string handleIn) : base(new AnonymousPipeClientStream(PipeDirection.In, handleIn), new AnonymousPipeClientStream(PipeDirection.Out, handleOut)){}
        }

        public sealed class PipeReadEventArgs : EventArgs{
            public string Key { get; }
            public string Data { get; }
            
            internal PipeReadEventArgs(string line){
                int separatorIndex = line.IndexOf(Separator, StringComparison.Ordinal);

                if (separatorIndex == -1){
                    Key = line;
                    Data = string.Empty;
                }
                else{
                    Key = line.Substring(0, separatorIndex);
                    Data = line.Substring(separatorIndex + 1);
                }
            }
        }
    }
}
