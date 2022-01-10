using System.Globalization;
using System.Reflection;
using System.Threading;

[assembly: AssemblyTitle("TweetDuck Core Library")]
[assembly: AssemblyDescription("TweetDuck Core Library")]
[assembly: AssemblyProduct("TweetCore.Lib")]

namespace TweetLib.Core {
	public static class Lib {
		public const string BrandName = "TweetDuck";
		public const string IssueTrackerUrl = "https://github.com/chylex/TweetDuck/issues";

		public static CultureInfo Culture { get; } = CultureInfo.CurrentCulture;

		public delegate void AppLauncher();

		public static AppLauncher Initialize(AppBuilder app) {
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

			#if DEBUG
			CultureInfo.DefaultThreadCurrentUICulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us"); // force english exceptions
			#endif

			return app.Build();
		}
	}
}
