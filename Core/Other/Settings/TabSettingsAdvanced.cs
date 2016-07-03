using System;
using System.Diagnostics;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Other.Settings{
    partial class TabSettingsAdvanced : BaseTabSettings{
        public TabSettingsAdvanced(){
            InitializeComponent();

            checkHardwareAcceleration.Checked = HardwareAcceleration.IsEnabled;

            BrowserCache.CalculateCacheSize(bytes => this.InvokeSafe(() => {
                if (bytes == -1L){
                    btnClearCache.Text = "Clear Cache (unknown size)";
                }
                else{
                    btnClearCache.Text = "Clear Cache ("+(int)Math.Ceiling(bytes/(1024.0*1024.0))+" MB)";
                }
            }));
        }

        private void btnClearCache_Click(object sender, EventArgs e){
            if (!Ready)return;

            btnClearCache.Enabled = false;
            BrowserCache.SetClearOnExit();

            MessageBox.Show("Cache will be automatically cleared when "+Program.BrandName+" exits.","Clear Cache",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void checkHardwareAcceleration_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            bool succeeded = false;

            if (checkHardwareAcceleration.Checked){
                if (HardwareAcceleration.CanEnable){
                    succeeded = HardwareAcceleration.Enable();
                }
                else{
                    MessageBox.Show("Cannot enable hardware acceleration, the libraries libEGL.dll and libGLESv2.dll could not be restored.",Program.BrandName+" Settings",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else{
                succeeded = HardwareAcceleration.Disable();
            }

            if (succeeded && MessageBox.Show("The application must restart for the setting to take place. Do you want to restart now?",Program.BrandName+" Settings",MessageBoxButtons.YesNo,MessageBoxIcon.Information) == DialogResult.Yes){ // TODO
                Process.Start(Application.ExecutablePath,"-restart");
                Application.Exit();
            }
            else if (!succeeded){
                checkHardwareAcceleration.CheckedChanged -= checkHardwareAcceleration_CheckedChanged;
                checkHardwareAcceleration.Checked = HardwareAcceleration.IsEnabled;
                checkHardwareAcceleration.CheckedChanged += checkHardwareAcceleration_CheckedChanged;
            }
        }
    }
}
