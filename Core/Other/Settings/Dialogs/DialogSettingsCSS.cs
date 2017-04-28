using System;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsCSS : Form{
        public string BrowserCSS => textBoxBrowserCSS.Text;
        public string NotificationCSS => textBoxNotificationCSS.Text;

        private readonly Action<string> reinjectBrowserCSS;

        public DialogSettingsCSS(Action<string> reinjectBrowserCSS){
            InitializeComponent();
            
            Text = Program.BrandName+" Settings - CSS";

            this.reinjectBrowserCSS = reinjectBrowserCSS;
            
            textBoxBrowserCSS.EnableMultilineShortcuts();
            textBoxBrowserCSS.Text = Program.UserConfig.CustomBrowserCSS ?? "";

            textBoxNotificationCSS.EnableMultilineShortcuts();
            textBoxNotificationCSS.Text = Program.UserConfig.CustomNotificationCSS ?? "";
        }

        private void textBoxBrowserCSS_KeyUp(object sender, KeyEventArgs e){
            timerTestBrowser.Stop();
            timerTestBrowser.Start();
        }

        private void timerTestBrowser_Tick(object sender, EventArgs e){
            reinjectBrowserCSS(textBoxBrowserCSS.Text);
            timerTestBrowser.Stop();
        }

        private void btnOpenWiki_Click(object sender, EventArgs e){
            BrowserUtils.OpenExternalBrowserUnsafe("https://github.com/chylex/TweetDuck/wiki");
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
