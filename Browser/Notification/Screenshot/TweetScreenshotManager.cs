#if DEBUG
// Uncomment to keep screenshot windows visible for debugging
// #define NO_HIDE_SCREENSHOTS

// Uncomment to generate screenshots of individual frames for at most 1 second
// #define GEN_SCREENSHOT_FRAMES
#endif

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweetDuck.Controls;
using TweetLib.Core;
using TweetLib.Core.Features.Plugins;
#if GEN_SCREENSHOT_FRAMES
using System.Drawing.Imaging;
using System.IO;
using TweetDuck.Utils;
#endif

namespace TweetDuck.Browser.Notification.Screenshot {
	sealed class TweetScreenshotManager : IDisposable {
		private readonly FormBrowser owner;
		private readonly PluginManager plugins;
		private readonly Timer timeout;
		private readonly Timer disposer;

		#if GEN_SCREENSHOT_FRAMES
		private readonly Timer debugger;
		private int frameCounter;

		public const int WaitFrames = 60;
		#else
		public const int WaitFrames = 5;
		#endif

		private FormNotificationScreenshotable screenshot;

		public TweetScreenshotManager(FormBrowser owner, PluginManager pluginManager) {
			this.owner = owner;
			this.plugins = pluginManager;

			this.timeout = new Timer { Interval = 8000 };
			this.timeout.Tick += timeout_Tick;

			this.disposer = new Timer { Interval = 1 };
			this.disposer.Tick += disposer_Tick;

			#if GEN_SCREENSHOT_FRAMES
			this.debugger = new Timer { Interval = 16 };
			this.debugger.Tick += debugger_Tick;
			#endif
		}

		private void timeout_Tick(object sender, EventArgs e) {
			timeout.Stop();
			OnFinished();
		}

		private void disposer_Tick(object sender, EventArgs e) {
			disposer.Stop();
			screenshot.Dispose();
			screenshot = null;
		}

		public void Trigger(string html, int width) {
			if (screenshot != null) {
				return;
			}

			screenshot = new FormNotificationScreenshotable(Callback, owner, plugins, html, width);
			screenshot.Show();
			timeout.Start();

			#if GEN_SCREENSHOT_FRAMES
			StartDebugger();
			#endif

			#if !NO_HIDE_SCREENSHOTS
			owner.IsWaiting = true;
			#endif
		}

		private void Callback() {
			if (!timeout.Enabled) {
				return;
			}

			timeout.Stop();
			screenshot.TakeScreenshot().ContinueWith(HandleResult, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void HandleResult(Task<Image> task) {
			if (task.IsFaulted) {
				App.ErrorHandler.HandleException("Screenshot Failed", "An error occurred while taking a screenshot.", true, task.Exception!.InnerException);
			}
			else if (task.IsCompleted) {
				Clipboard.SetImage(task.Result);
				#if !NO_HIDE_SCREENSHOTS
				OnFinished();
				#else
				screenshot.MoveToVisibleLocation();
				screenshot.FormClosed += (sender, args) => disposer.Start();
				#endif
			}
		}

		private void OnFinished() {
			#if GEN_SCREENSHOT_FRAMES
			debugger.Stop();
			#endif

			screenshot.Location = ControlExtensions.InvisibleLocation;
			owner.IsWaiting = false;
			disposer.Start();
		}

		public void Dispose() {
			#if GEN_SCREENSHOT_FRAMES
			debugger.Dispose();
			#endif

			timeout.Dispose();
			disposer.Dispose();
			screenshot?.Dispose();
		}

		#if GEN_SCREENSHOT_FRAMES
		private static readonly string DebugScreenshotPath = Path.Combine(Program.StoragePath, "TD_Screenshots");

		private void StartDebugger() {
			frameCounter = 0;

			try {
				Directory.Delete(DebugScreenshotPath, true);
				WindowsUtils.TrySleepUntil(() => !Directory.Exists(DebugScreenshotPath), 1000, 10);
			} catch (DirectoryNotFoundException) {}

			Directory.CreateDirectory(DebugScreenshotPath);
			debugger.Start();
		}

		private void debugger_Tick(object sender, EventArgs e) {
			if (frameCounter < 63) {
				int frame = ++frameCounter;
				screenshot.TakeScreenshot(true).ContinueWith(task => SaveDebugFrame(task, frame), TaskScheduler.FromCurrentSynchronizationContext());
			}
			else {
				debugger.Stop();
			}
		}

		private static void SaveDebugFrame(Task<Image> task, int frame) {
			if (task.IsFaulted) {
				System.Diagnostics.Debug.WriteLine("Failed generating frame " + frame + ": " + task.Exception!.InnerException);
			}
			else if (task.IsCompleted) {
				task.Result?.Save(Path.Combine(DebugScreenshotPath, "frame_" + (++frame) + ".png"), ImageFormat.Png);
			}
		}
		#endif
	}
}
