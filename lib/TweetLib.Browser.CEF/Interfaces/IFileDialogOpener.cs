using System;
using System.Collections.Generic;
using TweetLib.Utils.Dialogs;

namespace TweetLib.Browser.CEF.Interfaces {
	public interface IFileDialogOpener {
		void OpenFile(string title, bool multiple, List<FileDialogFilter> filters, Action<string[]> onAccepted, Action onCancelled);
	}
}
