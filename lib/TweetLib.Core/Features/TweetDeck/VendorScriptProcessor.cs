using System.Text.RegularExpressions;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Core.Features.TweetDeck {
	sealed class VendorScriptProcessor : ResponseProcessorUtf8 {
		public static VendorScriptProcessor Instance { get; } = new ();

		private static readonly Regex RegexRestoreJQuery = new (@"(\w+)\.fn=\1\.prototype", RegexOptions.Compiled);

		private VendorScriptProcessor() {}

		protected override string Process(string response) {
			return RegexRestoreJQuery.Replace(response, "window.$$=$1;$&", 1);
		}
	}
}
