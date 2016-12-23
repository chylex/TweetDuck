using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TweetDck.Core.Other.Settings;
using TweetDck.Core.Utils.Notification;
using TweetDck.Plugins;
using TweetDck.Updates;

namespace TweetDck.Core.Other{
    sealed partial class FormSettings : Form{
        private readonly Dictionary<Type, BaseTabSettings> tabs = new Dictionary<Type, BaseTabSettings>(4);

        public FormSettings(FormBrowser browserForm, PluginManager plugins, UpdateHandler updates){
            InitializeComponent();

            Text = Program.BrandName+" Settings";

            this.tabPanel.SetupTabPanel(100);
            this.tabPanel.AddButton("General", SelectTab<TabSettingsGeneral>);
            this.tabPanel.AddButton("Notifications", () => SelectTab(() => new TabSettingsNotifications(browserForm.CreateNotificationForm(NotificationFlags.DisableContextMenu))));
            this.tabPanel.AddButton("Updates", () => SelectTab(() => new TabSettingsUpdates(updates)));
            this.tabPanel.AddButton("Advanced", () => SelectTab(() => new TabSettingsAdvanced(browserForm.ReloadBrowser, plugins)));
            this.tabPanel.SelectTab(tabPanel.Buttons.First());
        }

        private void SelectTab<T>() where T : BaseTabSettings, new(){
            SelectTab(() => new T());
        }

        private void SelectTab<T>(Func<T> constructor) where T : BaseTabSettings{
            BaseTabSettings control;

            if (tabs.TryGetValue(typeof(T), out control)){
                tabPanel.ReplaceContent(control);
            }
            else{
                control = tabs[typeof(T)] = constructor();
                control.Ready = true;
                tabPanel.ReplaceContent(control);
            }
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e){
            Program.UserConfig.Save();

            foreach(BaseTabSettings control in tabs.Values){
                control.Dispose();
            }
        }

        private void btnClose_Click(object sender, EventArgs e){
            Close();
        }

        public void ReloadUI(){
            tabs.Clear();
            tabPanel.Content.Controls.Clear();
            tabPanel.ActiveButton.Callback();
        }
    }
}
