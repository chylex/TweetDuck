#nullable enable
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace TweetDuck.Video {
	static class Program {
		// referenced in VideoPlayer
		// set by task manager -- public const int "CodeProcessKilled" = 1;
		public const int CodeInvalidArgs = 2;
		public const int CodeLaunchFail = 3;
		public const int CodeMediaError = 4;
		public const int CodeOwnerGone = 5;
		public const int CodeUserRequested = 6;

		[STAThread]
		private static int Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			NativeMethods.SetProcessDPIAware();

			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

			IntPtr ownerHandle;
			int ownerDpi;
			int defaultVolume;
			string videoUrl;
			string pipeToken;

			try {
				ownerHandle = new IntPtr(int.Parse(args[0], NumberStyles.Integer));
				ownerDpi = int.Parse(args[1], NumberStyles.Integer);
				defaultVolume = int.Parse(args[2], NumberStyles.Integer);
				videoUrl = new Uri(args[3], UriKind.Absolute).AbsoluteUri;
				pipeToken = args[4];
			} catch {
				return CodeInvalidArgs;
			}

			try {
				Application.Run(new FormPlayer(ownerHandle, ownerDpi, defaultVolume, videoUrl, pipeToken));
			} catch (Exception e) {
				Console.Out.WriteLine(e);
				return CodeLaunchFail;
			}

			return 0;
		}
	}
}
