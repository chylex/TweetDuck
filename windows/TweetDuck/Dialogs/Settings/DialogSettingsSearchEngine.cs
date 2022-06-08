using System;
using System.Windows.Forms;

namespace TweetDuck.Dialogs.Settings {
	sealed partial class DialogSettingsSearchEngine : Form {
		public string Url => textBoxUrl.Text;

		public DialogSettingsSearchEngine() {
			InitializeComponent();

			Text = Program.BrandName + " Options - Custom Search Engine";

			textBoxUrl.Text = Program.Config.User.SearchEngineUrl ?? "";
			textBoxUrl.Select(textBoxUrl.Text.Length, 0);
		}

		private void btnApply_Click(object? sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object? sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
