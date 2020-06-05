using System.IO;
using System.Net;
using System.Text;
using CefSharp;
using TweetLib.Core.Browser;
using TweetLib.Core.Features.Plugins;

namespace TweetDuck.Plugins{
    sealed class PluginSchemeFactory : ISchemeHandlerFactory{
        public const string Name = PluginSchemeHandler<IResourceHandler>.Name;

        private readonly PluginSchemeHandler<IResourceHandler> handler = new PluginSchemeHandler<IResourceHandler>(new ResourceProvider());

        internal void Setup(PluginManager plugins){
            handler.Setup(plugins);
        }

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request){
            return handler.Process(request.Url);
        }

        private sealed class ResourceProvider : IResourceProvider<IResourceHandler>{
            private static ResourceHandler CreateHandler(byte[] bytes){
                var handler = ResourceHandler.FromStream(new MemoryStream(bytes), autoDisposeStream: true);
                handler.Headers.Set("Access-Control-Allow-Origin", "*");
                return handler;
            }

            public IResourceHandler Status(HttpStatusCode code, string message){
                var handler = CreateHandler(Encoding.UTF8.GetBytes(message));
                handler.StatusCode = (int)code;
                return handler;
            }

            public IResourceHandler File(byte[] bytes, string extension){
                if (bytes.Length == 0){
                    return Status(HttpStatusCode.NoContent, "File is empty."); // FromByteArray crashes CEF internals with no contents
                }
                else{
                    var handler = CreateHandler(bytes);
                    handler.MimeType = Cef.GetMimeType(extension);
                    return handler;
                }
            }
        }
    }
}
