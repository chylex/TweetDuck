using System;
using System.Drawing;
using System.Windows.Forms;

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
            this.labelDescription.Text = plugin.Description;
            this.btnToggleState.Text = pluginManager.Config.IsEnabled(plugin) ? "Disable" : "Enable";
        }

        private void panelDescription_Resize(object sender, EventArgs e){
            labelDescription.MaximumSize = new Size(panelDescription.Width-SystemInformation.VerticalScrollBarWidth,0);
        }

        private void btnToggleState_Click(object sender, EventArgs e){
            bool newState = !pluginManager.Config.IsEnabled(plugin);
            pluginManager.Config.SetEnabled(plugin,newState);

            btnToggleState.Text = newState ? "Disable" : "Enable";
        }
    }
}
