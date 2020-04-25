using System;
using System.Windows.Forms;
using TweetDuck.Controls;
using TweetDuck.Utils;
using TweetLib.Core.Collections;

namespace TweetDuck.Dialogs.Settings{
    sealed partial class DialogSettingsCefArgs : Form{
        public string CefArgs => textBoxArgs.Text;

        private readonly string initialArgs;

        public DialogSettingsCefArgs(string args){
            InitializeComponent();
            
            Text = Program.BrandName + " Options - CEF Arguments";
            
            textBoxArgs.EnableMultilineShortcuts();
            textBoxArgs.Text = initialArgs = args ?? "";
            textBoxArgs.Select(textBoxArgs.Text.Length, 0);
        }

        private void btnHelp_Click(object sender, EventArgs e){
            BrowserUtils.OpenExternalBrowser("http://peter.sh/experiments/chromium-command-line-switches/");
        }

        private void btnApply_Click(object sender, EventArgs e){
            if (CefArgs == initialArgs){
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            int count = CommandLineArgs.ReadCefArguments(CefArgs).Count;
            string prompt = count == 0 && !string.IsNullOrWhiteSpace(initialArgs) ? "All current arguments will be removed. Continue?" : count + (count == 1 ? " argument was" : " arguments were") + " detected. Continue?";

            if (FormMessage.Question("Confirm CEF Arguments", prompt, FormMessage.OK, FormMessage.Cancel)){
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
