using System;
using System.Collections.Generic;
using CefSharp;

namespace TweetDuck.Browser.Adapters {
	sealed class CefContextMenuActionRegistry {
		private readonly Dictionary<CefMenuCommand, Action> actions = new Dictionary<CefMenuCommand, Action>();

		public CefMenuCommand AddAction(Action action) {
			CefMenuCommand id = CefMenuCommand.UserFirst + 500 + actions.Count;
			actions[id] = action;
			return id;
		}

		public bool Execute(CefMenuCommand id) {
			if (actions.TryGetValue(id, out var action)) {
				action();
				return true;
			}

			return false;
		}

		public void Clear() {
			actions.Clear();
		}
	}
}
