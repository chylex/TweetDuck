using System;
using System.Windows.Forms;

namespace TweetDck.Migration{
    partial class FormBackgroundWork : Form{
        public FormBackgroundWork(){
            InitializeComponent();
        }

        public void ShowWorkDialog(Action onBegin){
            Shown += (sender, args) => onBegin();
            ShowDialog();
        }
    }
}
