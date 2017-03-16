using System.Windows.Forms;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Other{
    sealed partial class FormAbout : Form{
        private const string TipsLink = "https://github.com/chylex/TweetDuck/wiki";
        private const string IssuesLink = "https://github.com/chylex/TweetDuck/issues";

        public FormAbout(){
            InitializeComponent();

            Text = "About "+Program.BrandName+" "+Program.VersionTag;

            labelDescription.Text = Program.BrandName+" was created by chylex as a replacement to the discontinued official TweetDeck client for Windows.\n\nThe program is available for free under the open source MIT license.";
            
            labelWebsite.Links.Add(new LinkLabel.Link(0, labelWebsite.Text.Length, Program.Website));
            labelTips.Links.Add(new LinkLabel.Link(0, labelTips.Text.Length, TipsLink));
            labelIssues.Links.Add(new LinkLabel.Link(0, labelIssues.Text.Length, IssuesLink));
        }

        private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e){
            BrowserUtils.OpenExternalBrowserUnsafe(e.Link.LinkData as string);
        }
    }
}
