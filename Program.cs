using System;
using System.Diagnostics;
using System.IO;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Application;
using TweetDuck.Browser;
using TweetDuck.Browser.Adapters;
using TweetDuck.Browser.Handling;
using TweetDuck.Configuration;
using TweetDuck.Management;
using TweetDuck.Updates;
using TweetDuck.Utils;
using TweetLib.Core;
using TweetLib.Core.Application;
using TweetLib.Core.Features.Chromium;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.Plugins.Config;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Resources;
using TweetLib.Core.Systems.Configuration;
using TweetLib.Utils.Collections;
using Win = System.Windows.Forms;

namespace TweetDuck {
	static class Program {
		public const string BrandName = Lib.BrandName;
		public const string VersionTag = Version.Tag;

		public const string Website = "https://tweetduck.chylex.com";

		private const string InstallerFolder = "TD_Updates";
		private const string CefDataFolder = "TD_Chromium";
		private const string ConsoleLogFile = "TD_Console.txt";

		public static string ExecutablePath => Win.Application.ExecutablePath;

		private static Reporter errorReporter;
		private static LockManager lockManager;
		private static bool hasCleanedUp;

		public static ConfigObjects<UserConfig, SystemConfig> Config { get; private set; }

		internal static void SetupWinForms() {
			Win.Application.EnableVisualStyles();
			Win.Application.SetCompatibleTextRenderingDefault(false);
		}

		[STAThread]
		private static void Main() {
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			SetupWinForms();
			Cef.EnableHighDPISupport();

			var reporter = new Reporter();

			Config = new ConfigObjects<UserConfig, SystemConfig>(
				new UserConfig(),
				new SystemConfig(),
				new PluginConfig(new string[] {
					"official/clear-columns",
					"official/reply-account"
				})
			);

			Lib.AppLauncher launch = Lib.Initialize(new AppBuilder {
				Setup = new Setup(),
				ErrorHandler = reporter,
				SystemHandler = new SystemHandler(),
				MessageDialogs = new MessageDialogs(),
				FileDialogs = new FileDialogs(),
			});

			errorReporter = reporter;
			launch();
		}

		private sealed class Setup : IAppSetup {
			public bool IsPortable => File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "makeportable"));
			public bool IsDebugLogging => Arguments.HasFlag(Arguments.ArgLogging);
			public string CustomDataFolder => Arguments.GetValue(Arguments.ArgDataFolder);
			public string ResourceRewriteRules => Arguments.GetValue(Arguments.ArgFreeze);

			public ConfigManager CreateConfigManager(string storagePath) {
				return new ConfigManager<UserConfig, SystemConfig>(storagePath, Config);
			}

			public bool TryLockDataFolder(string lockFile) {
				lockManager = new LockManager(lockFile);
				return lockManager.Lock(Arguments.HasFlag(Arguments.ArgRestart));
			}

			public void BeforeLaunch() {
				if (Arguments.HasFlag(Arguments.ArgImportCookies)) {
					ProfileManager.ImportCookies();
				}
				else if (Arguments.HasFlag(Arguments.ArgDeleteCookies)) {
					ProfileManager.DeleteCookies();
				}

				if (Arguments.HasFlag(Arguments.ArgUpdated)) {
					WindowsUtils.TryDeleteFolderWhenAble(Path.Combine(App.StoragePath, InstallerFolder), 8000);
					WindowsUtils.TryDeleteFolderWhenAble(Path.Combine(App.StoragePath, "Service Worker"), 4000);
					BrowserCache.TryClearNow();
				}
			}

			public void Launch(ResourceCache resourceCache, PluginManager pluginManager) {
				string storagePath = App.StoragePath;

				BrowserCache.RefreshTimer();

				CefSharpSettings.WcfEnabled = false;
				CefSharpSettings.SubprocessExitIfParentProcessClosed = false;

				CefSettings settings = new CefSettings {
					UserAgent = BrowserUtils.UserAgentChrome,
					BrowserSubprocessPath = Path.Combine(App.ProgramPath, BrandName + ".Browser.exe"),
					CachePath = storagePath,
					UserDataPath = Path.Combine(storagePath, CefDataFolder),
					LogFile = Path.Combine(storagePath, ConsoleLogFile),
					#if !DEBUG
					LogSeverity = Arguments.HasFlag(Arguments.ArgLogging) ? LogSeverity.Info : LogSeverity.Disable
					#endif
				};

				CefSchemeHandlerFactory.Register(settings, new TweetDuckSchemeHandler(resourceCache));
				CefSchemeHandlerFactory.Register(settings, new PluginSchemeHandler(resourceCache, pluginManager));

				CefUtils.ParseCommandLineArguments(Config.User.CustomCefArgs).ToDictionary(settings.CefCommandLineArgs);
				BrowserUtils.SetupCefArgs(settings.CefCommandLineArgs);

				Cef.Initialize(settings, false, new BrowserProcessHandler());

				Win.Application.ApplicationExit += (sender, args) => ExitCleanup();
				var updateCheckClient = new UpdateCheckClient(Path.Combine(storagePath, InstallerFolder));
				var mainForm = new FormBrowser(resourceCache, pluginManager, updateCheckClient, lockManager.WindowRestoreMessage);
				Win.Application.Run(mainForm);

				if (mainForm.UpdateInstaller != null) {
					ExitCleanup();

					if (mainForm.UpdateInstaller.Launch()) {
						Win.Application.Exit();
					}
					else {
						RestartWithArgsInternal(Arguments.GetCurrentClean());
					}
				}
			}
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
			if (e.ExceptionObject is Exception ex) {
				AppException appEx = ex.GetBaseException() as AppException;
				string title = appEx?.Title ?? "TweetDuck Has Failed :(";
				string message = appEx?.Message ?? "An unhandled exception has occurred: " + ex.Message;

				if (errorReporter == null) {
					Debug.WriteLine(ex);
					Reporter.HandleEarlyFailure(title, message);
				}
				else {
					errorReporter.HandleException(title, message, false, ex);
				}
			}
		}

		public static void Restart(params string[] extraArgs) {
			CommandLineArgs args = Arguments.GetCurrentClean();
			CommandLineArgs.ReadStringArray('-', extraArgs, args);
			RestartWithArgs(args);
		}

		public static void RestartWithArgs(CommandLineArgs args) {
			FormBrowser browserForm = FormManager.TryFind<FormBrowser>();

			if (browserForm != null) {
				browserForm.ForceClose();

				ExitCleanup();
				RestartWithArgsInternal(args);
			}
		}

		private static void RestartWithArgsInternal(CommandLineArgs args) {
			args.AddFlag(Arguments.ArgRestart);
			Process.Start(ExecutablePath, args.ToString());
			Win.Application.Exit();
		}

		private static void ExitCleanup() {
			if (hasCleanedUp) {
				return;
			}

			App.Close();

			Cef.Shutdown();
			BrowserCache.Exit();

			lockManager.Unlock();
			hasCleanedUp = true;
		}
	}
}
