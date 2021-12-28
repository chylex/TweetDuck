using TweetLib.Browser.Contexts;

namespace TweetLib.Browser.Interfaces {
	public interface IContextMenuHandler {
		void Show(IContextMenuBuilder menu, Context context);
	}
}
