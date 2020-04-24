using System;
using System.Windows.Forms;
using TweetDuck.Core.Other.Analytics;
using TweetDuck.Core.Other.Settings.Dialogs;
using TweetDuck.Core.Utils;
using TweetLib.Core.Features.Plugins;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsFeedback : BaseTabSettings{
        private readonly AnalyticsFile analyticsFile;
        private readonly AnalyticsReportGenerator.ExternalInfo analyticsInfo;
        private readonly PluginManager plugins;

        public TabSettingsFeedback(AnalyticsManager analytics, AnalyticsReportGenerator.ExternalInfo analyticsInfo, PluginManager plugins){
            InitializeComponent();
            
            this.analyticsFile = analytics?.File ?? AnalyticsFile.Load(Program.AnalyticsFilePath);
            this.analyticsInfo = analyticsInfo;
            this.plugins = plugins;

            // feedback

            checkDataCollection.Checked = Config.AllowDataCollection;

            if (analytics != null){
                string collectionTime = analyticsFile.LastCollectionMessage;
                labelDataCollectionMessage.Text = string.IsNullOrEmpty(collectionTime) ? "No collection yet" : "Last collection: " + collectionTime;
            }
        }

        public override void OnReady(){
            btnSendFeedback.Click += btnSendFeedback_Click;
            checkDataCollection.CheckedChanged += checkDataCollection_CheckedChanged;
            labelDataCollectionLink.LinkClicked += labelDataCollectionLink_LinkClicked;
            btnViewReport.Click += btnViewReport_Click;
        }

        #region Feedback

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
            using DialogSettingsAnalytics dialog = new DialogSettingsAnalytics(AnalyticsReportGenerator.Create(analyticsFile, analyticsInfo, plugins));
            dialog.ShowDialog();
        }

        #endregion
    }
}
