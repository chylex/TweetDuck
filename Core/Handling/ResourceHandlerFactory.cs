using System;
using System.Collections.Concurrent;
using CefSharp;
using TweetDuck.Data;

namespace TweetDuck.Core.Handling{
    sealed class ResourceHandlerFactory : IResourceHandlerFactory{
        public bool HasHandlers => !handlers.IsEmpty;

        private readonly ConcurrentDictionary<string, IResourceHandler> handlers = new ConcurrentDictionary<string, IResourceHandler>(StringComparer.OrdinalIgnoreCase);

        public IResourceHandler GetResourceHandler(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request){
            try{
                return handlers.TryGetValue(request.Url, out IResourceHandler handler) ? handler : null;
            }finally{
                request.Dispose();
            }
        }

        // registration

        public bool RegisterHandler(string url, IResourceHandler handler){
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri)){
                handlers.AddOrUpdate(uri.AbsoluteUri, handler, (key, prev) => handler);
                return true;
            }

            return false;
        }

        public bool RegisterHandler(ResourceLink link){
            return RegisterHandler(link.Url, link.Handler);
        }

        public bool UnregisterHandler(string url){
            return handlers.TryRemove(url, out IResourceHandler _);
        }

        public bool UnregisterHandler(ResourceLink link){
            return UnregisterHandler(link.Url);
        }
    }
}
