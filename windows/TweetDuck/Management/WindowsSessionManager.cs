using System;
using Microsoft.Win32;
using Win = System.Windows.Forms;

namespace TweetDuck.Management {
	static class WindowsSessionManager {
		public static event EventHandler<bool>? LockStateChanged;

		public static void Register() {
			Win.Application.ApplicationExit += OnApplicationExit;
			SystemEvents.SessionSwitch += OnSessionSwitch;
		}

		private static void OnApplicationExit(object? sender, EventArgs e) {
			SystemEvents.SessionSwitch -= OnSessionSwitch;
		}

		private static void OnSessionSwitch(object? sender, SessionSwitchEventArgs e) {
			var reason = e.Reason;
			if (reason == SessionSwitchReason.SessionLock) {
				SetLocked(true);
			}
			else if (reason == SessionSwitchReason.SessionUnlock) {
				SetLocked(false);
			}
		}

		private static void SetLocked(bool newState) {
			LockStateChanged?.Invoke(null, newState);
		}
	}
}
