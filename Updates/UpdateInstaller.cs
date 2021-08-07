using System;
using System.ComponentModel;
using System.Diagnostics;
using TweetDuck.Configuration;
using TweetLib.Core.Utils;

namespace TweetDuck.Updates {
	sealed class UpdateInstaller {
		private string Path { get; }

		public UpdateInstaller(string path) {
			this.Path = path;
		}

		public bool Launch() {
			// ProgramPath has a trailing backslash
			string arguments = "/SP- /SILENT /FORCECLOSEAPPLICATIONS /UPDATEPATH=\"" + Program.ProgramPath + "\" /RUNARGS=\"" + Arguments.GetCurrentForInstallerCmd() + "\"" + (Program.IsPortable ? " /PORTABLE=1" : "");
			bool runElevated = !Program.IsPortable || !FileUtils.CheckFolderWritePermission(Program.ProgramPath);

			try {
				using (Process.Start(new ProcessStartInfo {
					FileName = Path,
					Arguments = arguments,
					Verb = runElevated ? "runas" : string.Empty,
					ErrorDialog = true
				})) {
					return true;
				}
			} catch (Win32Exception e) when (e.NativeErrorCode == 0x000004C7) { // operation canceled by the user
				return false;
			} catch (Exception e) {
				Program.Reporter.HandleException("Update Installer Error", "Could not launch update installer.", true, e);
				return false;
			}
		}
	}
}
