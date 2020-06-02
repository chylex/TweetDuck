using System.Net;

namespace TweetLib.Core.Browser{
    public interface IResourceProvider<T>{
        T Status(HttpStatusCode code, string message);
        T File(byte[] bytes, string extension);
    }
}
