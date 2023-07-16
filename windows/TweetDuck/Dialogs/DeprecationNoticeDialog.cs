using System;
using System.Windows.Forms;
using TweetLib.Core;

namespace TweetDuck.Dialogs;

static class DeprecationNoticeDialog {
	public static bool Show() {
		const string contents = """
					TweetDuck is no longer being maintained:
					 - Twitter has been constantly breaking TweetDeck and therefore also breaking TweetDuck.
					 - Twitter will be replacing TweetDeck with a new version that is incompatible with most of the app's features.
					 - Twitter is planning to put TweetDeck behind a subscription paywall.
					
					There will be no more updates.
					Continue at your own risk.
					""";

		using FormMessage message = new FormMessage("TweetDuck Deprecation Notice", contents, MessageBoxIcon.Warning);

		message.AddButton("Exit", DialogResult.Cancel, ControlType.Cancel);
		message.AddButton("Continue", DialogResult.OK, ControlType.Accept | ControlType.Focused);

		Button btnLearnMore = message.CreateButton("Learn More", x: 9, width: 106);
		btnLearnMore.Anchor |= AnchorStyles.Left;
		btnLearnMore.Margin = new Padding(0, 0, 48, 0);
		btnLearnMore.Click += OnBtnLearnMoreClick;
		message.AddActionControl(btnLearnMore);

		return message.ShowDialog() == DialogResult.OK;
	}

	private static void OnBtnLearnMoreClick(object? sender, EventArgs args) {
		App.SystemHandler.OpenBrowser(Program.Website + "/deprecation");
	}
}
