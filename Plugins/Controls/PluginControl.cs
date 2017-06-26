using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins.Enums;

namespace TweetDuck.Plugins.Controls{
    partial class PluginControl : UserControl{
        private readonly PluginManager pluginManager;
        private readonly Plugin plugin;

        private readonly float dpiScale;

        public PluginControl(){
            InitializeComponent();
        }

        public PluginControl(PluginManager pluginManager, Plugin plugin) : this(){
            this.pluginManager = pluginManager;
            this.plugin = plugin;

            this.dpiScale = this.GetDPIScale();

            this.labelName.Text = plugin.Name;
            this.labelDescription.Text = plugin.CanRun ? plugin.Description : "This plugin requires "+Program.BrandName+" "+plugin.RequiredVersion+" or newer.";
            this.labelVersion.Text = plugin.Version;
            this.labelAuthor.Text = plugin.Author;
            this.labelWebsite.Text = plugin.Website;
            
            this.labelType.LineHeight = BrowserUtils.Scale(9, dpiScale);

            UpdatePluginState();

            if (labelDescription.Text.Length == 0){
                labelDescription.Visible = false;
            }

            panelDescription_Resize(panelDescription, null);
        }

        private void panelDescription_Resize(object sender, EventArgs e){
            if (labelDescription.Text.Length == 0){
                Height = MinimumSize.Height;
            }
            else{
                labelDescription.MaximumSize = new Size(panelDescription.Width-SystemInformation.VerticalScrollBarWidth, 0);
                Height = Math.Min(MinimumSize.Height+BrowserUtils.Scale(9, dpiScale)+labelDescription.Height, MaximumSize.Height);
            }
        }

        private void labelWebsite_Click(object sender, EventArgs e){
            if (labelWebsite.Text.Length > 0){
                BrowserUtils.OpenExternalBrowser(labelWebsite.Text);
            }
        }

        private void btnOpenConfig_Click(object sender, EventArgs e){
            using(Process.Start("explorer.exe", "/select,\""+plugin.ConfigPath.Replace('/', '\\')+"\"")){}
        }

        private void btnToggleState_Click(object sender, EventArgs e){
            bool newState = !pluginManager.Config.IsEnabled(plugin);
            pluginManager.Config.SetEnabled(plugin, newState);

            UpdatePluginState();
        }

        private void UpdatePluginState(){
            bool isEnabled = pluginManager.Config.IsEnabled(plugin) && plugin.CanRun;
            Color textColor = isEnabled ? Color.Black : Color.FromArgb(90, 90, 90);
            
            labelVersion.ForeColor = textColor;
            labelAuthor.ForeColor = textColor;
            labelWebsite.ForeColor = isEnabled ? Color.Blue : Color.FromArgb(90, 90, 249);

            if (plugin.Group == PluginGroup.Official){
                labelType.Text = "OFFICIAL";
                labelType.BackColor = isEnabled ? Color.FromArgb(154, 195, 217) : Color.FromArgb(185, 185, 185);
            }
            else{
                labelType.Text = "CUSTOM";
                labelType.BackColor = isEnabled ? Color.FromArgb(208, 154, 217) : Color.FromArgb(185, 185, 185);
            }

            if (plugin.CanRun){
                labelName.ForeColor = textColor;
                labelDescription.ForeColor = textColor;
                btnToggleState.Text = isEnabled ? "Disable" : "Enable";
                btnOpenConfig.Visible = plugin.HasConfig;
                btnOpenConfig.Enabled = btnOpenConfig.Visible && File.Exists(plugin.ConfigPath);
            }
            else{
                labelName.ForeColor = Color.DarkRed;
                labelDescription.ForeColor = Color.DarkRed;
                btnToggleState.Visible = false;
                btnOpenConfig.Visible = false;
            }
        }
    }
}
