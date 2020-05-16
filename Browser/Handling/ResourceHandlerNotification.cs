using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using CefSharp;
using CefSharp.Callback;

namespace TweetDuck.Browser.Handling{
    sealed class ResourceHandlerNotification : IResourceHandler{
        private readonly NameValueCollection headers = new NameValueCollection(0);
        private MemoryStream dataIn;

        public void SetHTML(string html){
            dataIn?.Dispose();
            dataIn = ResourceHandler.GetMemoryStream(html, Encoding.UTF8);
        }

        public void Dispose(){
            if (dataIn != null){
                dataIn.Dispose();
                dataIn = null;
            }
        }

        bool IResourceHandler.Open(IRequest request, out bool handleRequest, ICallback callback){
            callback.Dispose();
            handleRequest = true;

            if (dataIn != null){
                dataIn.Position = 0;
            }

            return true;
        }

        void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl){
            redirectUrl = null;

            response.MimeType = "text/html";
            response.StatusCode = 200;
            response.StatusText = "OK";
            response.Headers = headers;
            responseLength = dataIn?.Length ?? 0;
        }

        bool IResourceHandler.Read(Stream dataOut, out int bytesRead, IResourceReadCallback callback){
            callback?.Dispose(); // TODO unnecessary null check once ReadResponse is removed

            try{
                byte[] buffer = new byte[Math.Min(dataIn.Length - dataIn.Position, dataOut.Length)];
                int length = buffer.Length;

                dataIn.Read(buffer, 0, length);
                dataOut.Write(buffer, 0, length);
                bytesRead = length;
            }catch{ // catch IOException, possibly NullReferenceException if dataIn is null
                bytesRead = 0;
            }

            return bytesRead > 0;
        }

        bool IResourceHandler.Skip(long bytesToSkip, out long bytesSkipped, IResourceSkipCallback callback){
            bytesSkipped = -2; // ERR_FAILED
            callback.Dispose();
            return false;
        }

        bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback){
            return ((IResourceHandler)this).Open(request, out bool _, callback);
        }

        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback){
            return ((IResourceHandler)this).Read(dataOut, out bytesRead, null);
        }

        void IResourceHandler.Cancel(){}
    }
}
