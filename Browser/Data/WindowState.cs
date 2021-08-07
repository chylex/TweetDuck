using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Controls;
using TweetLib.Core.Serialization.Converters;
using TweetLib.Core.Utils;

namespace TweetDuck.Browser.Data {
	sealed class WindowState {
		private Rectangle rect;
		private bool isMaximized;

		public void Save(Form form) {
			rect = form.WindowState == FormWindowState.Normal ? form.DesktopBounds : form.RestoreBounds;
			isMaximized = form.WindowState == FormWindowState.Maximized;
		}

		public void Restore(Form form, bool firstTimeFullscreen) {
			if (rect != Rectangle.Empty) {
				form.DesktopBounds = rect;
				form.WindowState = isMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
			}

			if ((rect == Rectangle.Empty && firstTimeFullscreen) || form.IsFullyOutsideView()) {
				form.DesktopBounds = Screen.PrimaryScreen.WorkingArea;
				form.WindowState = FormWindowState.Maximized;
				Save(form);
			}
		}

		public static readonly SingleTypeConverter<WindowState> Converter = new SingleTypeConverter<WindowState> {
			ConvertToString = value => $"{(value.isMaximized ? 'M' : '_')}{value.rect.X} {value.rect.Y} {value.rect.Width} {value.rect.Height}",
			ConvertToObject = value => {
				int[] elements = StringUtils.ParseInts(value.Substring(1), ' ');

				return new WindowState {
					rect = new Rectangle(elements[0], elements[1], elements[2], elements[3]),
					isMaximized = value[0] == 'M'
				};
			}
		};
	}
}
