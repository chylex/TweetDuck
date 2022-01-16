#if DEBUG
using System.Diagnostics;
using System.IO;
using TweetLib.Core;

namespace TweetDuck.Resources {
	static class ResourceHotSwap {
		private static readonly string HotSwapProjectRoot = FixPathSlash(Path.GetFullPath(Path.Combine(App.ProgramPath, "../../../")));
		private static readonly string HotSwapTargetDir = FixPathSlash(Path.Combine(HotSwapProjectRoot, "bin", "tmp"));
		private static readonly string HotSwapRebuildScript = Path.Combine(HotSwapProjectRoot, "Resources", "PostBuild.ps1");

		private static string FixPathSlash(string path) {
			return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + '\\';
		}

		public static void Run() {
			if (!File.Exists(HotSwapRebuildScript)) {
				Debug.WriteLine($"Failed resource hot swap, missing rebuild script: {HotSwapRebuildScript}");
				return;
			}

			Debug.WriteLine("Performing resource hot swap...");

			DeleteHotSwapFolder();
			Directory.CreateDirectory(HotSwapTargetDir);
			Directory.CreateDirectory(Path.Combine(HotSwapTargetDir, "plugins"));
			Directory.CreateDirectory(Path.Combine(HotSwapTargetDir, "plugins", "user"));
			CopyDirectory(Path.Combine(HotSwapProjectRoot, "Resources", "Content"), Path.Combine(HotSwapTargetDir, "resources"));
			CopyDirectory(Path.Combine(HotSwapProjectRoot, "Resources", "Guide"), Path.Combine(HotSwapTargetDir, "guide"));
			CopyDirectory(Path.Combine(HotSwapProjectRoot, "Resources", "Plugins"), Path.Combine(HotSwapTargetDir, "plugins", "official"));
			Directory.Move(Path.Combine(HotSwapTargetDir, "plugins", "official", ".debug"), Path.Combine(HotSwapTargetDir, "plugins", "user", ".debug"));

			Stopwatch sw = Stopwatch.StartNew();

			using (Process process = Process.Start(new ProcessStartInfo {
				FileName = "powershell",
				Arguments = $"-NoProfile -ExecutionPolicy Unrestricted -File \"{HotSwapRebuildScript}\" \"{HotSwapTargetDir}\\\" \"{Program.VersionTag}\"",
				WindowStyle = ProcessWindowStyle.Hidden
			})) {
				// ReSharper disable once PossibleNullReferenceException
				if (!process.WaitForExit(8000)) {
					Debug.WriteLine("Failed resource hot swap, script did not finish in time");
					return;
				}
				else if (process.ExitCode != 0) {
					Debug.WriteLine($"Failed resource hot swap, script exited with code {process.ExitCode}");
					return;
				}
			}

			sw.Stop();
			Debug.WriteLine($"Finished rebuild script in {sw.ElapsedMilliseconds} ms");

			Directory.Delete(App.ResourcesPath, true);
			Directory.Delete(App.GuidePath, true);
			Directory.Delete(App.PluginPath, true);

			Directory.Move(Path.Combine(HotSwapTargetDir, "resources"), App.ResourcesPath);
			Directory.Move(Path.Combine(HotSwapTargetDir, "guide"), App.GuidePath);
			Directory.Move(Path.Combine(HotSwapTargetDir, "plugins"), App.PluginPath);

			DeleteHotSwapFolder();
		}

		private static void DeleteHotSwapFolder() {
			try {
				Directory.Delete(HotSwapTargetDir, true);
			} catch (DirectoryNotFoundException) {}
		}

		private static void CopyDirectory(string from, string to) {
			foreach (var directory in Directory.GetDirectories(from)) {
				string name = Path.GetFileName(directory);
				Directory.CreateDirectory(Path.Combine(to, name));
				CopyDirectory(directory, Path.Combine(to, name));
			}

			foreach (var file in Directory.GetFiles(from)) {
				File.Copy(file, Path.Combine(to, Path.GetFileName(file)));
			}
		}
	}
}
#endif
