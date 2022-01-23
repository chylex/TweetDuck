using TweetLib.Browser.CEF.Data;

namespace TweetLib.Browser.CEF.Interfaces {
	public interface IResourceHandlerFactory<T> {
		T CreateResourceHandler(ByteArrayResource resource);
		string GetMimeTypeFromExtension(string extension);
	}
}
