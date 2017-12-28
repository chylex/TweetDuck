using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other.Settings.Dialogs;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsAdvanced : BaseTabSettings{
        private static SystemConfig SysConfig => Program.SystemConfig;

        private readonly Action<string> reinjectBrowserCSS;

        public TabSettingsAdvanced(Action<string> reinjectBrowserCSS){
            InitializeComponent();

            this.reinjectBrowserCSS = reinjectBrowserCSS;

            if (SystemConfig.IsHardwareAccelerationSupported){
                checkHardwareAcceleration.Checked = SysConfig.HardwareAcceleration;
            }
            else{
                checkHardwareAcceleration.Enabled = false;
                checkHardwareAcceleration.Checked = false;
            }
            
            BrowserCache.CalculateCacheSize(task => {
                string text = task.Status == TaskStatus.RanToCompletion ? (int)Math.Ceiling(task.Result/(1024.0*1024.0))+" MB" : "unknown";
                this.InvokeSafe(() => btnClearCache.Text = $"Clear Cache ({text})");
            });
        }

        public override void OnReady(){
            btnClearCache.Click += btnClearCache_Click;
            checkHardwareAcceleration.CheckedChanged += checkHardwareAcceleration_CheckedChanged;
            
            btnEditCefArgs.Click += btnEditCefArgs_Click;
            btnEditCSS.Click += btnEditCSS_Click;
            
            btnOpenAppFolder.Click += btnOpenAppFolder_Click;
            btnOpenDataFolder.Click += btnOpenDataFolder_Click;
            btnRestart.Click += btnRestart_Click;
            btnRestartArgs.Click += btnRestartArgs_Click;
        }

        public override void OnClosing(){
            SysConfig.Save();
        }

        private void btnClearCache_Click(object sender, EventArgs e){
            btnClearCache.Enabled = false;
            BrowserCache.SetClearOnExit();
            FormMessage.Information("Clear Cache", "Cache will be automatically cleared when TweetDuck exits.", FormMessage.OK);
        }

        private void checkHardwareAcceleration_CheckedChanged(object sender, EventArgs e){
            SysConfig.HardwareAcceleration = checkHardwareAcceleration.Checked;
            PromptRestart(); // calls OnClosing
        }

        private void btnEditCefArgs_Click(object sender, EventArgs e){
            DialogSettingsCefArgs form = new DialogSettingsCefArgs();

            form.VisibleChanged += (sender2, args2) => {
                form.MoveToCenter(ParentForm);
            };

            form.FormClosed += (sender2, args2) => {
                RestoreParentForm();

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
                RestoreParentForm();

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

        private void RestoreParentForm(){
            if (ParentForm != null){ // when the parent is closed first, ParentForm is null in FormClosed event
                NativeMethods.SetFormDisabled(ParentForm, false);
            }
        }
    }
}
