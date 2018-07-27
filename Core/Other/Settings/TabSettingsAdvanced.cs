using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Management;
using TweetDuck.Core.Other.Settings.Dialogs;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsAdvanced : BaseTabSettings{
        private readonly Action<string> reinjectBrowserCSS;
        private readonly Action openDevTools;

        public TabSettingsAdvanced(Action<string> reinjectBrowserCSS, Action openDevTools){
            InitializeComponent();

            this.reinjectBrowserCSS = reinjectBrowserCSS;
            this.openDevTools = openDevTools;

            // application
            
            toolTip.SetToolTip(btnOpenAppFolder, "Opens the folder where the app is located.");
            toolTip.SetToolTip(btnOpenDataFolder, "Opens the folder where your profile data is located.");
            toolTip.SetToolTip(btnRestart, "Restarts the program using the same command\r\nline arguments that were used at launch.");
            toolTip.SetToolTip(btnRestartArgs, "Restarts the program with customizable\r\ncommand line arguments.");

            // browser cache

            toolTip.SetToolTip(btnClearCache, "Clearing cache will free up space taken by downloaded images and other resources.");
            toolTip.SetToolTip(checkClearCacheAuto, "Automatically clears cache when its size exceeds the set threshold. Note that cache can only be cleared when closing TweetDuck.");

            checkClearCacheAuto.Checked = SysConfig.ClearCacheAutomatically;
            numClearCacheThreshold.Enabled = checkClearCacheAuto.Checked;
            numClearCacheThreshold.SetValueSafe(SysConfig.ClearCacheThreshold);
            
            BrowserCache.GetCacheSize(task => {
                string text = task.Status == TaskStatus.RanToCompletion ? (int)Math.Ceiling(task.Result/(1024.0*1024.0))+" MB" : "unknown";
                this.InvokeSafe(() => btnClearCache.Text = $"Clear Cache ({text})");
            });

            // configuration

            toolTip.SetToolTip(btnEditCefArgs, "Set custom command line arguments for Chromium Embedded Framework.");
            toolTip.SetToolTip(btnEditCSS, "Set custom CSS for browser and notification windows.");
        }

        public override void OnReady(){
            btnOpenAppFolder.Click += btnOpenAppFolder_Click;
            btnOpenDataFolder.Click += btnOpenDataFolder_Click;
            btnRestart.Click += btnRestart_Click;
            btnRestartArgs.Click += btnRestartArgs_Click;

            btnClearCache.Click += btnClearCache_Click;
            checkClearCacheAuto.CheckedChanged += checkClearCacheAuto_CheckedChanged;
            
            btnEditCefArgs.Click += btnEditCefArgs_Click;
            btnEditCSS.Click += btnEditCSS_Click;
        }

        public override void OnClosing(){
            SysConfig.ClearCacheAutomatically = checkClearCacheAuto.Checked;
            SysConfig.ClearCacheThreshold = (int)numClearCacheThreshold.Value;
        }

        #region Application
        
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

        #endregion
        #region Browser Cache

        private void btnClearCache_Click(object sender, EventArgs e){
            btnClearCache.Enabled = false;
            BrowserCache.SetClearOnExit();
            FormMessage.Information("Clear Cache", "Cache will be automatically cleared when TweetDuck exits.", FormMessage.OK);
        }

        private void checkClearCacheAuto_CheckedChanged(object sender, EventArgs e){
            numClearCacheThreshold.Enabled = checkClearCacheAuto.Checked;
        }

        #endregion
        #region Configuration

        private void btnEditCefArgs_Click(object sender, EventArgs e){
            DialogSettingsCefArgs form = new DialogSettingsCefArgs();

            form.VisibleChanged += (sender2, args2) => {
                form.MoveToCenter(ParentForm);
            };

            form.FormClosed += (sender2, args2) => {
                RestoreParentForm();

                if (form.DialogResult == DialogResult.OK){
                    Config.CustomCefArgs = form.CefArgs;
                }

                form.Dispose();
            };
            
            form.Show(ParentForm);
            NativeMethods.SetFormDisabled(ParentForm, true);
        }

        private void btnEditCSS_Click(object sender, EventArgs e){
            DialogSettingsCSS form = new DialogSettingsCSS(reinjectBrowserCSS, openDevTools);

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

        private void RestoreParentForm(){
            if (ParentForm != null){ // when the parent is closed first, ParentForm is null in FormClosed event
                NativeMethods.SetFormDisabled(ParentForm, false);
            }
        }

        #endregion
    }
}
