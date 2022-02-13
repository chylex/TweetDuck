using TweetDuck.Dialogs;
using TweetDuck.Management;
using TweetLib.Core.Application;

namespace TweetDuck.Application {
	sealed class MessageDialogs : IAppMessageDialogs {
		public void Information(string caption, string text) {
			FormManager.RunOnUIThreadAsync(() => FormMessage.Information(caption, text, FormMessage.OK));
		}

		public void Error(string caption, string text) {
			FormManager.RunOnUIThreadAsync(() => FormMessage.Error(caption, text, FormMessage.OK));
		}
	}
}
