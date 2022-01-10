using System;
using System.ComponentModel;
using System.Diagnostics;
using TweetDuck.Dialogs;
using TweetDuck.Utils;
using TweetLib.Core;
using TweetLib.Core.Systems.Startup;

namespace TweetDuck.Management {
	sealed class LockManager {
		private const int WaitRetryDelay = 250;
		private const int RestoreFailTimeout = 2000;
		private const int CloseNaturallyTimeout = 10000;
		private const int CloseKillTimeout = 5000;

		public uint WindowRestoreMessage { get; } = NativeMethods.RegisterWindowMessage("TweetDuckRestore");

		private readonly LockFile lockFile;

		public LockManager(string path) {
			this.lockFile = new LockFile(path);
		}

		public bool Lock(bool wasRestarted) {
			return wasRestarted ? LaunchAfterRestart() : LaunchNormally();
		}

		public bool Unlock() {
			return lockFile.Unlock();
		}

		// Locking

		private bool LaunchNormally() {
			LockResult lockResult = lockFile.Lock();

			if (lockResult is LockResult.HasProcess info) {
				if (!RestoreProcess(info.Process, WindowRestoreMessage) && FormMessage.Error("TweetDuck is Already Running", "Another instance of TweetDuck is already running.\nDo you want to close it?", FormMessage.Yes, FormMessage.No)) {
					if (!CloseProcess(info.Process)) {
						FormMessage.Error("TweetDuck Has Failed :(", "Could not close the other process.", FormMessage.OK);
						return false;
					}

					info.Dispose();
					lockResult = lockFile.Lock();
				}
				else {
					return false;
				}
			}

			if (lockResult is LockResult.Fail fail) {
				ShowGenericException(fail);
				return false;
			}
			else if (lockResult != LockResult.Success) {
				FormMessage.Error("TweetDuck Has Failed :(", "An unknown error occurred accessing the data folder. Please, make sure TweetDuck is not already running. If the problem persists, try restarting your system.", FormMessage.OK);
				return false;
			}

			return true;
		}

		private bool LaunchAfterRestart() {
			LockResult lockResult = lockFile.LockWait(10000, WaitRetryDelay);

			while (lockResult != LockResult.Success) {
				if (lockResult is LockResult.Fail fail) {
					ShowGenericException(fail);
					return false;
				}
				else if (!FormMessage.Warning("TweetDuck Cannot Restart", "TweetDuck is taking too long to close.", FormMessage.Retry, FormMessage.Exit)) {
					return false;
				}

				lockResult = lockFile.LockWait(5000, WaitRetryDelay);
			}

			return true;
		}

		// Helpers

		private static void ShowGenericException(LockResult.Fail fail) {
			App.ErrorHandler.HandleException("TweetDuck Has Failed :(", "An unknown error occurred accessing the data folder. Please, make sure TweetDuck is not already running. If the problem persists, try restarting your system.", false, fail.Exception);
		}

		private static bool RestoreProcess(Process process, uint windowRestoreMessage) {
			if (process.MainWindowHandle == IntPtr.Zero) { // restore if the original process is in tray
				NativeMethods.BroadcastMessage(windowRestoreMessage, (uint) process.Id, 0);

				if (WindowsUtils.TrySleepUntil(() => CheckProcessExited(process) || (process.MainWindowHandle != IntPtr.Zero && process.Responding), RestoreFailTimeout, WaitRetryDelay)) {
					return true;
				}
			}

			return false;
		}

		private static bool CloseProcess(Process process) {
			try {
				if (process.CloseMainWindow()) {
					// ReSharper disable once AccessToDisposedClosure
					WindowsUtils.TrySleepUntil(() => CheckProcessExited(process), CloseNaturallyTimeout, WaitRetryDelay);
				}

				if (!process.HasExited) {
					process.Kill();
					// ReSharper disable once AccessToDisposedClosure
					WindowsUtils.TrySleepUntil(() => CheckProcessExited(process), CloseKillTimeout, WaitRetryDelay);
				}

				if (process.HasExited) {
					process.Dispose();
					return true;
				}
				else {
					return false;
				}
			} catch (Exception ex) when (ex is InvalidOperationException || ex is Win32Exception) {
				bool hasExited = CheckProcessExited(process);
				process.Dispose();
				return hasExited;
			}
		}

		private static bool CheckProcessExited(Process process) {
			process.Refresh();
			return process.HasExited;
		}
	}
}
