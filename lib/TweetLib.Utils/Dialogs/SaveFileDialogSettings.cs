using System.Collections.Generic;

namespace TweetLib.Utils.Dialogs {
	public sealed class SaveFileDialogSettings {
		public string DialogTitle { get; set; } = "Save File";
		public bool OverwritePrompt { get; set; } = true;
		public string? FileName { get; set; }
		public IReadOnlyList<FileDialogFilter>? Filters { get; set; }
	}
}
