using System;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace TweetDuck.Video{
    partial class FormPlayer : Form{
        public FormPlayer(){
            InitializeComponent();

            player.Ocx.enableContextMenu = false;
            player.Ocx.uiMode = "none";
        }

        private void FormPlayer_Load(object sender, EventArgs e){
        }
    }
}
