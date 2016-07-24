using System;
using System.Drawing;
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
            this.btnToggleState.Text = pluginManager.Config.IsEnabled(plugin) ? "Disable" : "Enable";

            if (!plugin.CanRun){
                this.labelName.ForeColor = Color.DarkRed;
                this.btnToggleState.Enabled = false;
            }
            else if (labelDescription.Text == string.Empty){
                labelDescription.Visible = false;
            }

            panelDescription_Resize(panelDescription,new EventArgs());
        }

        private void panelDescription_Resize(object sender, EventArgs e){
            if (labelDescription.Text == string.Empty){
                Height = MinimumSize.Height;
            }
            else{
                labelDescription.MaximumSize = new Size(panelDescription.Width-SystemInformation.VerticalScrollBarWidth,0);
                Height = Math.Min(MinimumSize.Height+9+labelDescription.Height,MaximumSize.Height);
            }
        }

        private void labelWebsite_Click(object sender, EventArgs e){
            if (labelWebsite.Text.Length > 0){
                BrowserUtils.OpenExternalBrowser(labelWebsite.Text);
            }
        }

        private void btnToggleState_Click(object sender, EventArgs e){
            bool newState = !pluginManager.Config.IsEnabled(plugin);
            pluginManager.Config.SetEnabled(plugin,newState);

            btnToggleState.Text = newState ? "Disable" : "Enable";
        }
    }
}
