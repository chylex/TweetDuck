using System;
using System.Windows.Forms;
using TweetDuck.Core.Other.Analytics;
using TweetDuck.Core.Other.Settings.Dialogs;
using TweetDuck.Core.Utils;
using TweetDuck.Plugins;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsFeedback : BaseTabSettings{
        private readonly PluginManager plugins;

        public TabSettingsFeedback(PluginManager plugins){
            InitializeComponent();
            
            this.plugins = plugins;

            checkDataCollection.Checked = Config.AllowDataCollection;
        }

        public override void OnReady(){
            btnSendFeedback.Click += btnSendFeedback_Click;
            checkDataCollection.CheckedChanged += checkDataCollection_CheckedChanged;
            labelDataCollectionLink.LinkClicked += labelDataCollectionLink_LinkClicked;
            btnViewReport.Click += btnViewReport_Click;
        }

        private void btnSendFeedback_Click(object sender, EventArgs e){
            BrowserUtils.OpenExternalBrowser("https://github.com/chylex/TweetDuck/issues/new");
        }

        private void checkDataCollection_CheckedChanged(object sender, EventArgs e){
            Config.AllowDataCollection = checkDataCollection.Checked;
        }

        private void labelDataCollectionLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){
            BrowserUtils.OpenExternalBrowser("https://github.com/chylex/TweetDuck/wiki/Send-anonymous-data");
        }

        private void btnViewReport_Click(object sender, EventArgs e){
            using(DialogSettingsAnalytics dialog = new DialogSettingsAnalytics(AnalyticsReportGenerator.Create(FormManager.TryFind<FormBrowser>(), plugins))){
                dialog.ShowDialog();
            }
        }
    }
}
