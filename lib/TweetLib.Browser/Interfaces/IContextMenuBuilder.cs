using System;

namespace TweetLib.Browser.Interfaces {
	public interface IContextMenuBuilder {
		void AddAction(string name, Action action);
		void AddActionWithCheck(string name, bool isChecked, Action action);
		void AddSeparator();
	}
}
