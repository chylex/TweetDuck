using TweetDuck.Dialogs;
using TweetDuck.Management;
using TweetLib.Core.Application;

namespace TweetDuck.Application {
	sealed class MessageDialogs : IAppMessageDialogs {
		public void Information(string caption, string text, string buttonAccept) {
			FormManager.RunOnUIThreadAsync(() => FormMessage.Information(caption, text, buttonAccept));
		}

		public void Error(string caption, string text, string buttonAccept) {
			FormManager.RunOnUIThreadAsync(() => FormMessage.Error(caption, text, buttonAccept));
		}
	}
}
