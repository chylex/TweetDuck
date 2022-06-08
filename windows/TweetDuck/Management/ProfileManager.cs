using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CefSharp;
using TweetDuck.Dialogs;
using TweetLib.Core;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Enums;
using TweetLib.Utils.IO;

namespace TweetDuck.Management {
	sealed class ProfileManager {
		private const string AuthCookieUrl = "https://twitter.com";
		private const string AuthCookieName = "auth_token";
		private const string AuthCookieDomain = ".twitter.com";
		private const string AuthCookiePath = "/";

		[Flags]
		public enum Items {
			None = 0,
			UserConfig = 1,
			SystemConfig = 2,
			Session = 4,
			PluginData = 8
		}

		private readonly string file;
		private readonly PluginManager plugins;

		public ProfileManager(string file, PluginManager plugins) {
			this.file = file;
			this.plugins = plugins;
		}

		public bool Export(Items items) {
			try {
				using CombinedFileStream stream = new CombinedFileStream(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None));

				if (items.HasFlag(Items.UserConfig)) {
					stream.WriteFile("config", App.ConfigManager.UserPath);
				}

				if (items.HasFlag(Items.SystemConfig)) {
					stream.WriteFile("system", App.ConfigManager.SystemPath);
				}

				if (items.HasFlag(Items.PluginData)) {
					stream.WriteFile("plugin.config", App.ConfigManager.PluginsPath);

					foreach (Plugin plugin in plugins.Plugins) {
						foreach (PathInfo path in EnumerateFilesRelative(plugin.GetPluginFolder(PluginFolder.Data))) {
							try {
								stream.WriteFile(new string[] { "plugin.data", plugin.Identifier, path.Relative }, path.Full);
							} catch (ArgumentOutOfRangeException e) {
								FormMessage.Warning("Export Profile", "Could not include a plugin file in the export. " + e.Message, FormMessage.OK);
							}
						}
					}
				}

				if (items.HasFlag(Items.Session)) {
					string? authToken = ReadAuthCookie();

					if (authToken != null) {
						stream.WriteString("cookie.auth", authToken);
					}
					else {
						FormMessage.Warning("Export Profile", "Could not find any login session.", FormMessage.OK);
					}
				}

				stream.Flush();
				return true;
			} catch (Exception e) {
				App.ErrorHandler.HandleException("Profile Export Error", "An exception happened while exporting TweetDuck profile.", true, e);
				return false;
			}
		}

		public Items FindImportItems() {
			Items items = Items.None;

			try {
				using CombinedFileStream stream = new CombinedFileStream(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None));

				while (stream.SkipFile() is {} key) {
					switch (key) {
						case "config":
							items |= Items.UserConfig;
							break;

						case "system":
							items |= Items.SystemConfig;
							break;

						case "plugin.config":
						case "plugin.data":
							items |= Items.PluginData;
							break;

						case "cookies":
						case "localprefs":
						case "cookie.auth":
							items |= Items.Session;
							break;
					}
				}
			} catch (Exception) {
				items = Items.None;
			}

			return items;
		}

		public bool Import(Items items) {
			try {
				var missingPlugins = new HashSet<string>();
				bool oldCookies = false;

				using (CombinedFileStream stream = new CombinedFileStream(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))) {
					while (stream.ReadFile() is {} entry) {
						switch (entry.KeyName) {
							case "config":
								if (items.HasFlag(Items.UserConfig)) {
									entry.WriteToFile(App.ConfigManager.UserPath);
								}

								break;

							case "system":
								if (items.HasFlag(Items.SystemConfig)) {
									entry.WriteToFile(App.ConfigManager.SystemPath);
								}

								break;

							case "plugin.config":
								if (items.HasFlag(Items.PluginData)) {
									entry.WriteToFile(App.ConfigManager.PluginsPath);
								}

								break;

							case "plugin.data":
								if (items.HasFlag(Items.PluginData)) {
									string[] value = entry.KeyValue;

									entry.WriteToFile(Path.Combine(plugins.PluginDataFolder, value[0], value[1]), true);

									if (!plugins.Plugins.Any(plugin => plugin.Identifier.Equals(value[0]))) {
										missingPlugins.Add(value[0]);
									}
								}

								break;

							case "cookies":
							case "localprefs":
								if (items.HasFlag(Items.Session)) {
									oldCookies = true;
								}

								break;

							case "cookie.auth":
								if (items.HasFlag(Items.Session)) {
									using ICookieManager cookies = Cef.GetGlobalCookieManager();

									var _ = cookies.SetCookieAsync(AuthCookieUrl, new Cookie {
										Name = AuthCookieName,
										Domain = AuthCookieDomain,
										Path = AuthCookiePath,
										Value = Encoding.UTF8.GetString(entry.Contents),
										Expires = DateTime.Now.Add(TimeSpan.FromDays(365 * 5)),
										HttpOnly = true,
										Secure = true
									}).ContinueWith(t => {
										// ReSharper disable once AccessToDisposedClosure
										// ReSharper disable once ConvertToLambdaExpression
										return cookies.FlushStoreAsync();
									}).Result;
								}

								break;
						}
					}
				}

				if (items.HasFlag(Items.Session) && oldCookies) {
					FormMessage.Error("Profile Import Error", "Cannot import login session from an older version of TweetDuck.", FormMessage.OK);
					return false;
				}

				if (missingPlugins.Count > 0) {
					FormMessage.Information("Profile Import", "Detected missing plugins when importing plugin data:\n" + string.Join("\n", missingPlugins), FormMessage.OK);
				}

				return true;
			} catch (Exception e) {
				App.ErrorHandler.HandleException("Profile Import", "An exception happened while importing TweetDuck profile.", true, e);
				return false;
			}
		}

		private static IEnumerable<PathInfo> EnumerateFilesRelative(string root) {
			if (Directory.Exists(root)) {
				int rootLength = root.Length;
				return Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories).Select(fullPath => new PathInfo(fullPath, rootLength));
			}
			else {
				return Enumerable.Empty<PathInfo>();
			}
		}

		private sealed class PathInfo {
			public string Full { get; }
			public string Relative { get; }

			public PathInfo(string fullPath, int rootLength) {
				this.Full = fullPath;
				this.Relative = fullPath[rootLength..].TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); // strip leading separator character
			}
		}

		private static string? ReadAuthCookie() {
			using var cookieManager = Cef.GetGlobalCookieManager();

			foreach (var cookie in cookieManager.VisitUrlCookiesAsync(AuthCookieUrl, true).Result) {
				if (cookie.Name == AuthCookieName && cookie.Domain == AuthCookieDomain && cookie.Path == AuthCookiePath && cookie.HttpOnly && cookie.Secure) {
					return cookie.Value;
				}
			}

			return null;
		}

		public static void DeleteAuthCookie() {
			using var cookieManager = Cef.GetGlobalCookieManager();
			var _ = cookieManager.DeleteCookiesAsync(AuthCookieUrl, "auth_token").Result;
		}
	}
}
