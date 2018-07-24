using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Handling.General;
using TweetDuck.Core.Notification.Example;
using TweetDuck.Core.Other.Analytics;
using TweetDuck.Core.Other.Settings;
using TweetDuck.Core.Other.Settings.Dialogs;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;
using TweetDuck.Updates;

namespace TweetDuck.Core.Other{
    sealed partial class FormSettings : Form, FormManager.IAppDialog{
        public bool ShouldReloadBrowser { get; private set; }

        private readonly FormBrowser browser;
        private readonly PluginManager plugins;

        private readonly int buttonHeight;

        private readonly Dictionary<Type, SettingsTab> tabs = new Dictionary<Type, SettingsTab>(8);
        private SettingsTab currentTab;

        public FormSettings(FormBrowser browser, PluginManager plugins, UpdateHandler updates, AnalyticsManager analytics, Type startTab){
            InitializeComponent();

            Text = Program.BrandName+" Options";

            this.browser = browser;
            this.browser.PauseNotification();

            this.plugins = plugins;
            
            this.buttonHeight = BrowserUtils.Scale(39, this.GetDPIScale()) | 1;

            PrepareLoad();

            AddButton("General", () => new TabSettingsGeneral(this.browser.ReloadColumns, updates));
            AddButton("Locales", () => new TabSettingsLocales());
            AddButton("System Tray", () => new TabSettingsTray());
            AddButton("Notifications", () => new TabSettingsNotifications(new FormNotificationExample(this.browser, this.plugins)));
            AddButton("Sounds", () => new TabSettingsSounds(this.browser.PlaySoundNotification));
            AddButton("Feedback", () => new TabSettingsFeedback(analytics, AnalyticsReportGenerator.ExternalInfo.From(this.browser), this.plugins));
            AddButton("Advanced", () => new TabSettingsAdvanced(this.browser.ReinjectCustomCSS, this.browser.OpenDevTools));

            SelectTab(tabs[startTab ?? typeof(TabSettingsGeneral)]);
        }

        private void PrepareLoad(){
            Program.UserConfig.ProgramRestartRequested += Config_ProgramRestartRequested;
            Program.SystemConfig.ProgramRestartRequested += Config_ProgramRestartRequested;
        }

        private void PrepareUnload(){ // TODO refactor this further later
            currentTab.Control.OnClosing();
            
            Program.UserConfig.ProgramRestartRequested -= Config_ProgramRestartRequested;
            Program.SystemConfig.ProgramRestartRequested -= Config_ProgramRestartRequested;

            Program.UserConfig.Save();
            Program.SystemConfig.Save();
        }

        private void Config_ProgramRestartRequested(object sender, EventArgs e){
            if (FormMessage.Information("TweetDuck Options", "The application must restart for the option to take place. Do you want to restart now?", FormMessage.Yes, FormMessage.No)){
                Program.Restart();
            }
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e){
            PrepareUnload();

            foreach(SettingsTab tab in tabs.Values){
                if (tab.IsInitialized){
                    tab.Control.Dispose();
                }
            }
            
            browser.ResumeNotification();
        }

        private void btnManageOptions_Click(object sender, EventArgs e){
            PrepareUnload();

            using(DialogSettingsManage dialog = new DialogSettingsManage(plugins)){
                FormClosing -= FormSettings_FormClosing;
                
                if (dialog.ShowDialog() == DialogResult.OK){
                    if (!dialog.IsRestarting){
                        browser.ResumeNotification();
                    
                        BrowserProcessHandler.UpdatePrefs();
                        ShouldReloadBrowser = dialog.ShouldReloadBrowser;
                    }

                    Close();
                }
                else{
                    FormClosing += FormSettings_FormClosing;
                    PrepareLoad();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e){
            Close();
        }

        private void AddButton<T>(string title, Func<T> constructor) where T : BaseTabSettings{
            FlatButton btn = new FlatButton{
                BackColor = SystemColors.Control,
                FlatStyle = FlatStyle.Flat,
                Font = SystemFonts.MessageBoxFont,
                Location = new Point(0, (buttonHeight+1)*(panelButtons.Controls.Count/2)),
                Margin = new Padding(0),
                Size = new Size(panelButtons.Width, buttonHeight),
                Text = title,
                UseVisualStyleBackColor = true
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(179, 213, 232);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(216, 230, 237);
            
            panelButtons.Controls.Add(btn);

            panelButtons.Controls.Add(new Panel{
                BackColor = Color.DimGray,
                Location = new Point(0, panelButtons.Controls[panelButtons.Controls.Count-1].Location.Y+buttonHeight),
                Margin = new Padding(0),
                Size = new Size(panelButtons.Width, 1)
            });

            tabs.Add(typeof(T), new SettingsTab(btn, constructor));

            btn.Click += (sender, args) => SelectTab<T>();
        }

        private void SelectTab<T>() where T : BaseTabSettings{
            SelectTab(tabs[typeof(T)]);
        }

        private void SelectTab(SettingsTab tab){
            if (currentTab != null){
                currentTab.Button.BackColor = SystemColors.Control;
                currentTab.Control.OnClosing();
            }
            
            tab.Button.BackColor = tab.Button.FlatAppearance.MouseDownBackColor;

            if (!tab.IsInitialized){
                foreach(Control control in tab.Control.InteractiveControls){
                    if (control is ComboBox){
                        control.MouseLeave += control_MouseLeave;
                    }
                    else if (control is TrackBar){
                        control.MouseWheel += control_MouseWheel;
                    }
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

        private void control_MouseLeave(object sender, EventArgs e){
            if (sender is ComboBox cb && cb.DroppedDown){
                return; // prevents comboboxes from closing when MouseLeave event triggers during opening animation
            }

            panelContents.Focus();
        }

        private void control_MouseWheel(object sender, MouseEventArgs e){
            ((HandledMouseEventArgs)e).Handled = true;
            panelContents.Focus();
        }

        private sealed class SettingsTab{
            public Button Button { get; }

            public BaseTabSettings Control => control ?? (control = constructor());
            public bool IsInitialized => control != null;

            private readonly Func<BaseTabSettings> constructor;
            private BaseTabSettings control;

            public SettingsTab(Button button, Func<BaseTabSettings> constructor){
                this.Button = button;
                this.constructor = constructor;
            }
        }
    }
}
