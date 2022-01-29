using System;
using System.Drawing;
using System.Windows.Forms;

namespace TweetDuck.Controls {
	sealed class LabelVertical : Label {
		public int LineHeight { get; set; }

		protected override void OnPaint(PaintEventArgs e) {
			int y = (int) Math.Floor((ClientRectangle.Height - Text.Length * LineHeight) / 2F) - 1;
			using Brush brush = new SolidBrush(ForeColor);

			foreach (char chr in Text) {
				string str = chr.ToString();
				float x = (ClientRectangle.Width - e.Graphics.MeasureString(str, Font).Width) / 2F;

				e.Graphics.DrawString(str, Font, brush, x, y);
				y += LineHeight;
			}
		}
	}
}
