namespace TweetLib.Core.Application {
	public interface IAppSystemHandler {
		void OpenAssociatedProgram(string path);
		void OpenBrowser(string? url);
		void OpenFileExplorer(string path);
		void CopyImageFromFile(string path);
		void CopyText(string text);
		void SearchText(string text);
	}
}
