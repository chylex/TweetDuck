using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TweetDck.Core.Notification;
using TweetDck.Core.Other.Settings;
using TweetDck.Plugins;
using TweetDck.Updates;

namespace TweetDck.Core.Other{
    sealed partial class FormSettings : Form{
        public const int TabIndexNotification = 1;

        private readonly FormBrowser browser;
        private readonly Dictionary<Type, BaseTabSettings> tabs = new Dictionary<Type, BaseTabSettings>(4);
        private readonly bool hasFinishedLoading;

        public FormSettings(FormBrowser browser, PluginManager plugins, UpdateHandler updates, int startTabIndex = 0){
            InitializeComponent();

            Text = Program.BrandName+" Settings";

            this.browser = browser;
            this.browser.PauseNotification();

            this.tabPanel.SetupTabPanel(100);
            this.tabPanel.AddButton("General", SelectTab<TabSettingsGeneral>);
            this.tabPanel.AddButton("Notifications", () => SelectTab(() => new TabSettingsNotifications(browser.CreateNotificationForm(NotificationFlags.DisableContextMenu), !hasFinishedLoading)));
            this.tabPanel.AddButton("Updates", () => SelectTab(() => new TabSettingsUpdates(updates)));
            this.tabPanel.AddButton("Advanced", () => SelectTab(() => new TabSettingsAdvanced(browser.ReinjectCustomCSS, plugins)));

            this.tabPanel.SelectTab(tabPanel.Buttons.ElementAt(startTabIndex));
            hasFinishedLoading = true;
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
            foreach(BaseTabSettings control in tabs.Values){
                control.OnClosing();
            }

            Program.UserConfig.Save();

            foreach(BaseTabSettings control in tabs.Values){
                control.Dispose();
            }

            browser.ResumeNotification();
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
