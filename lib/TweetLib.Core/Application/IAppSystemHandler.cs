namespace TweetLib.Core.Application {
	public interface IAppSystemHandler {
		void OpenBrowser(string? url);
		void OpenFileExplorer(string path);

		OpenAssociatedProgramFunc? OpenAssociatedProgram { get; }
		CopyImageFromFileFunc? CopyImageFromFile { get; }
		CopyTextFunc? CopyText { get; }
		SearchTextFunc? SearchText { get; }

		public delegate void OpenAssociatedProgramFunc(string path);

		public delegate void CopyImageFromFileFunc(string path);

		public delegate void CopyTextFunc(string text);

		public delegate void SearchTextFunc(string text);
	}
}
