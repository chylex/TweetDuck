using TweetLib.Browser.Request;

namespace TweetLib.Browser.Interfaces {
	public interface ISchemeResourceVisitor<T> {
		T Status(SchemeResource.Status status);
		T File(SchemeResource.File file);
	}
}
