using System;
using TweetDuck.Utils;

namespace TweetDuck.Dialogs.Settings {
	sealed partial class TabSettingsFeedback : FormSettings.BaseTab {
		public TabSettingsFeedback() {
			InitializeComponent();
		}

		public override void OnReady() {
			btnSendFeedback.Click += btnSendFeedback_Click;
		}

		#region Feedback

		private void btnSendFeedback_Click(object sender, EventArgs e) {
			BrowserUtils.OpenExternalBrowser("https://github.com/chylex/TweetDuck/issues/new");
		}

		#endregion
	}
}
