using System;
using System.Diagnostics;
using System.Windows.Forms;
using TweetDck.Plugins;

namespace TweetDck.Core.Other{
    partial class FormPlugins : Form{
        private readonly PluginManager pluginManager;

        public FormPlugins(){
            InitializeComponent();
        }

        public FormPlugins(PluginManager pluginManager) : this(){
            this.pluginManager = pluginManager;
            this.pluginList.Initialize(pluginManager);
        }

        private void btnOpenFolder_Click(object sender, EventArgs e){
            using(Process.Start("explorer.exe","\""+pluginManager.PathCustomPlugins+"\"")){}
        }

        private void btnReload_Click(object sender, EventArgs e){
            pluginManager.Reload();
            pluginList.ReloadPlugins();
        }

        private void btnClose_Click(object sender, EventArgs e){
            Close();
        }
    }
}
