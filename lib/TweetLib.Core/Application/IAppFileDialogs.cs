using System;
using TweetLib.Core.Systems.Dialogs;

namespace TweetLib.Core.Application {
	public interface IAppFileDialogs {
		void SaveFile(SaveFileDialogSettings settings, Action<string> onAccepted);
	}
}
