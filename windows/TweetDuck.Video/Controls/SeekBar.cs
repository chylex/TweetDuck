#nullable enable
using System.Drawing;
using System.Windows.Forms;

namespace TweetDuck.Video.Controls {
	sealed class SeekBar : ProgressBar {
		private readonly SolidBrush brushFore;
		private readonly SolidBrush brushHover;
		private readonly SolidBrush brushOverlap;
		private readonly SolidBrush brushBack;

		public SeekBar() {
			brushFore = new SolidBrush(Color.White);
			brushHover = new SolidBrush(Color.White);
			brushOverlap = new SolidBrush(Color.White);
			brushBack = new SolidBrush(Color.White);

			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		public double GetProgress(int clientX) {
			return clientX / (Width - 1.0);
		}

		protected override void OnPaint(PaintEventArgs e) {
			if (brushFore.Color != ForeColor) {
				brushFore.Color = ForeColor;
				brushHover.Color = Color.FromArgb(128, ForeColor);
				brushOverlap.Color = Color.FromArgb(80 + ForeColor.R * 11 / 16, 80 + ForeColor.G * 11 / 16, 80 + ForeColor.B * 11 / 16);
				brushBack.Color = Parent?.BackColor ?? Color.Black;
			}

			Rectangle rect = new Rectangle(0, 0, Width, Height);
			Point cursor = PointToClient(Cursor.Position);
			int width = rect.Width - 1;
			int progress = (int) (width * ((double) Value / Maximum));

			rect.Width = progress;
			rect.Height -= 1;
			e.Graphics.FillRectangle(brushFore, rect);

			if (cursor.X >= 0 && cursor.Y >= 0 && cursor.X <= width && cursor.Y <= rect.Height) {
				if (progress >= cursor.X) {
					rect.Width = cursor.X;
					e.Graphics.FillRectangle(brushOverlap, rect);
				}
				else {
					rect.X = progress;
					rect.Width = cursor.X - rect.X;
					e.Graphics.FillRectangle(brushHover, rect);
				}
			}

			rect.X = width;
			rect.Width = 1;
			rect.Height += 1;
			e.Graphics.FillRectangle(brushBack, rect);

			rect.X = 0;
			rect.Y = rect.Height - 1;
			rect.Width = width;
			rect.Height = 1;
			e.Graphics.FillRectangle(brushBack, rect);
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (disposing) {
				brushFore.Dispose();
				brushHover.Dispose();
				brushOverlap.Dispose();
				brushBack.Dispose();
			}
		}
	}
}
