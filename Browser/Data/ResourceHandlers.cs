using System;
using System.Collections.Concurrent;
using CefSharp;

namespace TweetDuck.Browser.Data{
    sealed class ResourceHandlers{
        private readonly ConcurrentDictionary<string, IResourceHandler> handlers = new ConcurrentDictionary<string, IResourceHandler>(StringComparer.OrdinalIgnoreCase);

        public bool HasHandler(IRequest request){
            return handlers.ContainsKey(request.Url);
        }

        public IResourceHandler GetHandler(IRequest request){
            return handlers.TryGetValue(request.Url, out var handler) ? handler : null;
        }

        public bool Register(string url, IResourceHandler handler){
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri)){
                handlers.AddOrUpdate(uri.AbsoluteUri, handler, (key, prev) => handler);
                return true;
            }

            return false;
        }

        public bool Register(ResourceLink link){
            return Register(link.Url, link.Handler);
        }

        public bool Unregister(string url){
            return handlers.TryRemove(url, out IResourceHandler _);
        }

        public bool Unregister(ResourceLink link){
            return Unregister(link.Url);
        }
    }
}
