using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TweetDck.Configuration;
using TweetDck.Core.Other.Settings;
using TweetDck.Core.Other.Settings.Export;
using TweetDck.Updates;

namespace TweetDck.Core.Other{
    sealed partial class FormSettings : Form{
        private readonly Dictionary<Type,BaseTabSettings> tabs = new Dictionary<Type,BaseTabSettings>(4);

        public FormSettings(FormBrowser browserForm, UpdateHandler updates){
            InitializeComponent();

            Text = Program.BrandName+" Settings";

            this.tabPanel.SetupTabPanel(100);
            this.tabPanel.AddButton("General",SelectTab<TabSettingsGeneral>);
            this.tabPanel.AddButton("Notifications",() => SelectTab(() => new TabSettingsNotifications(browserForm.CreateNotificationForm(false))));
            this.tabPanel.AddButton("Updates",() => SelectTab(() => new TabSettingsUpdates(updates)));
            this.tabPanel.AddButton("Advanced",SelectTab<TabSettingsAdvanced>);
            this.tabPanel.SelectTab(tabPanel.Buttons.First());
        }

        private void SelectTab<T>() where T : BaseTabSettings, new(){
            SelectTab(() => new T());
        }

        private void SelectTab<T>(Func<T> constructor) where T : BaseTabSettings{
            BaseTabSettings control;

            if (tabs.TryGetValue(typeof(T),out control)){
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
        }

        private void btnExport_Click(object sender, EventArgs e){
            DialogResult resultSaveCredentials = MessageBox.Show("Do you want to include your login session? This will not save your password into the file, but it will allow anyone with the file to login into TweetDeck as you.","Export "+Program.BrandName+" Settings",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question,MessageBoxDefaultButton.Button3);
            if (resultSaveCredentials == DialogResult.Cancel)return;

            bool saveCredentials = resultSaveCredentials == DialogResult.Yes;
            string file;

            using(SaveFileDialog dialog = new SaveFileDialog{
                AddExtension = true,
                AutoUpgradeEnabled = true,
                OverwritePrompt = true,
                DefaultExt = "tdsettings",
                FileName = Program.BrandName+".tdsettings",
                Title = "Export "+Program.BrandName+" Settings",
                Filter = Program.BrandName+" Settings (*.tdsettings)|*.tdsettings"
            }){
                file = dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }

            if (file != null){
                Program.UserConfig.Save();

                ExportManager manager = new ExportManager(file);

                if (!manager.Export(saveCredentials)){
                    Program.HandleException("An exception happened while exporting "+Program.BrandName+" settings.",manager.LastException);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e){
            string file;

            using(OpenFileDialog dialog = new OpenFileDialog{
                AutoUpgradeEnabled = true,
                DereferenceLinks = true,
                Title = "Import "+Program.BrandName+" Settings",
                Filter = Program.BrandName+" Settings (*.tdsettings)|*.tdsettings"
            }){
                file = dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }

            if (file != null){
                ExportManager manager = new ExportManager(file);

                if (manager.Import()){
                    ReloadUI();
                }
                else{
                    Program.HandleException("An exception happened while importing "+Program.BrandName+" settings.",manager.LastException);
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e){
            if (MessageBox.Show("This will reset all of your settings, including disabled plugins. Do you want to proceed?","Reset "+Program.BrandName+" Settings",MessageBoxButtons.YesNo,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                Program.ResetConfig();
                ReloadUI();
            }
        }

        private void btnClose_Click(object sender, EventArgs e){
            Close();
        }

        private void ReloadUI(){
            tabs.Clear();
            tabPanel.Content.Controls.Clear();
            tabPanel.ActiveButton.Callback();
        }
    }
}
