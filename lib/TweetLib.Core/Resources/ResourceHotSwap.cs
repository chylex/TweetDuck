#if DEBUG
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TweetDuck;

namespace TweetLib.Core.Resources {
	public static class ResourceHotSwap {
		private static readonly string HotSwapProjectRoot = FixPathSlash(Path.GetFullPath(Path.Combine(App.ProgramPath, "../../../../../")));
		private static readonly string HotSwapTargetDir = FixPathSlash(Path.GetFullPath(Path.Combine(App.ProgramPath, "../../../bin/tmp")));

		private static string FixPathSlash(string path) {
			return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + '\\';
		}

		public static void Run() {
			Debug.WriteLine("Performing resource hot swap...");
			
			string resourcesRoot = Path.Combine(HotSwapProjectRoot, "resources");
			if (!Directory.Exists(resourcesRoot)) {
				Debug.WriteLine("Failed resource hot swap, cannot find resources folder: " + resourcesRoot);
				return;
			}

			Stopwatch sw = Stopwatch.StartNew();
			
			DeleteHotSwapFolder();
			Directory.CreateDirectory(HotSwapTargetDir);
			Directory.CreateDirectory(Path.Combine(HotSwapTargetDir, "plugins"));
			Directory.CreateDirectory(Path.Combine(HotSwapTargetDir, "plugins", "user"));
			CopyDirectory(Path.Combine(resourcesRoot, "Content"), Path.Combine(HotSwapTargetDir, "resources"));
			CopyDirectory(Path.Combine(resourcesRoot, "Guide"), Path.Combine(HotSwapTargetDir, "guide"));
			CopyDirectory(Path.Combine(resourcesRoot, "Plugins"), Path.Combine(HotSwapTargetDir, "plugins", "official"));
			Directory.Move(Path.Combine(HotSwapTargetDir, "plugins", "official", ".debug"), Path.Combine(HotSwapTargetDir, "plugins", "user", ".debug"));

			foreach (var file in Directory.EnumerateFiles(Path.Combine(HotSwapTargetDir, "plugins"), "*.meta", SearchOption.AllDirectories)) {
				var lines = File.ReadLines(file, Encoding.UTF8)
				                .Select(static line => line.Replace("{version}", Version.Tag))
				                .ToArray();
				
				File.WriteAllLines(file, lines, Encoding.UTF8);
			}

			sw.Stop();
			Debug.WriteLine($"Finished resource hot swap in {sw.ElapsedMilliseconds} ms");

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
