using System.Globalization;
using System.Reflection;
using System.Threading;

[assembly: AssemblyTitle("TweetDuck Core Library")]
[assembly: AssemblyDescription("TweetDuck Core Library")]
[assembly: AssemblyProduct("TweetCore.Lib")]

namespace TweetLib.Core {
	public static class Lib {
		public const string BrandName = "TweetDuck";

		#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		public static CultureInfo Culture { get; private set; }
		#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		public static void Initialize(App.Builder app) {
			Culture = CultureInfo.CurrentCulture;

			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

			#if DEBUG
			CultureInfo.DefaultThreadCurrentUICulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us"); // force english exceptions
			#endif

			app.Initialize();
		}
	}
}
