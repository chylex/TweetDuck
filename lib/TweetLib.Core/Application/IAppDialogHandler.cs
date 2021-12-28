using System;
using TweetLib.Core.Systems.Dialogs;

namespace TweetLib.Core.Application {
	public interface IAppDialogHandler {
		void Information(string caption, string text, string buttonAccept, string? buttonCancel = null);
		void Error(string caption, string text, string buttonAccept, string? buttonCancel = null);

		void SaveFile(SaveFileDialogSettings settings, Action<string> onAccepted);
	}
}
