﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Browser;
using TweetDuck.Browser.Base;
using TweetDuck.Browser.Notification;
using TweetDuck.Configuration;
using TweetDuck.Controls;
using TweetDuck.Dialogs.Settings;
using TweetDuck.Management;
using TweetDuck.Utils;
using TweetLib.Core;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Systems.Updates;

namespace TweetDuck.Dialogs {
	sealed partial class FormSettings : Form, FormManager.IAppDialog {
		public bool ShouldReloadBrowser { get; private set; }

		private readonly FormBrowser browser;
		private readonly PluginManager plugins;

		private readonly int buttonHeight;

		private readonly Dictionary<Type, SettingsTab> tabs = new (8);
		private SettingsTab? currentTab;

		public FormSettings(FormBrowser browser, PluginManager plugins, UpdateChecker updates, TweetDeckFunctions tweetDeckFunctions, Type? startTab) {
			InitializeComponent();

			Text = Program.BrandName + " Options";

			this.browser = browser;
			this.browser.PauseNotification(NotificationPauseReason.SettingsDialogOpen);

			this.plugins = plugins;

			this.buttonHeight = BrowserUtils.Scale(39, this.GetDPIScale()) | 1;

			PrepareLoad();

			AddButton("General", () => new TabSettingsGeneral(this.browser.ReloadToTweetDeck, tweetDeckFunctions.ReloadColumns, updates));
			AddButton("Notifications", () => new TabSettingsNotifications(this.browser.CreateExampleNotification()));
			AddButton("Sounds", () => new TabSettingsSounds(() => tweetDeckFunctions.PlaySoundNotification(true)));
			AddButton("Tray", static () => new TabSettingsTray());
			AddButton("Feedback", static () => new TabSettingsFeedback());
			AddButton("Advanced", () => new TabSettingsAdvanced(tweetDeckFunctions.ReinjectCustomCSS, this.browser.OpenDevTools));

			SelectTab(tabs[startTab ?? typeof(TabSettingsGeneral)]);
		}

		private void PrepareLoad() {
			App.ConfigManager.ProgramRestartRequested += Config_ProgramRestartRequested;
		}

		private void PrepareUnload() { // TODO refactor this further later
			currentTab?.Control.OnClosing();

			App.ConfigManager.ProgramRestartRequested -= Config_ProgramRestartRequested;
			App.ConfigManager.SaveAll();
		}

		private void Config_ProgramRestartRequested(object? sender, EventArgs e) {
			if (FormMessage.Information("TweetDuck Options", "The application must restart for the option to take place. Do you want to restart now?", FormMessage.Yes, FormMessage.No)) {
				Program.Restart();
			}
		}

		private void FormSettings_FormClosing(object? sender, FormClosingEventArgs e) {
			PrepareUnload();

			foreach (SettingsTab tab in tabs.Values) {
				if (tab.IsInitialized) {
					tab.Control.Dispose();
				}
			}

			browser.ResumeNotification(NotificationPauseReason.SettingsDialogOpen);
		}

		private void btnManageOptions_Click(object? sender, EventArgs e) {
			PrepareUnload();

			using DialogSettingsManage dialog = new DialogSettingsManage(plugins);
			FormClosing -= FormSettings_FormClosing;

			if (dialog.ShowDialog() == DialogResult.OK) {
				if (!dialog.IsRestarting) {
					browser.ResumeNotification(NotificationPauseReason.SettingsDialogOpen);

					if (dialog.ShouldReloadBrowser) {
						BrowserProcessHandler.UpdatePrefs();
						ShouldReloadBrowser = true;
					}
				}

				Close();
			}
			else {
				FormClosing += FormSettings_FormClosing;
				PrepareLoad();
			}
		}

		private void btnClose_Click(object? sender, EventArgs e) {
			Close();
		}

