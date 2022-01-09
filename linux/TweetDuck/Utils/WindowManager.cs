using System;
using System.IO;
using Gtk;
using Lunixo.ChromiumGtk.Interop;

namespace TweetDuck.Utils {
	static class WindowManager {
		public static Window? MainWindow { get; private set; }

		public static Window CreateWindow(string title) {
			Window window = new Window(title);
			window.SetIconFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico"));
			InteropLinux.SetDefaultWindowVisual(window.Handle);
			return window;
		}

		public static Window CreateMainWindow(string title) {
			Window window = CreateWindow(title);
			MainWindow = window;
			return window;
		}
	}
}
