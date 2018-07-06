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
        private static SystemConfig SysConfig => Program.SystemConfig;

        private readonly Action<string> reinjectBrowserCSS;

        public TabSettingsAdvanced(Action<string> reinjectBrowserCSS){
            InitializeComponent();

            this.reinjectBrowserCSS = reinjectBrowserCSS;
            
            toolTip.SetToolTip(btnOpenAppFolder, "Opens the folder where the app is located.");
            toolTip.SetToolTip(btnOpenDataFolder, "Opens the folder where your profile data is located.");
            toolTip.SetToolTip(btnRestart, "Restarts the program using the same command\r\nline arguments that were used at launch.");
            toolTip.SetToolTip(btnRestartArgs, "Restarts the program with customizable\r\ncommand line arguments.");

            toolTip.SetToolTip(checkHardwareAcceleration, "Uses graphics card to improve performance. Disable if you experience visual glitches, or to save a small amount of RAM.");

            toolTip.SetToolTip(btnClearCache, "Clearing cache will free up space taken by downloaded images and other resources.");
            toolTip.SetToolTip(checkClearCacheAuto, "Automatically clears cache when its size exceeds the set threshold. Note that cache can only be cleared when closing TweetDuck.");

            toolTip.SetToolTip(comboBoxThrottle, "Decides when to stop rendering and throttle the browser to improve performance and battery life.");

            toolTip.SetToolTip(btnEditCefArgs, "Set custom command line arguments for Chromium Embedded Framework.");
            toolTip.SetToolTip(btnEditCSS, "Set custom CSS for browser and notification windows.");

            checkHardwareAcceleration.Checked = SysConfig.HardwareAcceleration;

            checkClearCacheAuto.Checked = SysConfig.ClearCacheAutomatically;
            numClearCacheThreshold.Enabled = checkClearCacheAuto.Checked;
            numClearCacheThreshold.SetValueSafe(SysConfig.ClearCacheThreshold);
            
            BrowserCache.GetCacheSize(task => {
                string text = task.Status == TaskStatus.RanToCompletion ? (int)Math.Ceiling(task.Result/(1024.0*1024.0))+" MB" : "unknown";
                this.InvokeSafe(() => btnClearCache.Text = $"Clear Cache ({text})");
            });

            comboBoxThrottle.Items.Add("Minimized (Naive)");
            comboBoxThrottle.Items.Add("Covered (Smart)");
            comboBoxThrottle.Items.Add("Unfocused (Aggressive)");
            comboBoxThrottle.SelectedIndex = Math.Min(Math.Max((int)SysConfig.ThrottleBehavior, 0), comboBoxThrottle.Items.Count-1);
        }

        public override void OnReady(){
            btnOpenAppFolder.Click += btnOpenAppFolder_Click;
            btnOpenDataFolder.Click += btnOpenDataFolder_Click;
            btnRestart.Click += btnRestart_Click;
            btnRestartArgs.Click += btnRestartArgs_Click;

            checkHardwareAcceleration.CheckedChanged += checkHardwareAcceleration_CheckedChanged;

            btnClearCache.Click += btnClearCache_Click;
            checkClearCacheAuto.CheckedChanged += checkClearCacheAuto_CheckedChanged;

            comboBoxThrottle.SelectedIndexChanged += comboBoxThrottle_SelectedIndexChanged;
            
            btnEditCefArgs.Click += btnEditCefArgs_Click;
            btnEditCSS.Click += btnEditCSS_Click;
        }

        public override void OnClosing(){
            SysConfig.ClearCacheAutomatically = checkClearCacheAuto.Checked;
            SysConfig.ClearCacheThreshold = (int)numClearCacheThreshold.Value;
            SysConfig.Save();
        }

        private void btnClearCache_Click(object sender, EventArgs e){
            btnClearCache.Enabled = false;
            BrowserCache.SetClearOnExit();
            FormMessage.Information("Clear Cache", "Cache will be automatically cleared when TweetDuck exits.", FormMessage.OK);
        }

        private void checkClearCacheAuto_CheckedChanged(object sender, EventArgs e){
            numClearCacheThreshold.Enabled = checkClearCacheAuto.Checked;
        }

        private void checkHardwareAcceleration_CheckedChanged(object sender, EventArgs e){
            SysConfig.HardwareAcceleration = checkHardwareAcceleration.Checked;
            PromptRestart(); // calls OnClosing
        }

        private void comboBoxThrottle_SelectedIndexChanged(object sender, EventArgs e){
            SysConfig.ThrottleBehavior = (FormBrowser.ThrottleBehavior)comboBoxThrottle.SelectedIndex;
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
