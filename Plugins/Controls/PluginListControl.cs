using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
            SelectTab(btnTabOfficial,PluginGroup.Official);
        }

        public void ReloadPlugins(){
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
            
            ReloadPlugins();
        }

        private void flowLayoutPlugins_Resize(object sender, EventArgs e){
            int horizontalOffset = 8+(flowLayoutPlugins.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);

            foreach(Control control in flowLayoutPlugins.Controls){
                control.Width = flowLayoutPlugins.Width-control.Margin.Horizontal-horizontalOffset;
            }
        }
    }
}
