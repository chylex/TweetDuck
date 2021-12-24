namespace TweetLib.Core.Browser {
	public interface IScriptExecutor {
		void RunFunction(string name, params object[] args);
		void RunScript(string identifier, string script);
		void RunBootstrap(string moduleNamespace);
	}
}
