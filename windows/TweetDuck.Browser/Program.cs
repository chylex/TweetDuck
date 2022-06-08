using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CefSharp.Core;

namespace TweetDuck.Browser {
	static class Program {
		private const string ParentIdPrefix = "--host-process-id=";

		private static int Main(string[] args) {
			if (!int.TryParse(FindArg(args, ParentIdPrefix), out int parentId)) {
				return 0;
			}

			Task.Factory.StartNew(() => KillWhenHung(parentId), TaskCreationOptions.LongRunning);

			Cef.EnableHighDPISupport();
			return CefSharp.BrowserSubprocess.SelfHost.Main(args);
		}

		private static string? FindArg(string[] args, string key) {
			return Array.Find(args, arg => arg.StartsWith(key, StringComparison.OrdinalIgnoreCase))?[key.Length..];
		}

		private static async void KillWhenHung(int parentId) {
			try {
				using Process process = Process.GetProcessById(parentId);
				await process.WaitForExitAsync();
			} catch {
				// ded
			}

			await Task.Delay(10000);
			Environment.Exit(0);
		}
	}
}
