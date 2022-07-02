using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TweetLib.Utils.Data;

namespace TweetDuck.Controls {
	static class ControlExtensions {
		public static readonly Point InvisibleLocation = new (-32000, -32000);
		
		public static readonly Font DefaultFont = new ("Segoe UI", 9F, FontStyle.Regular);

		public static void InvokeSafe(this Control control, Action func) {
			if (control.InvokeRequired) {
				control.Invoke(func);
			}
			else {
				func();
			}
		}

		public static void InvokeAsyncSafe(this Control control, Action func) {
			if (control.InvokeRequired) {
				control.BeginInvoke(func);
			}
			else {
				func();
			}
		}

		public static float GetDPIScale(this Control control) {
			using Graphics graphics = control.CreateGraphics();
			return graphics.DpiY / 96F;
		}

		public static bool IsFullyOutsideView(this Form form) {
			return !Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(form.Bounds));
		}

		public static void MoveToCenter(this Form targetForm, Form parentForm) {
			targetForm.Location = new Point(parentForm.Location.X + (parentForm.Width / 2) - (targetForm.Width / 2), parentForm.Location.Y + (parentForm.Height / 2) - (targetForm.Height / 2));
		}

		public static void SetValueInstant(this ProgressBar bar, int value) {
			if (value == bar.Maximum) {
				bar.Value = value;
				bar.Value = value - 1;
				bar.Value = value;
			}
			else {
				bar.Value = value + 1;
				bar.Value = value;
			}
		}

		public static void SetValueSafe(this NumericUpDown numUpDown, int value) {
			if (value >= numUpDown.Minimum && value <= numUpDown.Maximum) {
				numUpDown.Value = value;
			}
		}

		public static void SetValueSafe(this TrackBar trackBar, int value) {
			if (value >= trackBar.Minimum && value <= trackBar.Maximum) {
				trackBar.Value = value;
			}
		}

		public static bool AlignValueToTick(this TrackBar trackBar) {
			if (trackBar.Value % trackBar.SmallChange != 0) {
				trackBar.Value = trackBar.SmallChange * (int) Math.Floor(((double) trackBar.Value / trackBar.SmallChange) + 0.5);
				return false;
			}

			return true;
		}

		public static void EnableMultilineShortcuts(this TextBox textBox) {
			textBox.KeyDown += static (sender, args) => {
				if (args.Control && args.KeyCode == Keys.A && sender is TextBox tb) {
					tb.SelectAll();
					args.SuppressKeyPress = true;
					args.Handled = true;
				}
			};
		}

		public static void Save(this WindowState state, Form form) {
			state.Bounds = form.WindowState == FormWindowState.Normal ? form.DesktopBounds : form.RestoreBounds;
			state.IsMaximized = form.WindowState == FormWindowState.Maximized;
		}

		public static void Restore(this WindowState state, Form form, bool firstTimeFullscreen) {
			if (state.Bounds != Rectangle.Empty) {
				form.DesktopBounds = state.Bounds;
				form.WindowState = state.IsMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
			}

			if ((state.Bounds == Rectangle.Empty && firstTimeFullscreen) || form.IsFullyOutsideView()) {
				form.DesktopBounds = Screen.PrimaryScreen.WorkingArea;
				form.WindowState = FormWindowState.Maximized;
				state.Save(form);
			}
		}
	}
}
