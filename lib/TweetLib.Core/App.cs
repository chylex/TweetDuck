using System;
using System.IO;
using System.Linq;
using TweetLib.Core.Application;
using TweetLib.Core.Features;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Resources;
using TweetLib.Core.Systems.Configuration;
using TweetLib.Core.Systems.Logging;
using TweetLib.Utils.Static;
using Version = TweetDuck.Version;

namespace TweetLib.Core {
	public static class App {
		private static IAppSetup Setup { get; } = Validate(Builder.Setup, nameof(Builder.Setup));

		public static readonly string ProgramPath = AppDomain.CurrentDomain.BaseDirectory;
		public static readonly bool IsPortable = Setup.IsPortable;

		public static readonly string ResourcesPath = Path.Combine(ProgramPath, "resources");
		public static readonly string PluginPath    = Path.Combine(ProgramPath, "plugins");
		public static readonly string GuidePath     = Path.Combine(ProgramPath, "guide");

		public static readonly string StoragePath = IsPortable ? Path.Combine(ProgramPath, "portable", "storage") : GetDataFolder();

		public static Logger Logger               { get; } = new (Path.Combine(StoragePath, "TD_Log.txt"), Setup.IsDebugLogging);
		public static ConfigManager ConfigManager { get; } = Setup.CreateConfigManager(StoragePath);

		public static IAppErrorHandler ErrorHandler     { get; } = Validate(Builder.ErrorHandler, nameof(Builder.ErrorHandler));
		public static IAppSystemHandler SystemHandler   { get; } = Validate(Builder.SystemHandler, nameof(Builder.SystemHandler));
		public static IAppMessageDialogs MessageDialogs { get; } = Validate(Builder.MessageDialogs, nameof(Builder.MessageDialogs));
		public static IAppFileDialogs? FileDialogs      { get; } = Builder.FileDialogs;

		internal static IAppUserConfiguration UserConfiguration => ConfigManager.User;
		internal static IAppSystemConfiguration SystemConfiguration => ConfigManager.System;

		private static string GetDataFolder() {
			string? custom = Setup.CustomDataFolder;

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

		internal static void Launch() {
			if (!FileUtils.CheckFolderWritePermission(StoragePath)) {
				throw new AppException("Permission Error", "TweetDuck does not have write permissions to the storage folder: " + StoragePath);
			}

			if (!Setup.TryLockDataFolder(Path.Combine(StoragePath, ".lock"))) {
				return;
			}

			ConfigManager.LoadAll();
			Setup.BeforeLaunch();

			var resourceRewriteRules = Setup.ResourceRewriteRules;
			if (resourceRewriteRules != null) {
				try {
					BaseResourceRequestHandler.LoadResourceRewriteRules(resourceRewriteRules);
				} catch (Exception e) {
					throw new AppException("Resource Freeze", "Error parsing resource rewrite rules: " + e.Message);
				}
			}

			WebUtils.DefaultUserAgent = Lib.BrandName + " " + Version.Tag;

			if (SystemConfiguration.UseSystemProxyForAllConnections) {
				WebUtils.EnableSystemProxy();
			}

			var resourceCache = new ResourceCache();
			var pluginManager = new PluginManager(ConfigManager.Plugins, PluginPath, Path.Combine(StoragePath, "TD_Plugins"));

			Setup.Launch(resourceCache, pluginManager);
		}

		public static void Close() {
			ConfigManager.SaveAll();
		}

		// Setup

		private static AppBuilder Builder => AppBuilder.Instance ?? throw new InvalidOperationException("App is initializing too early");

		private static bool isInitialized = false;

		internal static void Initialize() {
			if (isInitialized) {
				throw new InvalidOperationException("App is already initialized");
			}

			isInitialized = true;
		}

		private static T Validate<T>(T? obj, string name) where T : class {
			return obj ?? throw new InvalidOperationException("Missing property " + name + " on the provided App");
		}
	}

	public sealed class AppBuilder {
		public IAppSetup? Setup { get; set; }
		public IAppErrorHandler? ErrorHandler { get; set; }
		public IAppSystemHandler? SystemHandler { get; set; }
		public IAppMessageDialogs? MessageDialogs { get; set; }
		public IAppFileDialogs? FileDialogs { get; set; }

		internal static AppBuilder? Instance { get; private set; }

		internal Lib.AppLauncher Build() {
			Instance = this;
			App.Initialize();
			Instance = null;
			return App.Launch;
		}
	}
}
