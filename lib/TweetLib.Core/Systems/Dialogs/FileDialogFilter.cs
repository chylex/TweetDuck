using System.Collections.Generic;

namespace TweetLib.Core.Systems.Dialogs {
	public sealed class FileDialogFilter {
		public string Name { get; }
		public IReadOnlyList<string> Extensions { get; }

		public FileDialogFilter(string name, params string[] extensions) {
			Name = name;
			Extensions = extensions;
		}
	}
}
