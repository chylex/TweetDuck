using System;
using System.Windows.Forms;
using TweetDuck.Core.Other;
using TweetDuck.Core.Utils;

namespace TweetDuck.Updates{
    sealed partial class FormUpdateDownload : Form{
        private readonly UpdateInfo updateInfo;

        public FormUpdateDownload(UpdateInfo info){
            InitializeComponent();

            this.updateInfo = info;

            Text = "Updating "+Program.BrandName;
            labelDescription.Text = "Downloading version "+info.VersionTag+"...";
            timerDownloadCheck.Start();
        }

        private void btnCancel_Click(object sender, EventArgs e){
            Close();
        }

        private void timerDownloadCheck_Tick(object sender, EventArgs e){
            if (updateInfo.DownloadStatus == UpdateDownloadStatus.Done){
                timerDownloadCheck.Stop();
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (updateInfo.DownloadStatus == UpdateDownloadStatus.Failed){
                timerDownloadCheck.Stop();

                if (FormMessage.Error("Update Has Failed", "Could not download the update: "+(updateInfo.DownloadError?.Message ?? "unknown error")+"\n\nDo you want to open the website and try downloading the update manually?", FormMessage.Yes, FormMessage.No)){
                    BrowserUtils.OpenExternalBrowser(Program.Website);
                    DialogResult = DialogResult.OK;
                }
                
                Close();
            }
        }
    }
}
