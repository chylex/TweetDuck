using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TweetLib.Browser.CEF.Utils;
using TweetLib.Core;

namespace TweetDuck.Management {
	static class BrowserCache {
		private static string CacheFolder => CefUtils.GetCacheFolder(App.StoragePath);

		private static bool clearOnExit;
		private static Timer autoClearTimer;

		private static long CalculateCacheSize() {
			return new DirectoryInfo(CacheFolder).EnumerateFiles().Select(file => {
				try {
					return file.Length;
				} catch {
					return 0L;
				}
			}).Sum();
		}

		public static void GetCacheSize(Action<Task<long>> callbackBytes) {
			var task = new Task<long>(CalculateCacheSize);
			task.ContinueWith(callbackBytes);
			task.Start();
		}

		public static void RefreshTimer() {
			bool shouldRun = Program.Config.System.ClearCacheAutomatically && !clearOnExit;

			if (!shouldRun && autoClearTimer != null) {
				autoClearTimer.Dispose();
				autoClearTimer = null;
			}
			else if (shouldRun && autoClearTimer == null) {
				autoClearTimer = new Timer(state => {
					if (autoClearTimer != null) {
						try {
							if (CalculateCacheSize() >= Program.Config.System.ClearCacheThreshold * 1024L * 1024L) {
								SetClearOnExit();
							}
						} catch (Exception) {
							// TODO should probably log errors and report them at some point
						}
					}
				}, null, TimeSpan.FromSeconds(30), TimeSpan.FromHours(4));
			}
		}

		public static void SetClearOnExit() {
			clearOnExit = true;
			RefreshTimer();
		}

		public static void TryClearNow() {
			try {
				Directory.Delete(CacheFolder, true);
			} catch {
				// welp, too bad
			}
		}

		public static void Exit() {
			if (autoClearTimer != null) {
				autoClearTimer.Dispose();
				autoClearTimer = null;
			}

			if (clearOnExit) {
				TryClearNow();
			}
		}
	}
}
