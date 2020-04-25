using System;
using System.Windows.Forms;
using TweetDuck.Controls;
using TweetDuck.Management.Analytics;

namespace TweetDuck.Dialogs.Settings{
    sealed partial class DialogSettingsAnalytics : Form{
        public DialogSettingsAnalytics(AnalyticsReport report){
            InitializeComponent();
            
            Text = Program.BrandName + " Options - Analytics Report";
            
            textBoxReport.EnableMultilineShortcuts();
            textBoxReport.Text = report.ToString().TrimEnd();
            textBoxReport.Select(0, 0);
        }

        private void btnClose_Click(object sender, EventArgs e){
            Close();
        }
    }
}
