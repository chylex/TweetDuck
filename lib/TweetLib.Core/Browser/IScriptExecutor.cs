namespace TweetLib.Core.Browser {
	public interface IScriptExecutor {
		void RunFunction(string name, params object[] args);
		void RunScript(string identifier, string script);
		bool RunFile(string file);
		void RunBootstrap(string moduleNamespace);
	}
}
