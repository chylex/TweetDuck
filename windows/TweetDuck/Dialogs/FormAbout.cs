using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Management;
using TweetLib.Core;

namespace TweetDuck.Dialogs {
	sealed partial class FormAbout : Form, FormManager.IAppDialog {
		private const string TipsLink = "https://github.com/chylex/TweetDuck/wiki";

		public FormAbout() {
			InitializeComponent();

			Text = "About " + Program.BrandName + " " + Program.VersionTag;

			labelDescription.Text = "TweetDuck was created by chylex as a replacement to the discontinued official TweetDeck client for Windows.\n\nThe program is available for free under the open source MIT license.";

			labelWebsite.Links.Add(new LinkLabel.Link(0, labelWebsite.Text.Length, Program.Website));
			labelTips.Links.Add(new LinkLabel.Link(0, labelTips.Text.Length, TipsLink));
			labelIssues.Links.Add(new LinkLabel.Link(0, labelIssues.Text.Length, Lib.IssueTrackerUrl));

			try {
				pictureLogo.Image = Image.FromFile(App.LogoPath);
			} catch (Exception) {
				// ignore
			}
		}

		private void OnLinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) {
			App.SystemHandler.OpenBrowser(e.Link.LinkData as string);
		}

		private void FormAbout_HelpRequested(object? sender, HelpEventArgs hlpevent) {
			ShowGuide();
		}

		private void FormAbout_HelpButtonClicked(object? sender, CancelEventArgs e) {
			e.Cancel = true;
			ShowGuide();
		}

		private void ShowGuide() {
			FormGuide.Show();
			Close();
		}
	}
}
