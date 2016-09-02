using System;
using System.Drawing;
using System.Windows.Forms;

namespace TweetDck.Core.Controls{
    sealed partial class TabButton : FlatButton{
        public Action Callback { get; private set; }

        public void SetupButton(int locationX, int sizeWidth, string title, Action callback){
            Callback = callback;

            SuspendLayout();
            FlatAppearance.BorderColor = Color.DimGray;
            FlatAppearance.MouseDownBackColor = Color.White;
            FlatAppearance.MouseOverBackColor = Color.White;
            FlatStyle = FlatStyle.Flat;
            Location = new Point(locationX, 0);
            Margin = new Padding(0);
            Size = new Size(sizeWidth, 30);
            Text = title;
            UseVisualStyleBackColor = true;
            ResumeLayout(true);
        }
    }
}
