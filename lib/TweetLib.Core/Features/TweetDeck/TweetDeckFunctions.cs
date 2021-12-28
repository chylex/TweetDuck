using System;
using System.Text;
using System.Text.RegularExpressions;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Core.Features.TweetDeck {
	public sealed class TweetDeckFunctions {
		private readonly IScriptExecutor executor;

		internal TweetDeckFunctions(IScriptExecutor executor) {
			this.executor = executor;
		}

		public void ReinjectCustomCSS(string? css) {
			executor.RunFunction("TDGF_reinjectCustomCSS", css == null ? string.Empty : Regex.Replace(css, "\r?\n", " "));
		}

		public void OnMouseClickExtra(int button) {
			executor.RunFunction("TDGF_onMouseClickExtra", button);
		}

		public void ShowTweetDetail(string columnId, string chirpId, string fallbackUrl) {
			executor.RunFunction("TDGF_showTweetDetail", columnId, chirpId, fallbackUrl);
		}

		public void AddSearchColumn(string query) {
			executor.RunFunction("TDGF_performSearch", query);
		}

		public void TriggerTweetScreenshot(string columnId, string chirpId) {
			executor.RunFunction("TDGF_triggerScreenshot", columnId, chirpId);
		}

		public void ReloadColumns() {
			executor.RunFunction("TDGF_reloadColumns");
		}

		public void PlaySoundNotification() {
			executor.RunFunction("TDGF_playSoundNotification");
		}

		public void ApplyROT13() {
			executor.RunFunction("TDGF_applyROT13");
		}

		public void SetSoundNotificationData(bool isCustom, int volume) {
			executor.RunFunction("TDGF_setSoundNotificationData", isCustom, volume);
		}

		public void ShowUpdateNotification(string versionTag, string releaseNotes) {
			executor.RunFunction("TDUF_displayNotification", versionTag, Convert.ToBase64String(Encoding.GetEncoding("iso-8859-1").GetBytes(releaseNotes)));
		}
	}
}
