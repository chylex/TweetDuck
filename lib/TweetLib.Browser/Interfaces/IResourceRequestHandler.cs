using TweetLib.Browser.Request;

namespace TweetLib.Browser.Interfaces {
	public interface IResourceRequestHandler {
		RequestHandleResult? Handle(string url, ResourceType resourceType);
	}
}
