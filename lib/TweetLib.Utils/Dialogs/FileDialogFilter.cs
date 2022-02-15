using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TweetLib.Utils.Dialogs {
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public sealed class FileDialogFilter {
		public string Name { get; }
		public IReadOnlyList<string> Extensions { get; }

		public string FullName => FullNameAndPattern.Item1;
		public string Pattern => FullNameAndPattern.Item2;

		private (string, string) FullNameAndPattern {
			get {
				if (Extensions.Count == 0) {
					return (Name, "*.*");
				}
				else {
					string pattern = string.Join(";", Extensions.Select(static ext => "*" + ext));
					string fullName = Name + " (" + pattern + ")";
					return (fullName, pattern);
				}
			}
		}

		public FileDialogFilter(string name, params string[] extensions) {
			Name = name;
			Extensions = extensions;
		}

		public string JoinFullNameAndPattern(string separator) {
			var (fullName, pattern) = FullNameAndPattern;
			return fullName + separator + pattern;
		}
	}
}
