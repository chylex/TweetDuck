using System;
using System.IO;
using System.Reflection;
using TweetLib.Api;
using TweetLib.Core.Systems.Api;

namespace TweetLib.Core.Features.Extensions {
	public static class ExtensionLoader {
		public static void LoadAllInFolder(string extensionFolder) {
			if (!Directory.Exists(extensionFolder)) {
				return;
			}

			try {
				foreach (string file in Directory.EnumerateFiles(extensionFolder, "*.dll", SearchOption.TopDirectoryOnly)) {
					try {
						Assembly assembly = Assembly.LoadFile(file);
						foreach (Type type in assembly.GetTypes()) {
							if (typeof(TweetDuckExtension).IsAssignableFrom(type) && Activator.CreateInstance(type) is TweetDuckExtension extension) {
								EnableExtension(extension);
							}
						}
					} catch (Exception e) {
						App.ErrorHandler.HandleException("Extension Error", "Could not load extension: " + Path.GetFileNameWithoutExtension(file), true, e);
					}
				}
			} catch (DirectoryNotFoundException) {
				// ignore
			} catch (Exception e) {
				App.ErrorHandler.HandleException("Extension Error", "Could not load extensions.", true, e);
			}
		}

		private static void EnableExtension(TweetDuckExtension extension) {
			ApiImplementation apiImplementation = App.Api;
			apiImplementation.CurrentExtension = extension;
			extension.Enable(apiImplementation);
			apiImplementation.CurrentExtension = null;
		}
	}
}
