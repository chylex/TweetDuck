using System;
using System.IO;

namespace TweetLib.Utils.Static {
	public static class FileUtils {
		public static void CreateDirectoryForFile(string file) {
			string? dir = Path.GetDirectoryName(file);

			if (dir == null) {
				throw new ArgumentException("Invalid file path: " + file);
			}
			else if (dir.Length > 0) {
				Directory.CreateDirectory(dir);
			}
		}

		public static bool CheckFolderWritePermission(string path) {
			string testFile = Path.Combine(path, ".test");

			try {
				Directory.CreateDirectory(path);

				using (File.Create(testFile)) {}

				File.Delete(testFile);
				return true;
			} catch {
				return false;
			}
		}

		public static bool FileExistsAndNotEmpty(string path) {
			try {
				return new FileInfo(path).Length > 0;
			} catch {
				return false;
			}
		}

		public static string ResolveRelativePathSafely(string rootFolder, string relativePath) {
			string fullPath = Path.Combine(rootFolder, relativePath);

			try {
				string folderPathName = new DirectoryInfo(rootFolder).FullName;
				DirectoryInfo currentInfo = new DirectoryInfo(fullPath); // initially points to the file, which is convenient for the Attributes check below
				DirectoryInfo? parentInfo = currentInfo.Parent;

				while (parentInfo != null) {
					if (currentInfo.Exists && currentInfo.Attributes.HasFlag(FileAttributes.ReparsePoint)) {
						return string.Empty; // no reason why there should be any files/folders with symlinks, junctions, or any other crap
					}

					if (parentInfo.FullName == folderPathName) {
						return fullPath;
					}

					currentInfo = parentInfo;
					parentInfo = currentInfo.Parent;
				}
			} catch {
				// ignore
			}

			return string.Empty;
		}
	}
}
