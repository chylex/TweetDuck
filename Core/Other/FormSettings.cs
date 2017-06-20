using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other.Settings;
using TweetDuck.Plugins;
using TweetDuck.Updates;

namespace TweetDuck.Core.Other{
    sealed partial class FormSettings : Form{
        private readonly FormBrowser browser;
        private readonly Dictionary<Type, SettingsTab> tabs = new Dictionary<Type, SettingsTab>(4);
        private SettingsTab currentTab;

        public FormSettings(FormBrowser browser, PluginManager plugins, UpdateHandler updates, Type startTab){
            InitializeComponent();

            Text = Program.BrandName+" Options";

            this.browser = browser;
            this.browser.PauseNotification();

            AddButton("General", () => new TabSettingsGeneral(updates));
            AddButton("Notifications", () => new TabSettingsNotifications(browser.CreateNotificationForm(false)));
            AddButton("Sounds", () => new TabSettingsSounds());
            AddButton("Advanced", () => new TabSettingsAdvanced(browser.ReinjectCustomCSS, plugins));

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

        private void btnClose_Click(object sender, EventArgs e){
            Close();
        }

        private void AddButton<T>(string title, Func<T> constructor) where T : BaseTabSettings{
            const int btnHeight = 39;

            FlatButton btn = new FlatButton{
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, (btnHeight+1)*(panelButtons.Controls.Count/2)),
                Margin = new Padding(0),
                Size = new Size(panelButtons.Width, btnHeight),
                Text = title,
                UseVisualStyleBackColor = true
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color.White;
            btn.FlatAppearance.MouseOverBackColor = Color.White;
            
            panelButtons.Controls.Add(btn);

            panelButtons.Controls.Add(new Panel{
                BackColor = Color.DimGray,
                Location = new Point(0, panelButtons.Controls[panelButtons.Controls.Count-1].Location.Y+btnHeight+1),
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
            
            tab.Button.BackColor = Color.White;

            if (!tab.IsInitialized){
                tab.Control.OnReady();
            }
            
            panelContents.SuspendLayout();
            panelContents.Controls.Clear();
            panelContents.Controls.Add(tab.Control);
            panelContents.ResumeLayout(true);

            currentTab = tab;
        }

        public void ReloadUI(){
            foreach(SettingsTab tab in tabs.Values){
                tab.Control = null;
            }

            SelectTab(currentTab);
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
