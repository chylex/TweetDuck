using System;
using System.Windows.Forms;
using TweetDck.Core.Other.Settings.Export;

namespace TweetDck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsExport : Form{
        public static DialogSettingsExport Import(ExportFileFlags flags){
            return new DialogSettingsExport(flags);
        }

        public static DialogSettingsExport Export(){
            return new DialogSettingsExport(ExportFileFlags.None);
        }

        public ExportFileFlags Flags{
            get{
                return selectedFlags;
            }

            set{
                selectedFlags = value;
                btnApply.Enabled = selectedFlags != ExportFileFlags.None;

                cbConfig.Checked = selectedFlags.HasFlag(ExportFileFlags.Config);
                cbSession.Checked = selectedFlags.HasFlag(ExportFileFlags.Session);
                cbPluginData.Checked = selectedFlags.HasFlag(ExportFileFlags.PluginData);
            }
        }
        
        private ExportFileFlags selectedFlags = ExportFileFlags.None;

        private DialogSettingsExport(ExportFileFlags importFlags){
            InitializeComponent();

            bool isExporting = importFlags == ExportFileFlags.None;

            if (isExporting){
                Text = "Export Profile";
                btnApply.Text = "Export";
                Flags = ExportFileFlags.All & ~ExportFileFlags.Session;
            }
            else{
                Text = "Import Profile";
                btnApply.Text = "Import";
                Flags = importFlags;

                cbConfig.Enabled = cbConfig.Checked;
                cbSession.Enabled = cbSession.Checked;
                cbPluginData.Enabled = cbPluginData.Checked;
            }
        }

        private void SetFlag(ExportFileFlags flag, bool enable){
            selectedFlags = enable ? selectedFlags | flag : selectedFlags & ~flag;
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

        private void btnApply_Click(object sender, EventArgs e){
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e){
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
