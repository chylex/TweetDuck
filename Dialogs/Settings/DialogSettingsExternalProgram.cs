using System;
using System.Windows.Forms;
using TweetLib.Core.Utils;
using IOPath = System.IO.Path;

namespace TweetDuck.Dialogs.Settings{
    sealed partial class DialogSettingsExternalProgram : Form{
        public string Path{
            get => StringUtils.NullIfEmpty(textBoxPath.Text);
            set => textBoxPath.Text = value ?? string.Empty;
        }

        public string Args{
            get => StringUtils.NullIfEmpty(textBoxArgs.Text);
            set => textBoxArgs.Text = value ?? string.Empty;
        }

        private readonly string fileDialogTitle;

        public DialogSettingsExternalProgram(string windowTitle, string fileDialogTitle){
            InitializeComponent();
            
            Text = Program.BrandName + " Options - " + windowTitle;
            AcceptButton = btnApply;
            CancelButton = btnCancel;

            this.fileDialogTitle = fileDialogTitle;
        }

        private void btnBrowse_Click(object sender, EventArgs e){
            using OpenFileDialog dialog = new OpenFileDialog{
                AutoUpgradeEnabled = true,
                DereferenceLinks = true,
                InitialDirectory = IOPath.GetDirectoryName(Path), // returns null if argument is null
                Title = fileDialogTitle,
                Filter = "Executables (*.exe;*.bat;*.cmd)|*.exe;*.bat;*.cmd|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK && Path != dialog.FileName){
                Path = dialog.FileName;
                Args = string.Empty;
            }
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
