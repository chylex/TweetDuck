using System;
using System.Text;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Core.Utils;

namespace TweetDck.Core.Other{
    sealed partial class FormAbout : Form{
        private const string GitHubLink = "https://github.com/chylex/TweetDuck";

        public FormAbout(){
            InitializeComponent();

            Text = "About "+Program.BrandName+" "+Program.VersionTag;

            StringBuilder build = new StringBuilder();
            build.Append(@"\fs22").Append(Program.BrandName).Append(@" was created by chylex as a replacement to the discontinued TweetDeck client for Windows, and is released under the MIT license.\par ");
            build.Append(@"Official Website: ").Append(RichTextLabel.AddLink(Program.Website)).Append(@"\line ");
            build.Append(@"Source Code: ").Append(RichTextLabel.AddLink(GitHubLink));

            labelAbout.Rtf = RichTextLabel.Wrap(build.ToString());
        }

        private void labelAbout_Click(object sender, EventArgs e){ // LinkClicked isn't working so fuck that
            if (Cursor.Current != Cursors.Hand)return;

            // I don't even give a fuck, someone else PR a proper fix please
            int index = labelAbout.GetCharIndexFromPosition(((MouseEventArgs)e).Location);

            if (IsClickingOn(index,Program.Website)){
                BrowserUtils.OpenExternalBrowser(Program.Website);
            }
            else if (IsClickingOn(index,GitHubLink)){
                BrowserUtils.OpenExternalBrowser(GitHubLink);
            }
        }

        private bool IsClickingOn(int index, string substringSearch){
            int substringIndex = labelAbout.Text.IndexOf(substringSearch,StringComparison.Ordinal);
            return index >= substringIndex && index <= substringIndex+substringSearch.Length;
        }
    }
}
