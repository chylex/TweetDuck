using System.Text;
using System.Windows.Forms;
using TweetDick.Core.Controls;

namespace TweetDick.Core.Other{
    public partial class FormAbout : Form{
        public FormAbout(){
            InitializeComponent();

            StringBuilder build = new StringBuilder();
            build.Append(@"\fs22TweetDick was created by chylex as a replacement to the discontinued TweetDeck client for Windows, and is released under the MIT license.\par ");
            build.Append(@"Official Website: ").Append(RichTextLabel.AddLink("http://tweetdick.chylex.com")).Append(@"\line ");
            build.Append(@"Source Code: ").Append(RichTextLabel.AddLink("https://github.com/chylex/TweetDick"));

            labelAbout.Rtf = RichTextLabel.Wrap(build.ToString());
        }
    }
}
