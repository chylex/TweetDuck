using System;
using System.Windows.Forms;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Settings{
    sealed partial class TabSettingsFeedback : BaseTabSettings{
        public TabSettingsFeedback(){
            InitializeComponent();
            
            checkDataCollection.Checked = Config.AllowDataCollection;
        }

        public override void OnReady(){
            btnSendFeedback.Click += btnSendFeedback_Click;
            checkDataCollection.CheckedChanged += checkDataCollection_CheckedChanged;
        }

        private void btnSendFeedback_Click(object sender, EventArgs e){
            BrowserUtils.OpenExternalBrowserUnsafe("https://github.com/chylex/TweetDuck/issues/new");
        }

        private void checkDataCollection_CheckedChanged(object sender, EventArgs e){
            Config.AllowDataCollection = checkDataCollection.Checked;
        }

        private void labelDataCollectionLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){
            BrowserUtils.OpenExternalBrowserUnsafe("https://github.com/chylex/TweetDuck/wiki/Send-anonymous-data");
        }
    }
}
