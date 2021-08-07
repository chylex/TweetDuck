using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using TweetDuck.Controls;

namespace TweetDuck.Browser.Notification.Screenshot {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	sealed class ScreenshotBridge {
		private readonly Control owner;

		private readonly Action<int> safeSetHeight;
		private readonly Action safeTriggerScreenshot;

		public ScreenshotBridge(Control owner, Action<int> safeSetHeight, Action safeTriggerScreenshot) {
			this.owner = owner;
			this.safeSetHeight = safeSetHeight;
			this.safeTriggerScreenshot = safeTriggerScreenshot;
		}

		public void SetHeight(int tweetHeight) {
			owner.InvokeSafe(() => safeSetHeight(tweetHeight));
		}

		public void TriggerScreenshot() {
			owner.InvokeSafe(safeTriggerScreenshot);
		}
	}
}
