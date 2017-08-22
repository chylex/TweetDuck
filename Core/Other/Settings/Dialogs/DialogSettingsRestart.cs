using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Data;

namespace TweetDuck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsRestart : Form{
        private const string DefaultLocale = "en-US";

        public CommandLineArgs Args { get; private set; }

        public DialogSettingsRestart(CommandLineArgs currentArgs){
            InitializeComponent();

            try{
                object[] locales = Directory.EnumerateFiles(Path.Combine(Program.ProgramPath, "locales"), "*.pak", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension).ToArray<object>();
                comboLocale.Items.AddRange(locales);
            }catch{
                comboLocale.Items.Add(DefaultLocale);
            }

            cbLogging.Checked = currentArgs.HasFlag(Arguments.ArgLogging);
            cbDebugUpdates.Checked = currentArgs.HasFlag(Arguments.ArgDebugUpdates);
            comboLocale.SelectedItem = currentArgs.GetValue(Arguments.ArgLocale, DefaultLocale);

            if (Program.IsPortable){
                tbDataFolder.Text = "Not available in portable version";
                tbDataFolder.Enabled = false;
            }
            else{
                tbDataFolder.Text = currentArgs.GetValue(Arguments.ArgDataFolder, string.Empty);
            }

            Text = Program.BrandName+" Arguments";
        }

        private void btnRestart_Click(object sender, EventArgs e){
            Args = new CommandLineArgs();
            
            if (cbLogging.Checked){
                Args.AddFlag(Arguments.ArgLogging);
            }
            
            if (cbDebugUpdates.Checked){
                Args.AddFlag(Arguments.ArgDebugUpdates);
            }

            string locale = comboLocale.SelectedItem as string;

            if (!string.IsNullOrWhiteSpace(locale) && locale != DefaultLocale){
                Args.SetValue(Arguments.ArgLocale, locale);
            }

            if (!string.IsNullOrWhiteSpace(tbDataFolder.Text)){
                Args.SetValue(Arguments.ArgDataFolder, tbDataFolder.Text);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e){
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
