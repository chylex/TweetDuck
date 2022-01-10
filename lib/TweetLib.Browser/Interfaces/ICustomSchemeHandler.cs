using System;
using TweetLib.Browser.Request;

namespace TweetLib.Browser.Interfaces {
	public interface ICustomSchemeHandler {
		string Protocol { get; }
		SchemeResource? Resolve(Uri uri);
	}
}
