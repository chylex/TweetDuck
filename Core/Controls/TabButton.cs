using System;
using System.Drawing;
using System.Windows.Forms;

namespace TweetDck.Core.Controls{
    sealed partial class TabButton : Button{
        protected override bool ShowFocusCues{
            get{
                return false;
            }
        }

        public Action Callback { get; private set; }

        public TabButton(){
            GotFocus += TabButton_GotFocus;
        }

        public void SetupButton(int locationX, int sizeWidth, string title, Action callback){
            Callback = callback;

            SuspendLayout();
            FlatAppearance.BorderColor = Color.DimGray;
            FlatAppearance.MouseDownBackColor = Color.White;
            FlatAppearance.MouseOverBackColor = Color.White;
            FlatStyle = FlatStyle.Flat;
            Location = new Point(locationX,0);
            Margin = new Padding(0);
            Size = new Size(sizeWidth,30);
            Text = title;
            UseVisualStyleBackColor = true;
            ResumeLayout(true);
        }

        private void TabButton_GotFocus(object sender, EventArgs e){ // removes extra border when focused
            NotifyDefault(false);
        }
    }
}
