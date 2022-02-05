using System;
using System.Collections.Generic;

namespace TweetLib.Browser.CEF.Data {
	abstract class ContextMenuActionRegistry<T> {
		private readonly Dictionary<T, Action> actions = new ();

		protected abstract T NextId(int n);

		public T AddAction(Action action) {
			T id = NextId(actions.Count);
			actions[id] = action;
			return id;
		}

		public bool Execute(T id) {
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
