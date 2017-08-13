using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TweetDuck.Video.Controls{
    sealed class SeekBar : ProgressBar{
        private readonly SolidBrush brushFore;
        private readonly SolidBrush brushHover;
        private readonly SolidBrush brushOverlap;

        public SeekBar(){
            brushFore = new SolidBrush(Color.White);
            brushHover = new SolidBrush(Color.White);
            brushOverlap = new SolidBrush(Color.White);

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e){
            if (brushFore.Color != ForeColor){
                brushFore.Color = ForeColor;
                brushHover.Color = Color.FromArgb(128, ForeColor);
                brushOverlap.Color = Color.FromArgb(80+ForeColor.R*11/16, 80+ForeColor.G*11/16, 80+ForeColor.B*11/16);
            }
            
            Rectangle rect = e.ClipRectangle;
            Point cursor = PointToClient(Cursor.Position);
            int width = rect.Width;
            int progress = (int)(width*((double)Value/Maximum));

            rect.Width = progress;
            e.Graphics.FillRectangle(brushFore, rect);

            if (cursor.X >= 0 && cursor.Y >= 0 && cursor.X <= width && cursor.Y <= rect.Height){
                if (progress >= cursor.X){
                    rect.Width = cursor.X;
                    e.Graphics.FillRectangle(brushOverlap, rect);
                }
                else{
                    rect.X = progress;
                    rect.Width = cursor.X-rect.X;
                    e.Graphics.FillRectangle(brushHover, rect);
                }
            }
        }

        protected override void Dispose(bool disposing){
            base.Dispose(disposing);

            if (disposing){
                brushFore.Dispose();
                brushHover.Dispose();
                brushOverlap.Dispose();
            }
        }
    }
}
