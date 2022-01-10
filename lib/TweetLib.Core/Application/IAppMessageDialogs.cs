using TweetLib.Core.Systems.Dialogs;

namespace TweetLib.Core.Application {
	public interface IAppMessageDialogs {
		void Information(string caption, string text, string buttonAccept = Dialogs.OK);
		void Error(string caption, string text, string buttonAccept = Dialogs.OK);
	}
}
