namespace TweetLib.Browser.Interfaces {
	public interface IScriptExecutor {
		void RunFunction(string name, params object[] args);
		void RunScript(string identifier, string script);
	}
}
