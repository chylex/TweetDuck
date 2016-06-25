using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TweetDck.Plugins.Events;

namespace TweetDck.Plugins.Controls{
    sealed partial class PluginListControl : UserControl{
        private PluginManager pluginManager;
        private PluginGroup? selectedGroup;

        private IEnumerable<Button> tabButtons{
            get{
                yield return btnTabOfficial;
                yield return btnTabCustom;
            }
        }

        public PluginListControl(){
            InitializeComponent();
        }

        public void Initialize(PluginManager manager){
            if (this.pluginManager != null){
                throw new InvalidOperationException("PluginListControl was already initialized!");
            }

            this.pluginManager = manager;
            this.pluginManager.Reloaded += pluginManager_Reloaded;
            this.pluginManager_Reloaded(manager,null);

            SelectTab(btnTabOfficial,PluginGroup.Official);
        }

        private void pluginManager_Reloaded(object sender, PluginLoadEventArgs e){
            btnTabOfficial.Text = "Official: "+pluginManager.CountPluginByGroup(PluginGroup.Official);
            btnTabCustom.Text = "Custom: "+pluginManager.CountPluginByGroup(PluginGroup.Custom);
        }

        public void ReloadPluginTab(){
            if (!selectedGroup.HasValue)return;

            flowLayoutPlugins.Controls.Clear();

            foreach(Plugin plugin in pluginManager.GetPluginsByGroup(selectedGroup.Value)){
                flowLayoutPlugins.Controls.Add(new PluginControl(pluginManager,plugin));
            }

            flowLayoutPlugins_Resize(flowLayoutPlugins,new EventArgs());
        }

        private void btnTabOfficial_Click(object sender, EventArgs e){
            SelectTab(btnTabOfficial,PluginGroup.Official);
        }

        private void btnTabCustom_Click(object sender, EventArgs e){
            SelectTab(btnTabCustom,PluginGroup.Custom);
        }

        private void SelectTab(Button button, PluginGroup group){
            if (selectedGroup.HasValue && selectedGroup == group)return;

            selectedGroup = group;

            foreach(Button btn in tabButtons){
                btn.BackColor = SystemColors.Control;
            }

            button.BackColor = Color.White;
            
            ReloadPluginTab();
        }

        private void flowLayoutPlugins_Resize(object sender, EventArgs e){
            int horizontalOffset = 8+(flowLayoutPlugins.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);

            foreach(Control control in flowLayoutPlugins.Controls){
                control.Width = flowLayoutPlugins.Width-control.Margin.Horizontal-horizontalOffset;
            }
        }
    }
}
