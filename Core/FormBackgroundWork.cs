using System;
using System.Windows.Forms;

namespace TweetDick.Core{
    public partial class FormBackgroundWork : Form{
        public FormBackgroundWork(){
            InitializeComponent();

            labelDescription.Rtf = RichTextLabel.Wrap(@"Please, watch this informationless progress bar showcase while some magic happens in the background...");
        }

        public void ShowWorkDialog(Action onBegin){
            Shown += (sender, args) => onBegin();
            ShowDialog();
        }
    }
}
