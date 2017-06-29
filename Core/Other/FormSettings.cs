using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other.Settings;
using TweetDuck.Core.Other.Settings.Dialogs;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;
using TweetDuck.Updates;

namespace TweetDuck.Core.Other{
    sealed partial class FormSettings : Form{
        private readonly FormBrowser browser;
        private readonly PluginManager plugins;

        private readonly int buttonHeight;

        private readonly Dictionary<Type, SettingsTab> tabs = new Dictionary<Type, SettingsTab>(4);
        private SettingsTab currentTab;

        public FormSettings(FormBrowser browser, PluginManager plugins, UpdateHandler updates, Type startTab){
            InitializeComponent();

            Text = Program.BrandName+" Options";

            this.browser = browser;
            this.browser.PauseNotification();

            this.plugins = plugins;
            
            this.buttonHeight = BrowserUtils.Scale(39, this.GetDPIScale()) | 1;

            AddButton("General", () => new TabSettingsGeneral(updates));
            AddButton("Notifications", () => new TabSettingsNotifications(browser.CreateNotificationForm(false)));
            AddButton("Sounds", () => new TabSettingsSounds());
            AddButton("Advanced", () => new TabSettingsAdvanced(browser.ReinjectCustomCSS));

            SelectTab(tabs[startTab ?? typeof(TabSettingsGeneral)]);
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e){
            foreach(SettingsTab tab in tabs.Values){
                if (tab.IsInitialized){
                    tab.Control.OnClosing();
                    tab.Control.Dispose();
                }
            }
            
            Program.UserConfig.Save();
            browser.ResumeNotification();
        }

        private void btnManageOptions_Click(object sender, EventArgs e){
            using(DialogSettingsManage dialog = new DialogSettingsManage(plugins)){
                if (dialog.ShowDialog() == DialogResult.OK && dialog.ShouldReloadUI){
                    foreach(SettingsTab tab in tabs.Values){
                        tab.Control = null;
                    }

                    SelectTab(currentTab);
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
            
            panelContents.SuspendLayout();
            panelContents.VerticalScroll.Value = 0; // https://gfycat.com/GrotesqueTastyAstarte
            panelContents.Controls.Clear();
            panelContents.Controls.Add(tab.Control);
            panelContents.ResumeLayout(true);
            panelContents.Focus();

            currentTab = tab;
        }

        private void control_MouseLeave(object sender, EventArgs e){
            panelContents.Focus();
        }

        private void control_MouseWheel(object sender, MouseEventArgs e){
            ((HandledMouseEventArgs)e).Handled = true;
            panelContents.Focus();
        }

        private class SettingsTab{
            public Button Button { get; }

            public BaseTabSettings Control{
                get => control ?? (control = constructor());
                set => control = value;
            }

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
