using System;
using CefSharp;

namespace TweetDuck.Browser.Data{
    sealed class ResourceLink{
        public string Url { get; }
        public Func<IResourceHandler> Factory { get; }

        public ResourceLink(string url, Func<IResourceHandler> factory){
            this.Url = url;
            this.Factory = factory;
        }
    }
}
