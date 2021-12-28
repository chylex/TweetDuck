using TweetLib.Browser.Interfaces;

namespace TweetLib.Browser.Base {
	public sealed class BrowserSetup {
		public IContextMenuHandler? ContextMenuHandler { get; set; }
		public IResourceRequestHandler? ResourceRequestHandler { get; set; }
	}
}
