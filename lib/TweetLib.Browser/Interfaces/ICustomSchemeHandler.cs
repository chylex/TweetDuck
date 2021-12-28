using System;

namespace TweetLib.Browser.Interfaces {
	public interface ICustomSchemeHandler<T> where T : class {
		string Protocol { get; }
		T? Resolve(Uri uri);
	}
}
