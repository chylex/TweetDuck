using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Plugins;
using TweetDck.Plugins.Controls;
using TweetDck.Plugins.Events;

namespace TweetDck.Core.Other{
    sealed partial class FormPlugins : Form{
        private readonly PluginManager pluginManager;
        private readonly TabButton tabBtnOfficial, tabBtnCustom;
        private readonly PluginListFlowLayout flowLayoutPlugins;

        private PluginGroup? selectedGroup;

        public FormPlugins(){
            InitializeComponent();

            Text = Program.BrandName+" Plugins";
        }

        public FormPlugins(PluginManager pluginManager) : this(){
            this.pluginManager = pluginManager;
            this.pluginManager.Reloaded += pluginManager_Reloaded;

            this.flowLayoutPlugins = new PluginListFlowLayout();
            this.flowLayoutPlugins.Resize += flowLayoutPlugins_Resize;

            this.tabPanelPlugins.SetupTabPanel(90);
            this.tabPanelPlugins.ReplaceContent(flowLayoutPlugins);

            this.tabBtnOfficial = tabPanelPlugins.AddButton("", () => SelectGroup(PluginGroup.Official));
            this.tabBtnCustom = tabPanelPlugins.AddButton("", () => SelectGroup(PluginGroup.Custom));

            this.tabPanelPlugins.SelectTab(tabBtnOfficial);
            this.pluginManager_Reloaded(pluginManager, null);

            Shown += (sender, args) => {
                Program.UserConfig.PluginsWindow.Restore(this, false);
            };

            FormClosed += (sender, args) => {
                Program.UserConfig.PluginsWindow.Save(this);
                Program.UserConfig.Save();
            };
        }

        private void SelectGroup(PluginGroup group){
            if (selectedGroup.HasValue && selectedGroup == group)return;

            selectedGroup = group;
            
            ReloadPluginTab();
        }

        public void ReloadPluginTab(){
            if (!selectedGroup.HasValue)return;

            flowLayoutPlugins.SuspendLayout();
            flowLayoutPlugins.Controls.Clear();

            Plugin[] plugins = pluginManager.GetPluginsByGroup(selectedGroup.Value).OrderBy(plugin => !plugin.CanRun ? 0 : pluginManager.Config.IsEnabled(plugin) ? 1 : 2).ThenBy(plugin => plugin.Name).ToArray();

            for(int index = 0; index < plugins.Length; index++){
                flowLayoutPlugins.Controls.Add(new PluginControl(pluginManager, plugins[index]));

                if (index < plugins.Length-1){
                    flowLayoutPlugins.Controls.Add(new Panel{
                        BackColor = Color.DimGray,
                        Size = new Size(1, 1)
                    });
                }
            }

            flowLayoutPlugins_Resize(flowLayoutPlugins, new EventArgs());
            flowLayoutPlugins.ResumeLayout(true);
        }

        private void pluginManager_Reloaded(object sender, PluginLoadEventArgs e){
            tabBtnOfficial.Text = "Official: "+pluginManager.CountPluginByGroup(PluginGroup.Official);
            tabBtnCustom.Text = "Custom: "+pluginManager.CountPluginByGroup(PluginGroup.Custom);
        }

        private void flowLayoutPlugins_Resize(object sender, EventArgs e){
            if (flowLayoutPlugins.Controls.Count == 0){
                return;
            }

            Control lastControl = flowLayoutPlugins.Controls[flowLayoutPlugins.Controls.Count-1];
            bool showScrollBar = lastControl.Location.Y+lastControl.Height >= flowLayoutPlugins.Height;
            int horizontalOffset = showScrollBar ? SystemInformation.VerticalScrollBarWidth : 0;
            
            flowLayoutPlugins.AutoScroll = showScrollBar;
            flowLayoutPlugins.VerticalScroll.Visible = showScrollBar;

            foreach(Control control in flowLayoutPlugins.Controls){
                control.Width = flowLayoutPlugins.Width-control.Margin.Horizontal-horizontalOffset;
            }

            flowLayoutPlugins.Focus();
        }

        private void btnOpenFolder_Click(object sender, EventArgs e){
            using(Process.Start("explorer.exe", "\""+pluginManager.PathCustomPlugins+"\"")){}
        }

        private void btnReload_Click(object sender, EventArgs e){
            if (MessageBox.Show("This will also reload the browser window. Do you want to proceed?", "Reloading Plugins", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                pluginManager.Reload();
                ReloadPluginTab();
            }
        }

        private void btnClose_Click(object sender, EventArgs e){
            Close();
        }
    }
}
