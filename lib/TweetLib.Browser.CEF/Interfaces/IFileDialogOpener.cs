using System;
using System.Collections.Generic;

namespace TweetLib.Browser.CEF.Interfaces {
	public interface IFileDialogOpener {
		void OpenFile(string title, bool multiple, List<string> supportedExtensions, Action<string[]> onAccepted, Action onCancelled);
	}
}
