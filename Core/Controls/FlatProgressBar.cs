using System.Drawing;
using System.Windows.Forms;

namespace TweetDick.Core.Controls{
    public partial class FlatProgressBar : ProgressBar{
        private SolidBrush brush;

        public FlatProgressBar(){
            SetStyle(ControlStyles.UserPaint,true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer,true);
        }

        public void SetValueInstant(int value){
            if (value == Maximum){
                Value = value;
                Value = value-1;
                Value = value;
            }
            else{
                Value = value+1;
                Value = value;
            }
        }

        protected override void OnPaint(PaintEventArgs e){
            if (brush == null || brush.Color != ForeColor){
                brush = new SolidBrush(ForeColor);
            }

            Rectangle rect = e.ClipRectangle;
            rect.Width = (int)(rect.Width*((double)Value/Maximum));
            e.Graphics.FillRectangle(brush,rect);
        }
    }
}
