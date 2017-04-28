using CefSharp;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace TweetDck.Core.Handling{
    class ResourceHandlerNotification : IResourceHandler{
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

        bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback){
            callback.Continue();
            return true;
        }

        void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl){
            redirectUrl = null;

            response.MimeType = "text/html";
            response.StatusCode = 200;
            response.StatusText = "OK";
            response.ResponseHeaders = headers;
            responseLength = dataIn?.Length ?? -1;
        }

        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback){
            callback.Dispose();

            try{
                int length = (int)dataIn.Length;

                dataIn.CopyTo(dataOut, length);
                bytesRead = length;
                return true;
            }catch{ // catch IOException, possibly NullReferenceException if dataIn is null
                bytesRead = 0;
                return false;
            }
        }

        bool IResourceHandler.CanGetCookie(Cookie cookie){
            return true;
        }

        bool IResourceHandler.CanSetCookie(Cookie cookie){
            return true;
        }

        void IResourceHandler.Cancel(){}
    }
}
