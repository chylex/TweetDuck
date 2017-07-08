using System;
using System.Diagnostics;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other.Settings.Dialogs;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Settings{
    partial class TabSettingsAdvanced : BaseTabSettings{
        private readonly Action<string> reinjectBrowserCSS;

        public TabSettingsAdvanced(Action<string> reinjectBrowserCSS){
            InitializeComponent();

            this.reinjectBrowserCSS = reinjectBrowserCSS;

            if (SystemConfig.IsHardwareAccelerationSupported){
                checkHardwareAcceleration.Checked = Program.SystemConfig.HardwareAcceleration;
            }
            else{
                checkHardwareAcceleration.Enabled = false;
                checkHardwareAcceleration.Checked = false;
            }

            checkBrowserGCReload.Checked = Config.EnableBrowserGCReload;
            numMemoryThreshold.Enabled = checkBrowserGCReload.Checked;
            numMemoryThreshold.SetValueSafe(Config.BrowserMemoryThreshold);

            BrowserCache.CalculateCacheSize(bytes => this.InvokeSafe(() => {
                if (bytes == -1L){
                    btnClearCache.Text = "Clear Cache (unknown size)";
                }
                else{
                    btnClearCache.Text = "Clear Cache ("+(int)Math.Ceiling(bytes/(1024.0*1024.0))+" MB)";
                }
            }));
        }

        public override void OnReady(){
            btnClearCache.Click += btnClearCache_Click;
            checkHardwareAcceleration.CheckedChanged += checkHardwareAcceleration_CheckedChanged;

            checkBrowserGCReload.CheckedChanged += checkBrowserGCReload_CheckedChanged;
            numMemoryThreshold.ValueChanged += numMemoryThreshold_ValueChanged;

            btnEditCefArgs.Click += btnEditCefArgs_Click;
            btnEditCSS.Click += btnEditCSS_Click;
            
            btnOpenAppFolder.Click += btnOpenAppFolder_Click;
            btnOpenDataFolder.Click += btnOpenDataFolder_Click;
            btnRestart.Click += btnRestart_Click;
            btnRestartArgs.Click += btnRestartArgs_Click;
        }

        private void btnClearCache_Click(object sender, EventArgs e){
            btnClearCache.Enabled = false;
            BrowserCache.SetClearOnExit();

            MessageBox.Show("Cache will be automatically cleared when "+Program.BrandName+" exits.", "Clear Cache", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkHardwareAcceleration_CheckedChanged(object sender, EventArgs e){
            Program.SystemConfig.HardwareAcceleration = checkHardwareAcceleration.Checked;
            Program.SystemConfig.Save();
            PromptRestart();
        }

        private void checkBrowserGCReload_CheckedChanged(object sender, EventArgs e){
            Config.EnableBrowserGCReload = checkBrowserGCReload.Checked;
            numMemoryThreshold.Enabled = checkBrowserGCReload.Checked;
        }

        private void numMemoryThreshold_ValueChanged(object sender, EventArgs e){
            Config.BrowserMemoryThreshold = (int)numMemoryThreshold.Value;
        }

        private void btnEditCefArgs_Click(object sender, EventArgs e){
            DialogSettingsCefArgs form = new DialogSettingsCefArgs();

            form.VisibleChanged += (sender2, args2) => {
                form.MoveToCenter(ParentForm);
            };

            form.FormClosed += (sender2, args2) => {
                NativeMethods.SetFormDisabled(ParentForm, false);

                if (form.DialogResult == DialogResult.OK){
                    Config.CustomCefArgs = form.CefArgs;
                    PromptRestart();
                    form.Dispose();
                }
                else form.Dispose();
            };
            
            form.Show(ParentForm);
            NativeMethods.SetFormDisabled(ParentForm, true);
        }

        private void btnEditCSS_Click(object sender, EventArgs e){
            DialogSettingsCSS form = new DialogSettingsCSS(reinjectBrowserCSS);

            form.VisibleChanged += (sender2, args2) => {
                form.MoveToCenter(ParentForm);
            };

            form.FormClosed += (sender2, args2) => {
                NativeMethods.SetFormDisabled(ParentForm, false);

                if (form.DialogResult == DialogResult.OK){
                    Config.CustomBrowserCSS = form.BrowserCSS;
                    Config.CustomNotificationCSS = form.NotificationCSS;
                }

                reinjectBrowserCSS(Config.CustomBrowserCSS); // reinject on cancel too, because the CSS is updated while typing
                form.Dispose();
            };
            
            form.Show(ParentForm);
            NativeMethods.SetFormDisabled(ParentForm, true);
        }

        private void btnOpenAppFolder_Click(object sender, EventArgs e){
            using(Process.Start("explorer.exe", "\""+Program.ProgramPath+"\"")){}
        }

        private void btnOpenDataFolder_Click(object sender, EventArgs e){
            using(Process.Start("explorer.exe", "\""+Program.StoragePath+"\"")){}
        }

        private void btnRestart_Click(object sender, EventArgs e){
            Program.Restart();
        }

        private void btnRestartArgs_Click(object sender, EventArgs e){
            using(DialogSettingsRestart dialog = new DialogSettingsRestart(Arguments.GetCurrentClean())){
                if (dialog.ShowDialog() == DialogResult.OK){
                    Program.RestartWithArgs(dialog.Args);
                }
            }
        }
    }
}
