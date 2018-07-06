using System;
using System.Collections.Generic;
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
                foreach(KeyValuePair<CheckBox, ProfileManager.Items> kvp in checkBoxMap){
                    kvp.Key.Checked = value.HasFlag(kvp.Value);
                }
            }
        }
        
        public bool IsRestarting { get; private set; }
        public bool ShouldReloadBrowser { get; private set; }
        
        private readonly PluginManager plugins;
        private readonly Dictionary<CheckBox, ProfileManager.Items> checkBoxMap = new Dictionary<CheckBox, ProfileManager.Items>(4);

        private State currentState;
        private ProfileManager importManager;

        private ProfileManager.Items _selectedItems = ProfileManager.Items.None;

        public DialogSettingsManage(PluginManager plugins, bool openImportImmediately = false){
            InitializeComponent();

            this.plugins = plugins;
            this.currentState = State.Deciding;

            this.checkBoxMap[cbProgramConfig] = ProfileManager.Items.UserConfig;
            this.checkBoxMap[cbSystemConfig] = ProfileManager.Items.SystemConfig;
            this.checkBoxMap[cbSession] = ProfileManager.Items.Session;
            this.checkBoxMap[cbPluginData] = ProfileManager.Items.PluginData;

            if (openImportImmediately){
                radioImport.Checked = true;
                btnContinue_Click(null, EventArgs.Empty);
            }
        }

        private void radioDecision_CheckedChanged(object sender, EventArgs e){
            btnContinue.Enabled = true;
        }

        private void checkBoxSelection_CheckedChanged(object sender, EventArgs e){
            CheckBox cb = (CheckBox)sender;
            SetFlag(checkBoxMap[cb], cb.Checked);
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

                        foreach(CheckBox cb in checkBoxMap.Keys){
                            cb.Enabled = cb.Checked;
                        }
                    }

                    // Export
                    else if (radioExport.Checked){
                        currentState = State.Export;

                        Text = "Export Profile";
                        btnContinue.Text = "Export Profile";
                        SelectedItems = ProfileManager.Items.UserConfig | ProfileManager.Items.PluginData;
                    }
                    
                    // Continue...
                    panelDecision.Visible = false;
                    panelSelection.Visible = true;
                    Height += panelSelection.Height-panelDecision.Height;
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
                            RestartProgram(Arguments.ArgDeleteCookies);
                        }
                        else if (SelectedItems.HasFlag(ProfileManager.Items.SystemConfig)){
                            RestartProgram();
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
                                RestartProgram(Arguments.ArgImportCookies);
                            }
                            else if (SelectedItems.HasFlag(ProfileManager.Items.SystemConfig)){
                                RestartProgram();
                            }
                        }
                        else{
                            ShouldReloadBrowser = true;
                        }
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
                    
                    new ProfileManager(file, plugins).Export(SelectedItems);

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
                btnContinue.Text = _selectedItems.NeedsRestart() ? "Import && Restart" : "Import Profile";
            }
            else if (currentState == State.Reset){
                btnContinue.Text = _selectedItems.NeedsRestart() ? "Restore && Restart" : "Restore Defaults";
            }
        }

        private void RestartProgram(params string[] extraArgs){
            IsRestarting = true;
            Program.Restart(extraArgs);
        }
    }
}
