using System;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other.Analytics;

namespace TweetDuck.Core.Other.Settings.Dialogs{
    sealed partial class DialogSettingsAnalytics : Form{
        public string CefArgs => textBoxReport.Text;

        public DialogSettingsAnalytics(AnalyticsReport report){
            InitializeComponent();
            
            Text = Program.BrandName+" Options - Analytics Report";
            
            textBoxReport.EnableMultilineShortcuts();
            textBoxReport.Text = report.ToString().TrimEnd();
            textBoxReport.Select(0, 0);
        }

        private void btnClose_Click(object sender, EventArgs e){
            Close();
        }
    }
}
