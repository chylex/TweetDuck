using System;
using TweetLib.Browser.CEF.Dialogs;

namespace TweetLib.Browser.CEF.Interfaces {
	public interface IJsDialogOpener {
		void Alert(MessageDialogType type, string title, string message, Action<bool> callback);
		void Confirm(MessageDialogType type, string title, string message, Action<bool> callback);
		void Prompt(MessageDialogType type, string title, string message, Action<bool, string> callback);
	}
}
