using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TweetDck.Core.Utils;

namespace TweetDck.Plugins.Controls{
    partial class PluginControl : UserControl{
        private readonly PluginManager pluginManager;
        private readonly Plugin plugin;

        public PluginControl(){
            InitializeComponent();
        }

        public PluginControl(PluginManager pluginManager, Plugin plugin) : this(){
            this.pluginManager = pluginManager;
            this.plugin = plugin;

            this.labelName.Text = plugin.Name;
            this.labelDescription.Text = plugin.CanRun ? plugin.Description : "This plugin requires "+Program.BrandName+" "+plugin.RequiredVersion+" or newer.";
            this.labelVersion.Text = plugin.Version;
            this.labelAuthor.Text = plugin.Author;
            this.labelWebsite.Text = plugin.Website;

            UpdatePluginState();

            if (labelDescription.Text.Length == 0){
                labelDescription.Visible = false;
            }

            panelDescription_Resize(panelDescription, new EventArgs());
        }

        private void panelDescription_Resize(object sender, EventArgs e){
            if (labelDescription.Text.Length == 0){
                Height = MinimumSize.Height;
            }
            else{
                labelDescription.MaximumSize = new Size(panelDescription.Width-SystemInformation.VerticalScrollBarWidth, 0);
                Height = Math.Min(MinimumSize.Height+9+labelDescription.Height, MaximumSize.Height);
            }
        }

        private void labelWebsite_Click(object sender, EventArgs e){
            if (labelWebsite.Text.Length > 0){
                BrowserUtils.OpenExternalBrowser(labelWebsite.Text);
            }
        }

        private void btnOpenConfig_Click(object sender, EventArgs e){
            using(Process.Start("explorer.exe", "/select,\""+plugin.ConfigPath+"\"")){}
        }

        private void btnToggleState_Click(object sender, EventArgs e){
            bool newState = !pluginManager.Config.IsEnabled(plugin);
            pluginManager.Config.SetEnabled(plugin, newState);

            UpdatePluginState();
        }

        private void UpdatePluginState(){
            bool isEnabled = plugin.CanRun && pluginManager.Config.IsEnabled(plugin);
            Color textColor = isEnabled ? Color.Black : Color.FromArgb(90, 90, 90);
            
            labelVersion.ForeColor = textColor;
            labelAuthor.ForeColor = textColor;
            labelWebsite.ForeColor = isEnabled ? Color.Blue : Color.FromArgb(90, 90, 249);

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
