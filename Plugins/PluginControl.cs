using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Controls;
using TweetDuck.Utils;
using TweetLib.Core;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Core.Systems.Configuration;

namespace TweetDuck.Plugins {
	sealed partial class PluginControl : UserControl {
		private readonly PluginManager pluginManager;
		private readonly Plugin plugin;
		private readonly bool isConfigurable;

		private int nextHeight;

		public PluginControl() {
			InitializeComponent();
		}

		public PluginControl(PluginManager pluginManager, Plugin plugin) : this() {
			this.pluginManager = pluginManager;
			this.plugin = plugin;

			this.isConfigurable = pluginManager.IsPluginConfigurable(plugin);

			float dpiScale = this.GetDPIScale();

			if (dpiScale > 1F) {
				Size = MaximumSize = new Size(MaximumSize.Width, MaximumSize.Height + 3);
			}

			this.labelName.Text = plugin.Name;
			this.labelDescription.Text = plugin.CanRun ? plugin.Description : $"This plugin requires TweetDuck {plugin.RequiredVersion} or newer.";
			this.labelAuthor.Text = string.IsNullOrWhiteSpace(plugin.Author) ? string.Empty : $"by {plugin.Author}";
			this.labelWebsite.Text = plugin.Website;
			this.labelVersion.Text = plugin.Version;

			this.labelType.LineHeight = BrowserUtils.Scale(11, dpiScale);

			UpdatePluginState();

			if (labelDescription.Text.Length == 0) {
				labelDescription.Visible = false;
			}

			panelDescription_Resize(panelDescription, EventArgs.Empty);
		}

		private void timerLayout_Tick(object sender, EventArgs e) {
			timerLayout.Stop();
			Height = nextHeight;
			ResumeLayout();
		}

		private void panelDescription_Resize(object sender, EventArgs e) {
			SuspendLayout();

			int maxWidth = panelDescription.Width - (panelDescription.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);
			labelDescription.MaximumSize = new Size(maxWidth, int.MaxValue);

			Font font = labelDescription.Font;
			int descriptionLines = TextRenderer.MeasureText(labelDescription.Text, font, new Size(maxWidth, int.MaxValue), TextFormatFlags.WordBreak).Height / (font.Height - 1);

			int requiredLines = Math.Max(descriptionLines, 1 + (string.IsNullOrEmpty(labelVersion.Text) ? 0 : 1) + (isConfigurable ? 1 : 0));

			nextHeight = requiredLines switch {
				1 => MaximumSize.Height - 2 * (font.Height - 1),
				2 => MaximumSize.Height - 1 * (font.Height - 1),
				_ => MaximumSize.Height
			};

			if (nextHeight != Height) {
				timerLayout.Start();
			}
			else {
				ResumeLayout();
			}
		}

		private void labelWebsite_Click(object sender, EventArgs e) {
			if (labelWebsite.Text.Length > 0) {
				App.SystemHandler.OpenBrowser(labelWebsite.Text);
			}
		}

		private void btnConfigure_Click(object sender, EventArgs e) {
			pluginManager.ConfigurePlugin(plugin);
			ParentForm?.Close();
		}

		private void btnToggleState_Click(object sender, EventArgs e) {
			pluginManager.Config.SetEnabled(plugin, !pluginManager.Config.IsEnabled(plugin));
			pluginManager.Config.Save();
			UpdatePluginState();
		}

		private void UpdatePluginState() {
			bool isEnabled = pluginManager.Config.IsEnabled(plugin) && plugin.CanRun;
			Color textColor = isEnabled ? Color.Black : Color.FromArgb(90, 90, 90);

			labelVersion.ForeColor = textColor;
			labelAuthor.ForeColor = textColor;
			labelWebsite.ForeColor = isEnabled ? Color.Blue : Color.FromArgb(90, 90, 249);

			if (plugin.Group == PluginGroup.Official) {
				labelType.Text = "CORE";
				labelType.BackColor = isEnabled ? Color.FromArgb(154, 195, 217) : Color.FromArgb(185, 185, 185);
			}
			else {
				labelType.Text = "USER";
				labelType.BackColor = isEnabled ? Color.FromArgb(208, 154, 217) : Color.FromArgb(185, 185, 185);
			}

			if (plugin.CanRun) {
				labelName.ForeColor = textColor;
				labelDescription.ForeColor = textColor;
				btnToggleState.Text = isEnabled ? "Disable" : "Enable";
				btnConfigure.Visible = isConfigurable;
				btnConfigure.Enabled = isEnabled;
			}
			else {
				labelName.ForeColor = Color.DarkRed;
				labelDescription.ForeColor = Color.DarkRed;
				btnToggleState.Visible = false;
				btnConfigure.Visible = false;
			}
		}
	}
}
