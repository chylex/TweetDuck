using System;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Utils {
	sealed class CefActionTask : CefTask {
		private readonly Action action;

		public CefActionTask(Action action) {
			this.action = action;
		}

		protected override void Execute() {
			action();
		}
	}
}
