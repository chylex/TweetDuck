using System;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsCefArgs : Form{
        public string CefArgs => textBoxArgs.Text;

        public DialogSettingsCefArgs(){
            InitializeComponent();
            
            Text = Program.BrandName+" Options - CEF Arguments";
            
            textBoxArgs.EnableMultilineShortcuts();
            textBoxArgs.Text = Program.UserConfig.CustomCefArgs ?? "";
            textBoxArgs.Select(textBoxArgs.Text.Length, 0);
        }

        private void btnHelp_Click(object sender, EventArgs e){
            BrowserUtils.OpenExternalBrowserUnsafe("http://peter.sh/experiments/chromium-command-line-switches/");
        }

        private void btnApply_Click(object sender, EventArgs e){
            string prevArgs = Program.UserConfig.CustomCefArgs;

            if (CefArgs == prevArgs){
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            int count = CommandLineArgsParser.ReadCefArguments(CefArgs).Count;
            string prompt = count == 0 && !string.IsNullOrWhiteSpace(prevArgs) ? "All arguments will be removed from the settings. Continue?" : count+(count == 1 ? " argument" : " arguments")+" will be added to the settings. Continue?";

            if (MessageBox.Show(prompt, "Confirm CEF Arguments", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK){
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e){
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
