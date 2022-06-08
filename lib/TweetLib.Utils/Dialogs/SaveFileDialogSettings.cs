using System.Collections.Generic;

namespace TweetLib.Utils.Dialogs {
	public sealed class SaveFileDialogSettings {
		public string DialogTitle { get; init; } = "Save File";
		public bool OverwritePrompt { get; init; } = true;
		public string? FileName { get; init; }
		public IReadOnlyList<FileDialogFilter>? Filters { get; init; }
	}
}
