using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;

namespace TweetDck.Updates{
    sealed partial class FormUpdateDownload : Form{
        private const double BytesToKB = 1024.0;

        public string InstallerPath{
            get{
                return Path.Combine(Path.GetTempPath(), updateInfo.FileName);
            }
        }

        public enum Status{
            Waiting, Failed, Cancelled, Manual, Succeeded
        }

        public Status UpdateStatus { get; private set; }
        
        private readonly WebClient webClient;
        private readonly UpdateInfo updateInfo;

        public FormUpdateDownload(UpdateInfo info){
            InitializeComponent();

            this.webClient = new WebClient{ Proxy = null };
            this.webClient.Headers[HttpRequestHeader.UserAgent] = BrowserUtils.HeaderUserAgent;

            this.updateInfo = info;
            this.UpdateStatus = Status.Waiting;

            webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;

            Text = "Updating "+Program.BrandName;
            labelDescription.Text = "Downloading version "+info.VersionTag+"...";

            Disposed += (sender, args) => this.webClient.Dispose();
        }

        private void FormUpdateDownload_Shown(object sender, EventArgs e){
            webClient.DownloadFileAsync(new Uri(updateInfo.DownloadUrl), InstallerPath);
        }

        private void btnCancel_Click(object sender, EventArgs e){
            webClient.CancelAsync();
            btnCancel.Enabled = false;
        }

        private void FormUpdateDownload_FormClosing(object sender, FormClosingEventArgs e){
            if (UpdateStatus == Status.Waiting){
                e.Cancel = true;
                webClient.CancelAsync();
                UpdateStatus = e.CloseReason == CloseReason.UserClosing ? Status.Cancelled : Status.Manual; // manual will exit the app
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e){
            this.InvokeSafe(() => {
                if (e.TotalBytesToReceive == -1){
                    if (progressDownload.Style != ProgressBarStyle.Marquee){
                        progressDownload.Style = ProgressBarStyle.Continuous;
                        progressDownload.SetValueInstant(1000);
                    }

                    labelStatus.Text = (long)(e.BytesReceived/BytesToKB)+" kB";
                }
                else{
                    if (progressDownload.Style != ProgressBarStyle.Continuous){
                        progressDownload.Style = ProgressBarStyle.Continuous;
                    }

                    progressDownload.SetValueInstant(e.ProgressPercentage*10);
                    labelStatus.Text = (long)(e.BytesReceived/BytesToKB)+" / "+(long)(e.TotalBytesToReceive/BytesToKB)+" kB";
                }
            });
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e){
            this.InvokeSafe(() => {
                if (e.Cancelled){
                    if (UpdateStatus == Status.Waiting){
                        UpdateStatus = Status.Cancelled;
                    }
                }
                else if (e.Error != null){
                    Program.Reporter.Log(e.Error.ToString());

                    if (MessageBox.Show("Could not download the update: "+e.Error.Message+"\r\n\r\nDo you want to open the website and try downloading the update manually?", "Update Has Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.Yes){
                        BrowserUtils.OpenExternalBrowser(Program.Website);
                        UpdateStatus = Status.Manual;
                    }
                    else{
                        UpdateStatus = Status.Failed;
                    }
                }
                else{
                    UpdateStatus = Status.Succeeded;
                }

                Close();
            });
        }
    }
}
