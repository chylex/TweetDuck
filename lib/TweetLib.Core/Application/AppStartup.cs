using System;
using System.IO;
using System.Linq;

namespace TweetLib.Core.Application {
	public sealed class AppStartup {
		public string? CustomDataFolder { get; set; }

		internal string GetDataFolder() {
			string? custom = CustomDataFolder;

			if (custom != null && (custom.Contains(Path.DirectorySeparatorChar) || custom.Contains(Path.AltDirectorySeparatorChar))) {
				if (Path.GetInvalidPathChars().Any(custom.Contains)) {
					throw new AppException("Data Folder Invalid", "The data folder contains invalid characters:\n" + custom);
				}
				else if (!Path.IsPathRooted(custom)) {
					throw new AppException("Data Folder Invalid", "The data folder has to be either a simple folder name, or a full path:\n" + custom);
				}

				return Environment.ExpandEnvironmentVariables(custom);
			}
			else {
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), custom ?? Lib.BrandName);
			}
		}
	}
}
