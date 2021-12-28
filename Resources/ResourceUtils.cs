using System;
using System.IO;
using System.Text;
using TweetLib.Core;

namespace TweetDuck.Resources {
	public static class ResourceUtils {
		public static string ReadFileOrNull(string relativePath) {
			string path = Path.Combine(Program.ResourcesPath, relativePath);

			try {
				return File.ReadAllText(path, Encoding.UTF8);
			} catch (Exception e) {
				App.ErrorHandler.Log("Error reading file: " + path);
				App.ErrorHandler.Log(e.ToString());
				return null;
			}
		}
	}
}
