using System.Net;
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
            public IResourceHandler Status(HttpStatusCode code, string message){
                return ResourceHandler.ForErrorMessage(message, code);
            }

            public IResourceHandler File(byte[] bytes, string extension){
                if (bytes.Length == 0){
                    return Status(HttpStatusCode.NoContent, "File is empty."); // FromByteArray crashes CEF internals with no contents
                }
                else{
                    return ResourceHandler.FromByteArray(bytes, ResourceHandler.GetMimeType(extension));
                }
            }
        }
    }
}
