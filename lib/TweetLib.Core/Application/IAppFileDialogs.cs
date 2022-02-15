using System;
using TweetLib.Utils.Dialogs;

namespace TweetLib.Core.Application {
	public interface IAppFileDialogs {
		void SaveFile(SaveFileDialogSettings settings, Action<string> onAccepted);
	}
}
