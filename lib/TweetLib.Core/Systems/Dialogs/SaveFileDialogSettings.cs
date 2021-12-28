using System.Collections.Generic;

namespace TweetLib.Core.Systems.Dialogs {
	public sealed class SaveFileDialogSettings {
		public string DialogTitle { get; internal set; } = "Save File";
		public bool OverwritePrompt { get; internal set; } = true;
		public string? FileName { get; internal set; }
		public IReadOnlyList<FileDialogFilter>? Filters { get; internal set; }
	}
}
