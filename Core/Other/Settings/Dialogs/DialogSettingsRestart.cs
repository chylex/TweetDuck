using System;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Data;

namespace TweetDuck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsRestart : Form{
        public CommandLineArgs Args { get; private set; }

        public DialogSettingsRestart(CommandLineArgs currentArgs){
            InitializeComponent();

            cbLogging.Checked = currentArgs.HasFlag(Arguments.ArgLogging);
            cbLogging.CheckedChanged += control_Change;

            if (Program.IsPortable){
                tbDataFolder.Text = "Not available in portable version";
                tbDataFolder.Enabled = false;
            }
            else{
                tbDataFolder.Text = currentArgs.GetValue(Arguments.ArgDataFolder, string.Empty);
                tbDataFolder.TextChanged += control_Change;
            }

            control_Change(this, EventArgs.Empty);

            Text = Program.BrandName+" Arguments";
        }

        private void control_Change(object sender, EventArgs e){
            Args = new CommandLineArgs();
            
            if (cbLogging.Checked){
                Args.AddFlag(Arguments.ArgLogging);
            }

            if (!string.IsNullOrWhiteSpace(tbDataFolder.Text) && tbDataFolder.Enabled){
                Args.SetValue(Arguments.ArgDataFolder, tbDataFolder.Text);
            }

            tbShortcutTarget.Text = $@"""{Application.ExecutablePath}""{(Args.Count > 0 ? " " : "")}{Args}";
            tbShortcutTarget.Select(tbShortcutTarget.Text.Length, 0);
        }

        private void tbShortcutTarget_Click(object sender, EventArgs e){
            if (tbShortcutTarget.SelectionLength == 0){
                tbShortcutTarget.SelectAll();
            }
        }

        private void btnRestart_Click(object sender, EventArgs e){
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e){
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
