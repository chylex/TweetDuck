using System;
using System.Windows.Forms;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsCSS : Form{
        public string BrowserCSS{
            get{
                return textBoxBrowserCSS.Text;
            }
        }
        
        public string NotificationCSS{
            get{
                return textBoxNotificationCSS.Text;
            }
        }

        public DialogSettingsCSS(){
            InitializeComponent();
            
            Text = Program.BrandName+" Settings - CSS";
            
            textBoxBrowserCSS.EnableMultilineShortcuts();
            textBoxBrowserCSS.Text = Program.UserConfig.CustomBrowserCSS ?? "";

            textBoxNotificationCSS.EnableMultilineShortcuts();
            textBoxNotificationCSS.Text = Program.UserConfig.CustomNotificationCSS ?? "";
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
