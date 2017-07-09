using System;
using System.Windows.Forms;
using TweetDuck.Core.Other.Settings.Export;
using TweetDuck.Plugins;

namespace TweetDuck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsManage : Form{
        private enum State{
            Deciding, Import, Export
        }

        public ExportFileFlags Flags{
            get => selectedFlags;

            set{
                // this will call events and SetFlag, which also updates the UI
                cbConfig.Checked = value.HasFlag(ExportFileFlags.Config);
                cbSession.Checked = value.HasFlag(ExportFileFlags.Session);
                cbPluginData.Checked = value.HasFlag(ExportFileFlags.PluginData);
            }
        }

        public bool ShouldReloadUI { get; private set; }
        
        private readonly PluginManager plugins;
        private State currentState;

        private ExportManager importManager;
        private ExportFileFlags selectedFlags = ExportFileFlags.None;

        public DialogSettingsManage(PluginManager plugins){
            InitializeComponent();

            this.plugins = plugins;
            this.currentState = State.Deciding;
        }

        private void radioDecision_CheckedChanged(object sender, EventArgs e){
            btnContinue.Enabled = true;
        }

        private void cbConfig_CheckedChanged(object sender, EventArgs e){
            SetFlag(ExportFileFlags.Config, cbConfig.Checked);
        }

        private void cbSession_CheckedChanged(object sender, EventArgs e){
            SetFlag(ExportFileFlags.Session, cbSession.Checked);
        }

        private void cbPluginData_CheckedChanged(object sender, EventArgs e){
            SetFlag(ExportFileFlags.PluginData, cbPluginData.Checked);
        }

        private void btnContinue_Click(object sender, EventArgs e){
            string file;

            switch(currentState){
                case State.Deciding:
                    // Reset
                    if (radioReset.Checked){
                        if (FormMessage.Warning("Reset "+Program.BrandName+" Options", "This will reset all of your program options. Plugins will not be affected. Do you want to proceed?", FormMessage.Yes, FormMessage.No)){
                            Program.ResetConfig();

                            ShouldReloadUI = true;
                            DialogResult = DialogResult.OK;
                            Close();
                        }

                        return;
                    }

                    // Import
                    else if (radioImport.Checked){
                        using(OpenFileDialog dialog = new OpenFileDialog{
                            AutoUpgradeEnabled = true,
                            DereferenceLinks = true,
                            Title = "Import "+Program.BrandName+" Profile",
                            Filter = Program.BrandName+" Profile (*.tdsettings)|*.tdsettings"
                        }){
                            if (dialog.ShowDialog() != DialogResult.OK){
                                return;
                            }

                            file = dialog.FileName;
                        }

                        importManager = new ExportManager(file, plugins);
                        currentState = State.Import;

                        Text = "Import Profile";
                        Flags = importManager.GetImportFlags();

                        cbConfig.Enabled = cbConfig.Checked;
                        cbSession.Enabled = cbSession.Checked;
                        cbPluginData.Enabled = cbPluginData.Checked;
                    }

                    // Export
                    else if (radioExport.Checked){
                        currentState = State.Export;

                        Text = "Export Profile";
                        btnContinue.Text = "Export Profile";
                        Flags = ExportFileFlags.All & ~ExportFileFlags.Session;
                    }
                    
                    // Continue...
                    panelDecision.Visible = false;
                    panelExport.Visible = true;
                    break;

                case State.Import:
                    if (importManager.Import(Flags)){
                        if (!importManager.IsRestarting){
                            ShouldReloadUI = true;
                        }
                    }
                    else{
                        Program.Reporter.HandleException("Profile Import Error", "An exception happened while importing "+Program.BrandName+" profile.", true, importManager.LastException);
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
                        FileName = Program.BrandName+".tdsettings",
                        Title = "Export "+Program.BrandName+" Profile",
                        Filter = Program.BrandName+" Profile (*.tdsettings)|*.tdsettings"
                    }){
                        if (dialog.ShowDialog() != DialogResult.OK){
                            return;
                        }

                        file = dialog.FileName;
                    }

                    Program.UserConfig.Save();
                    ExportManager manager = new ExportManager(file, plugins);

                    if (!manager.Export(Flags)){
                        Program.Reporter.HandleException("Profile Export Error", "An exception happened while exporting "+Program.BrandName+" profile.", true, manager.LastException);
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

        private void SetFlag(ExportFileFlags flag, bool enable){
            selectedFlags = enable ? selectedFlags | flag : selectedFlags & ~flag;
            btnContinue.Enabled = selectedFlags != ExportFileFlags.None;

            if (currentState == State.Import){
                btnContinue.Text = selectedFlags.HasFlag(ExportFileFlags.Session) ? "Import && Restart" : "Import Profile";
            }
        }
    }
}
