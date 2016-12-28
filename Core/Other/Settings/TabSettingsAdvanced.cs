﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Core.Other.Settings.Dialogs;
using TweetDck.Core.Other.Settings.Export;
using TweetDck.Core.Utils;
using TweetDck.Plugins;

namespace TweetDck.Core.Other.Settings{
    partial class TabSettingsAdvanced : BaseTabSettings{
        private readonly Action browserReloadAction;
        private readonly PluginManager plugins;

        public TabSettingsAdvanced(Action browserReloadAction, PluginManager plugins){
            InitializeComponent();

            this.browserReloadAction = browserReloadAction;
            this.plugins = plugins;

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

            MessageBox.Show("Cache will be automatically cleared when "+Program.BrandName+" exits.", "Clear Cache", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkHardwareAcceleration_CheckedChanged(object sender, EventArgs e){
            if (!Ready)return;

            bool succeeded = false;

            if (checkHardwareAcceleration.Checked){
                if (HardwareAcceleration.CanEnable){
                    succeeded = HardwareAcceleration.Enable();
                }
                else{
                    MessageBox.Show("Cannot enable hardware acceleration, the libraries libEGL.dll and libGLESv2.dll could not be restored.", Program.BrandName+" Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else{
                succeeded = HardwareAcceleration.Disable();
            }

            if (succeeded){
                PromptRestart();
            }
            else{
                checkHardwareAcceleration.CheckedChanged -= checkHardwareAcceleration_CheckedChanged;
                checkHardwareAcceleration.Checked = HardwareAcceleration.IsEnabled;
                checkHardwareAcceleration.CheckedChanged += checkHardwareAcceleration_CheckedChanged;
            }
        }

        private void btnEditCefArgs_Click(object sender, EventArgs e){
            DialogSettingsCefArgs form = new DialogSettingsCefArgs();

            if (form.ShowDialog(ParentForm) == DialogResult.OK){
                Config.CustomCefArgs = form.CefArgs;
                form.Dispose();
                PromptRestart();
            }
            else{
                form.Dispose();
            }
        }

        private void btnEditCSS_Click(object sender, EventArgs e){
            using(DialogSettingsCSS form = new DialogSettingsCSS()){
                if (form.ShowDialog(ParentForm) == DialogResult.OK){
                    bool hasChangedBrowser = form.BrowserCSS != Config.CustomBrowserCSS;

                    Config.CustomBrowserCSS = form.BrowserCSS;
                    Config.CustomNotificationCSS = form.NotificationCSS;

                    if (hasChangedBrowser && MessageBox.Show("The browser CSS has changed, do you want to reload it?", "Browser CSS Changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                        browserReloadAction();
                    }
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e){
            ExportFileFlags flags;

            using(DialogSettingsExport dialog = DialogSettingsExport.Export()){
                if (dialog.ShowDialog() != DialogResult.OK){
                    return;
                }

                flags = dialog.Flags;
            }

            string file;

            using(SaveFileDialog dialog = new SaveFileDialog{
                AddExtension = true,
                AutoUpgradeEnabled = true,
                OverwritePrompt = true,
                DefaultExt = "tdsettings",
                FileName = Program.BrandName+".tdsettings",
                Title = "Export "+Program.BrandName+" Settings",
                Filter = Program.BrandName+" Settings (*.tdsettings)|*.tdsettings"
            }){
                if (dialog.ShowDialog() != DialogResult.OK){
                    return;
                }

                file = dialog.FileName;
            }

            Program.UserConfig.Save();

            ExportManager manager = new ExportManager(file, plugins);
            
            if (!manager.Export(flags)){
                Program.Reporter.HandleException("Profile Export Error", "An exception happened while exporting "+Program.BrandName+" settings.", true, manager.LastException);
            }
        }

        private void btnImport_Click(object sender, EventArgs e){
            string file;

            using(OpenFileDialog dialog = new OpenFileDialog{
                AutoUpgradeEnabled = true,
                DereferenceLinks = true,
                Title = "Import "+Program.BrandName+" Settings",
                Filter = Program.BrandName+" Settings (*.tdsettings)|*.tdsettings"
            }){
                if (dialog.ShowDialog() != DialogResult.OK){
                    return;
                }

                file = dialog.FileName;
            }

            ExportManager manager = new ExportManager(file, plugins);
            ExportFileFlags flags;

            using(DialogSettingsExport dialog = DialogSettingsExport.Import(manager.GetImportFlags())){
                if (dialog.ShowDialog() != DialogResult.OK){
                    return;
                }

                flags = dialog.Flags;
            }

            if (manager.Import(flags)){
                if (!manager.IsRestarting){
                    ((FormSettings)ParentForm).ReloadUI();
                }
            }
            else{
                Program.Reporter.HandleException("Profile Import Error", "An exception happened while importing "+Program.BrandName+" settings.", true, manager.LastException);
            }
        }

        private void btnReset_Click(object sender, EventArgs e){
            if (MessageBox.Show("This will reset all of your settings, including disabled plugins. Do you want to proceed?", "Reset "+Program.BrandName+" Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                Program.ResetConfig();
                ((FormSettings)ParentForm).ReloadUI();
            }
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

        private void btnRestartLog_Click(object sender, EventArgs e){
            Program.Restart(new string[]{ "-log" });
        }
    }
}
