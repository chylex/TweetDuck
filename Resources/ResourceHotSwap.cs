#if DEBUG
using System.Diagnostics;
using System.IO;
using TweetLib.Core;

namespace TweetDuck.Resources {
	static class ResourceHotSwap {
		private static readonly string HotSwapProjectRoot = FixPathSlash(Path.GetFullPath(Path.Combine(App.ProgramPath, "../../../")));
		private static readonly string HotSwapTargetDir = FixPathSlash(Path.Combine(HotSwapProjectRoot, "bin", "tmp"));
		private static readonly string HotSwapRebuildScript = Path.Combine(HotSwapProjectRoot, "bld", "post_build.exe");

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

			Stopwatch sw = Stopwatch.StartNew();

			using (Process process = Process.Start(new ProcessStartInfo {
				FileName = HotSwapRebuildScript,
				Arguments = $"\"{HotSwapTargetDir}\\\" \"{HotSwapProjectRoot}\\\" \"Debug\" \"{Program.VersionTag}\"",
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
			Directory.Delete(App.PluginPath, true);

			Directory.Move(Path.Combine(HotSwapTargetDir, "resources"), App.ResourcesPath);
			Directory.Move(Path.Combine(HotSwapTargetDir, "plugins"), App.PluginPath);

			DeleteHotSwapFolder();
		}

		private static void DeleteHotSwapFolder() {
			try {
				Directory.Delete(HotSwapTargetDir, true);
			} catch (DirectoryNotFoundException) {}
		}
	}
}
#endif
