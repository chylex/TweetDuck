using System.Drawing;
using System.Windows.Forms;

namespace TweetDuck.Video{
    sealed class FlatProgressBar : ProgressBar{
        private readonly SolidBrush brush;

        public FlatProgressBar(){
            brush = new SolidBrush(Color.White);

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e){
            if (brush.Color != ForeColor){
                brush.Color = ForeColor;
            }

            Rectangle rect = e.ClipRectangle;
            rect.Width = (int)(rect.Width*((double)Value/Maximum));
            e.Graphics.FillRectangle(brush, rect);
        }

        protected override void Dispose(bool disposing){
            base.Dispose(disposing);

            if (disposing){
                brush.Dispose();
            }
        }
    }
}
