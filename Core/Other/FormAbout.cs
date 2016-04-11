using System.Text;
using System.Windows.Forms;
using TweetDick.Core.Controls;

namespace TweetDick.Core.Other{
    sealed partial class FormAbout : Form{
        public FormAbout(){
            InitializeComponent();

            Text = "About "+Program.BrandName;

            StringBuilder build = new StringBuilder();
            build.Append(@"\fs22").Append(Program.BrandName).Append(@" was created by chylex as a replacement to the discontinued TweetDeck client for Windows, and is released under the MIT license.\par ");
            build.Append(@"Official Website: ").Append(RichTextLabel.AddLink(Program.Website)).Append(@"\line ");
            build.Append(@"Source Code: ").Append(RichTextLabel.AddLink("https://github.com/chylex/TweetDick"));

            labelAbout.Rtf = RichTextLabel.Wrap(build.ToString());
        }
    }
}
