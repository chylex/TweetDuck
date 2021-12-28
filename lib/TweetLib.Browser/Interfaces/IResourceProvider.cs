using System.Net;

namespace TweetLib.Browser.Interfaces {
	public interface IResourceProvider<T> {
		T Status(HttpStatusCode code, string message);
		T File(byte[] contents, string extension);
	}
}
