using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Configuration;
using TweetDuck.Plugins;
using TweetLib.Core;
using TweetLib.Core.Features.Plugins;

namespace TweetDuck.Core.Other{
    sealed partial class FormPlugins : Form, FormManager.IAppDialog{
        private static UserConfig Config => Program.Config.User;

        private readonly PluginManager pluginManager;
        
        public FormPlugins(){
            InitializeComponent();

            Text = Program.BrandName+" Plugins";
        }

        public FormPlugins(PluginManager pluginManager) : this(){
            this.pluginManager = pluginManager;

            if (!Config.PluginsWindowSize.IsEmpty){
                Size targetSize = Config.PluginsWindowSize;
                Size = new Size(Math.Max(MinimumSize.Width, targetSize.Width), Math.Max(MinimumSize.Height, targetSize.Height));
            }
            
            Shown += (sender, args) => {
                ReloadPluginList();
            };

            FormClosed += (sender, args) => {
                Config.PluginsWindowSize = Size;
                Config.Save();
            };

            ResizeEnd += (sender, args) => {
                timerLayout.Start();
            };
        }

        private int GetPluginOrderIndex(Plugin plugin){
            return !plugin.CanRun ? 0 : pluginManager.Config.IsEnabled(plugin) ? 1 : 2;
        }

        private void ReloadPluginList(){
            flowLayoutPlugins.Controls.Clear();
            flowLayoutPlugins.SuspendLayout();

            foreach(Plugin plugin in pluginManager.Plugins.OrderBy(GetPluginOrderIndex).ThenBy(plugin => plugin.Name)){
                flowLayoutPlugins.Controls.Add(new PluginControl(pluginManager, plugin));

                flowLayoutPlugins.Controls.Add(new Panel{
                    BackColor = Color.DimGray,
                    Margin = new Padding(0),
                    Size = new Size(1, 1)
                });
            }

            flowLayoutPlugins.ResumeLayout(true);
            
            timerLayout_Tick(null, EventArgs.Empty);
            timerLayout.Start();
        }

        private void timerLayout_Tick(object sender, EventArgs e){
            timerLayout.Stop();
            
            // stupid WinForms scrollbars and panels
            Padding = new Padding(Padding.Left, Padding.Top, Padding.Right+1, Padding.Bottom+1);
            Padding = new Padding(Padding.Left, Padding.Top, Padding.Right-1, Padding.Bottom-1);
        }

        public void flowLayoutPlugins_Resize(object sender, EventArgs e){
            Control lastPlugin = flowLayoutPlugins.Controls.OfType<PluginControl>().LastOrDefault();
            
            if (lastPlugin == null){
                return;
            }

            bool showScrollBar = lastPlugin.Location.Y+lastPlugin.Height+1 >= flowLayoutPlugins.Height;
            int horizontalOffset = showScrollBar ? SystemInformation.VerticalScrollBarWidth : 0;
            
            flowLayoutPlugins.AutoScroll = showScrollBar;
            flowLayoutPlugins.VerticalScroll.Visible = showScrollBar;

            foreach(Control control in flowLayoutPlugins.Controls){
                control.Width = flowLayoutPlugins.Width-control.Margin.Horizontal-horizontalOffset;
            }
            
            flowLayoutPlugins.Controls[flowLayoutPlugins.Controls.Count-1].Visible = !showScrollBar;
            flowLayoutPlugins.Focus();
        }

        private void btnOpenFolder_Click(object sender, EventArgs e){
            App.SystemHandler.OpenFileExplorer(pluginManager.PathCustomPlugins);
        }

        private void btnReload_Click(object sender, EventArgs e){
            if (FormMessage.Warning("Reloading Plugins", "This will also reload the browser window. Do you want to proceed?", FormMessage.Yes, FormMessage.No)){
                pluginManager.Reload();
                ReloadPluginList();
            }
        }

        private void btnClose_Click(object sender, EventArgs e){
            Close();
        }
    }
}
