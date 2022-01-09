using System;
using System.Diagnostics;
using TweetLib.Core.Application;

namespace TweetDuck.Application {
	sealed class SystemHandler : IAppSystemHandler {
		public void OpenBrowser(string? url) {
			if (string.IsNullOrWhiteSpace(url)) {
				return;
			}

			Console.WriteLine(url);
			using (Process.Start(new ProcessStartInfo {
				FileName = url,
				ErrorDialog = true,
				UseShellExecute = true
			})) {}
		}

		public void OpenFileExplorer(string path) {
			using (Process.Start("xdg-open", path)) {}
		}

		public IAppSystemHandler.OpenAssociatedProgramFunc? OpenAssociatedProgram => null;

		public IAppSystemHandler.CopyImageFromFileFunc? CopyImageFromFile => null;

		public IAppSystemHandler.CopyTextFunc? CopyText => null;

		public IAppSystemHandler.SearchTextFunc? SearchText => null;
	}
}
