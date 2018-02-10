using CefSharp;

namespace TweetDuck.Data{
    sealed class ResourceLink{
        public string Url { get; }
        public IResourceHandler Handler { get; }

        public ResourceLink(string url, IResourceHandler handler){
            this.Url = url;
            this.Handler = handler;
        }
    }
}
