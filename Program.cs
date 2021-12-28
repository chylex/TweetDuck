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
using TweetDuck.Dialogs;
using TweetDuck.Management;
using TweetDuck.Updates;
using TweetDuck.Utils;
using TweetLib.Core;
using TweetLib.Core.Application;
using TweetLib.Core.Features;
using TweetLib.Core.Features.Chromium;
using TweetLib.Core.Features.Plugins;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Resources;
using TweetLib.Utils.Collections;
using TweetLib.Utils.Static;
using Win = System.Windows.Forms;

namespace TweetDuck {
	static class Program {
		public const string BrandName = Lib.BrandName;
		public const string VersionTag = Version.Tag;

		public const string Website = "https://tweetduck.chylex.com";

		private const string PluginDataFolder = "TD_Plugins";
		private const string InstallerFolder = "TD_Updates";
		private const string CefDataFolder = "TD_Chromium";

		private const string ProgramLogFile = "TD_Log.txt";
		private const string ConsoleLogFile = "TD_Console.txt";

		public static string ExecutablePath => Win.Application.ExecutablePath;

		public static uint WindowRestoreMessage;

		private static LockManager lockManager;
		private static Reporter errorReporter;
		private static bool hasCleanedUp;

		public static ConfigManager Config { get; private set; }

		internal static void SetupWinForms() {
			Win.Application.EnableVisualStyles();
			Win.Application.SetCompatibleTextRenderingDefault(false);
		}

		[STAThread]
		private static void Main() {
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			SetupWinForms();
			Cef.EnableHighDPISupport();

			var startup = new AppStartup {
				CustomDataFolder = Arguments.GetValue(Arguments.ArgDataFolder)
			};

			var reporter = new Reporter();
			var userConfig = new UserConfig();

			Lib.Initialize(new AppBuilder {
				Startup = startup,
				Logger = new Logger(ProgramLogFile),
				ErrorHandler = reporter,
				SystemHandler = new SystemHandler(),
				DialogHandler = new DialogHandler(),
				UserConfiguration = userConfig
			});

			LaunchApp(reporter, userConfig);
		}

		private static void LaunchApp(Reporter reporter, UserConfig userConfig) {
			App.Launch();

			errorReporter = reporter;
			string storagePath = App.StoragePath;

			Config = new ConfigManager(userConfig, new ConfigManager.Paths {
				UserConfig   = Path.Combine(storagePath, "TD_UserConfig.cfg"),
				SystemConfig = Path.Combine(storagePath, "TD_SystemConfig.cfg"),
				PluginConfig = Path.Combine(storagePath, "TD_PluginConfig.cfg")
			});

			lockManager = new LockManager(Path.Combine(storagePath, ".lock"));
			WindowRestoreMessage = NativeMethods.RegisterWindowMessage("TweetDuckRestore");

			if (!lockManager.Lock(Arguments.HasFlag(Arguments.ArgRestart))) {
				return;
			}

			Config.LoadAll();

			if (Arguments.HasFlag(Arguments.ArgImportCookies)) {
				ProfileManager.ImportCookies();
			}
			else if (Arguments.HasFlag(Arguments.ArgDeleteCookies)) {
				ProfileManager.DeleteCookies();
			}

			var installerFolderPath = Path.Combine(storagePath, InstallerFolder);

			if (Arguments.HasFlag(Arguments.ArgUpdated)) {
				WindowsUtils.TryDeleteFolderWhenAble(installerFolderPath, 8000);
				WindowsUtils.TryDeleteFolderWhenAble(Path.Combine(storagePath, "Service Worker"), 4000);
				BrowserCache.TryClearNow();
			}

			try {
				BaseResourceRequestHandler.LoadResourceRewriteRules(Arguments.GetValue(Arguments.ArgFreeze));
			} catch (Exception e) {
				FormMessage.Error("Resource Freeze", "Error parsing resource rewrite rules: " + e.Message, FormMessage.OK);
				return;
			}

			WebUtils.DefaultUserAgent = BrowserUtils.UserAgentVanilla;

			if (Config.User.UseSystemProxyForAllConnections) {
				WebUtils.EnableSystemProxy();
			}

			BrowserCache.RefreshTimer();

			CefSharpSettings.WcfEnabled = false;

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

			var resourceProvider = new CachingResourceProvider<IResourceHandler>(new ResourceProvider());
			var pluginManager = new PluginManager(Config.Plugins, Path.Combine(storagePath, PluginDataFolder));

			CefSchemeHandlerFactory.Register(settings, new TweetDuckSchemeHandler<IResourceHandler>(resourceProvider));
			CefSchemeHandlerFactory.Register(settings, new PluginSchemeHandler<IResourceHandler>(resourceProvider, pluginManager));

			CefUtils.ParseCommandLineArguments(Config.User.CustomCefArgs).ToDictionary(settings.CefCommandLineArgs);
			BrowserUtils.SetupCefArgs(settings.CefCommandLineArgs);

			Cef.Initialize(settings, false, new BrowserProcessHandler());

			Win.Application.ApplicationExit += (sender, args) => ExitCleanup();
			FormBrowser mainForm = new FormBrowser(resourceProvider, pluginManager, new UpdateCheckClient(installerFolderPath));
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

			Config.SaveAll();

			Cef.Shutdown();
			BrowserCache.Exit();

			lockManager.Unlock();
			hasCleanedUp = true;
		}
	}
}