		private void AddButton<T>(string title, Func<T> constructor) where T : BaseTab {
			FlatButton btn = new FlatButton {
				BackColor = SystemColors.Control,
				FlatStyle = FlatStyle.Flat,
				Font = SystemFonts.MessageBoxFont,
				Location = new Point(0, (buttonHeight + 1) * (panelButtons.Controls.Count / 2)),
				Margin = new Padding(0),
				Size = new Size(panelButtons.Width, buttonHeight),
				Text = title,
				UseVisualStyleBackColor = true
			};

			btn.FlatAppearance.BorderSize = 0;
			btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(179, 213, 232);
			btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(216, 230, 237);

			panelButtons.Controls.Add(btn);

			panelButtons.Controls.Add(new Panel {
				BackColor = Color.DimGray,
				Location = new Point(0, panelButtons.Controls[^1].Location.Y + buttonHeight),
				Margin = new Padding(0),
				Size = new Size(panelButtons.Width, 1)
			});

			tabs.Add(typeof(T), new SettingsTab(btn, constructor));

			btn.Click += (_, _) => SelectTab<T>();
		}

		private void SelectTab<T>() where T : BaseTab {
			SelectTab(tabs[typeof(T)]);
		}

		private void SelectTab(SettingsTab tab) {
			if (currentTab != null) {
				currentTab.Button.BackColor = SystemColors.Control;
				currentTab.Control.OnClosing();
			}

			tab.Button.BackColor = tab.Button.FlatAppearance.MouseDownBackColor;

			if (!tab.IsInitialized) {
				foreach (Control control in tab.Control.InteractiveControls) {
					if (control is ComboBox) {
						control.MouseLeave += control_MouseLeave;
					}
					else if (control is TrackBar) {
						control.MouseWheel += control_MouseWheel;
					}
				}

				if (tab.Control.Height < panelContents.Height - 2) {
					tab.Control.Height = panelContents.Height - 2; // fixes off-by-pixel error on high DPI
				}

				tab.Control.OnReady();
			}

			panelContents.VerticalScroll.Enabled = false; // required to stop animation that would otherwise break everything
			panelContents.PerformLayout();

			panelContents.SuspendLayout();
			panelContents.VerticalScroll.Value = 0; // https://gfycat.com/GrotesqueTastyAstarte
			panelContents.Controls.Clear();
			panelContents.Controls.Add(tab.Control);
			panelContents.ResumeLayout(true);

			panelContents.VerticalScroll.Enabled = true;
			panelContents.Focus();

			currentTab = tab;
		}

		private void control_MouseLeave(object? sender, EventArgs e) {
			if (sender is ComboBox { DroppedDown: true } ) {
				return; // prevents comboboxes from closing when MouseLeave event triggers during opening animation
			}

			panelContents.Focus();
		}

		private void control_MouseWheel(object? sender, MouseEventArgs e) {
			((HandledMouseEventArgs) e).Handled = true;
			panelContents.Focus();
		}

		private sealed class SettingsTab {
			public Button Button { get; }

			public BaseTab Control => control ??= constructor();
			public bool IsInitialized => control != null;

			private readonly Func<BaseTab> constructor;
			private BaseTab? control;

			public SettingsTab(Button button, Func<BaseTab> constructor) {
				this.Button = button;
				this.constructor = constructor;
			}
		}

		internal class BaseTab : UserControl {
			protected static UserConfig Config => Program.Config.User;
			protected static SystemConfig SysConfig => Program.Config.System;

			public IEnumerable<Control> InteractiveControls {
				get {
					static IEnumerable<Control> FindInteractiveControls(Control parent) {
						foreach (Control control in parent.Controls) {
							if (control is Panel subPanel) {
								foreach (Control subControl in FindInteractiveControls(subPanel)) {
									yield return subControl;
								}
							}
							else {
								yield return control;
							}
						}
					}

					return FindInteractiveControls(this);
				}
			}

			protected BaseTab() {
				Padding = new Padding(6);
			}

			public virtual void OnReady() {}
			public virtual void OnClosing() {}
		}
	}
}
