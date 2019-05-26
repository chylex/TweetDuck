using System;
using System.Windows.Forms;
using TweetLib.Core.Features.Updates;

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
            if (updateInfo.DownloadStatus.IsFinished(false)){
                timerDownloadCheck.Stop();
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
