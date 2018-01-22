using System;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Management;
using TweetDuck.Plugins;

namespace TweetDuck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsManage : Form{
        private enum State{
            Deciding, Reset, Import, Export
        }

        private ProfileManager.Items SelectedItems{
            get => _selectedItems;

            set{
                // this will call events and SetFlag, which also updates the UI
                cbConfig.Checked = value.HasFlag(ProfileManager.Items.UserConfig);
                cbSession.Checked = value.HasFlag(ProfileManager.Items.Session);
                cbPluginData.Checked = value.HasFlag(ProfileManager.Items.PluginData);
            }
        }

        public bool ShouldReloadBrowser { get; private set; }
        
        private readonly PluginManager plugins;

        private State currentState;
        private ProfileManager importManager;

        private ProfileManager.Items _selectedItems = ProfileManager.Items.None;

        public DialogSettingsManage(PluginManager plugins){
            InitializeComponent();

            this.plugins = plugins;
            this.currentState = State.Deciding;
        }

        private void radioDecision_CheckedChanged(object sender, EventArgs e){
            btnContinue.Enabled = true;
        }

        private void cbConfig_CheckedChanged(object sender, EventArgs e){
            SetFlag(ProfileManager.Items.UserConfig, cbConfig.Checked);
        }

        private void cbSession_CheckedChanged(object sender, EventArgs e){
            SetFlag(ProfileManager.Items.Session, cbSession.Checked);
        }

        private void cbPluginData_CheckedChanged(object sender, EventArgs e){
            SetFlag(ProfileManager.Items.PluginData, cbPluginData.Checked);
        }

        private void btnContinue_Click(object sender, EventArgs e){
            string file;

            switch(currentState){
                case State.Deciding:
                    // Reset
                    if (radioReset.Checked){
                        currentState = State.Reset;

                        Text = "Restore Defaults";
                        SelectedItems = ProfileManager.Items.UserConfig;
                    }

                    // Import
                    else if (radioImport.Checked){
                        using(OpenFileDialog dialog = new OpenFileDialog{
                            AutoUpgradeEnabled = true,
                            DereferenceLinks = true,
                            Title = "Import TweetDuck Profile",
                            Filter = "TweetDuck Profile (*.tdsettings)|*.tdsettings"
                        }){
                            if (dialog.ShowDialog() != DialogResult.OK){
                                return;
                            }

                            file = dialog.FileName;
                        }

                        importManager = new ProfileManager(file, plugins);
                        currentState = State.Import;

                        Text = "Import Profile";
                        SelectedItems = importManager.FindImportItems();

                        cbConfig.Enabled = cbConfig.Checked;
                        cbSession.Enabled = cbSession.Checked;
                        cbPluginData.Enabled = cbPluginData.Checked;
                    }

                    // Export
                    else if (radioExport.Checked){
                        currentState = State.Export;

                        Text = "Export Profile";
                        btnContinue.Text = "Export Profile";
                        SelectedItems = ProfileManager.Items.All & ~ProfileManager.Items.Session;
                    }
                    
                    // Continue...
                    panelDecision.Visible = false;
                    panelExport.Visible = true;
                    break;

                case State.Reset:
                    if (FormMessage.Warning("Reset TweetDuck Options", "This will reset the selected items. Are you sure you want to proceed?", FormMessage.Yes, FormMessage.No)){
                        if (SelectedItems.HasFlag(ProfileManager.Items.UserConfig)){
                            Program.UserConfig.Reset();
                        }

                        if (SelectedItems.HasFlag(ProfileManager.Items.SystemConfig)){
                            try{
                                File.Delete(Program.SystemConfigFilePath);
                            }catch(Exception ex){
                                Program.Reporter.HandleException("System Config Reset Error", "Could not delete system config.", true, ex);
                            }
                        }

                        if (SelectedItems.HasFlag(ProfileManager.Items.PluginData)){
                            try{
                                File.Delete(Program.PluginConfigFilePath);
                                Directory.Delete(Program.PluginDataPath, true);
                            }catch(Exception ex){
                                Program.Reporter.HandleException("Plugin Data Reset Error", "Could not delete plugin data.", true, ex);
                            }
                        }

                        if (SelectedItems.HasFlag(ProfileManager.Items.Session)){
                            Program.Restart(Arguments.ArgDeleteCookies);
                        }
                        else if (SelectedItems.HasFlag(ProfileManager.Items.SystemConfig)){
                            Program.Restart();
                        }
                        else{
                            ShouldReloadBrowser = true;
                        }

                        DialogResult = DialogResult.OK;
                        Close();
                    }

                    break;

                case State.Import:
                    if (importManager.Import(SelectedItems)){
                        Program.UserConfig.Reload();

                        if (importManager.IsRestarting){
                            if (SelectedItems.HasFlag(ProfileManager.Items.Session)){
                                Program.Restart(Arguments.ArgImportCookies);
                            }
                            else if (SelectedItems.HasFlag(ProfileManager.Items.SystemConfig)){
                                Program.Restart();
                            }
                        }
                        else{
                            ShouldReloadBrowser = true;
                        }
                    }
                    else{
                        Program.Reporter.HandleException("Profile Import Error", "An exception happened while importing TweetDuck profile.", true, importManager.LastException);
                    }
                    
                    DialogResult = DialogResult.OK;
                    Close();
                    break;

                case State.Export:
                    using(SaveFileDialog dialog = new SaveFileDialog{
                        AddExtension = true,
                        AutoUpgradeEnabled = true,
                        OverwritePrompt = true,
                        DefaultExt = "tdsettings",
                        FileName = "TweetDuck.tdsettings",
                        Title = "Export TweetDuck Profile",
                        Filter = "TweetDuck Profile (*.tdsettings)|*.tdsettings"
                    }){
                        if (dialog.ShowDialog() != DialogResult.OK){
                            return;
                        }

                        file = dialog.FileName;
                    }

                    Program.UserConfig.Save();
                    Program.SystemConfig.Save();

                    ProfileManager manager = new ProfileManager(file, plugins);

                    if (!manager.Export(SelectedItems)){
                        Program.Reporter.HandleException("Profile Export Error", "An exception happened while exporting TweetDuck profile.", true, manager.LastException);
                    }

                    DialogResult = DialogResult.OK;
                    Close();
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e){
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SetFlag(ProfileManager.Items flag, bool enable){
            _selectedItems = enable ? _selectedItems | flag : _selectedItems & ~flag;
            btnContinue.Enabled = _selectedItems != ProfileManager.Items.None;

            if (currentState == State.Import){
                btnContinue.Text = _selectedItems.HasFlag(ProfileManager.Items.Session) ? "Import && Restart" : "Import Profile";
            }
            else if (currentState == State.Reset){
                btnContinue.Text = _selectedItems.HasFlag(ProfileManager.Items.Session) ? "Restore && Restart" : "Restore Defaults";
            }
        }
    }
}
