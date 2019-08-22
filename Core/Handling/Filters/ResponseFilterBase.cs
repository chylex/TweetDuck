using System;
using System.IO;
using System.Text;
using CefSharp;

namespace TweetDuck.Core.Handling.Filters{
    abstract class ResponseFilterBase : IResponseFilter{
        private enum State{
            Reading, Writing, Done
        }

        private readonly Encoding encoding;
        private byte[] responseData;

        private State state;
        private int offset;

        protected ResponseFilterBase(int totalBytes, Encoding encoding){
            this.responseData = new byte[totalBytes];
            this.encoding = encoding;
            this.state = State.Reading;
        }
        
        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten){
            int responseLength = responseData.Length;

            if (state == State.Reading){
                int bytesToRead = Math.Min(responseLength - offset, (int)Math.Min(dataIn?.Length ?? 0, int.MaxValue));
                
                dataIn?.Read(responseData, offset, bytesToRead);
                offset += bytesToRead;
                
                dataInRead = bytesToRead;
                dataOutWritten = 0;
                
                if (offset >= responseLength){
                    responseData = encoding.GetBytes(ProcessResponse(encoding.GetString(responseData)));
                    state = State.Writing;
                    offset = 0;
                }

                return FilterStatus.NeedMoreData;
            }
            else if (state == State.Writing){
                int bytesToWrite = Math.Min(responseLength - offset, (int)Math.Min(dataOut.Length, int.MaxValue));

                if (bytesToWrite > 0){
                    dataOut.Write(responseData, offset, bytesToWrite);
                    offset += bytesToWrite;
                }

                dataOutWritten = bytesToWrite;
                dataInRead = 0;

                if (offset < responseLength){
                    return FilterStatus.NeedMoreData;
                }
                else{
                    state = State.Done;
                    return FilterStatus.Done;
                }
            }
            else{
                throw new InvalidOperationException("This resource filter cannot be reused.");
            }
        }
        
        public abstract bool InitFilter();
        protected abstract string ProcessResponse(string text);
        public abstract void Dispose();
    }
}
