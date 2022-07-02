using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TweetLib.Core;

namespace TweetDuck.Management {
	static class ClipboardManager {
		private static readonly Lazy<Regex> RegexStripHtmlStyles = new (static () => new Regex(@"\s?(?:style|class)="".*?"""), false);
		private static readonly Lazy<Regex> RegexOffsetClipboardHtml = new (static () => new Regex(@"(?<=EndHTML:|EndFragment:)(\d+)"), false);

		public static void SetText(string text, TextDataFormat format) {
			if (string.IsNullOrEmpty(text)) {
				return;
			}

			DataObject obj = new DataObject();
			obj.SetText(text, format);
			SetClipboardData(obj);
		}

		public static void SetImage(Image image) {
			DataObject obj = new DataObject();
			obj.SetImage(image);
			SetClipboardData(obj);
		}

		private static void SetClipboardData(DataObject obj) {
			try {
				Clipboard.SetDataObject(obj);
			} catch (ExternalException e) {
				App.ErrorHandler.HandleException("Clipboard Error", "TweetDuck could not access the clipboard as it is currently used by another process.", true, e);
			}
		}

		public static void StripHtmlStyles() {
			if (!Clipboard.ContainsText(TextDataFormat.Html) || !Clipboard.ContainsText(TextDataFormat.UnicodeText)) {
				return;
			}

			string originalText = Clipboard.GetText(TextDataFormat.UnicodeText);
			string originalHtml = Clipboard.GetText(TextDataFormat.Html);

			string updatedHtml = RegexStripHtmlStyles.Value.Replace(originalHtml, string.Empty);

			int removed = originalHtml.Length - updatedHtml.Length;
			updatedHtml = RegexOffsetClipboardHtml.Value.Replace(updatedHtml, match => (int.Parse(match.Value) - removed).ToString().PadLeft(match.Value.Length, '0'));

			DataObject obj = new DataObject();
			obj.SetText(originalText, TextDataFormat.UnicodeText);
			obj.SetText(updatedHtml, TextDataFormat.Html);
			SetClipboardData(obj);
		}
	}
}
