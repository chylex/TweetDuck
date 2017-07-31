using System;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;
using TweetDuck.Data;

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

            int count = CommandLineArgs.ReadCefArguments(CefArgs).Count;
            string prompt = count == 0 && !string.IsNullOrWhiteSpace(prevArgs) ? "All current arguments will be removed. Continue?" : count+(count == 1 ? " argument was" : " arguments were")+" detected. Continue?";

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
