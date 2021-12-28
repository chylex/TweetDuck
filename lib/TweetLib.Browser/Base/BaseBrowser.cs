using System;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Browser.Base {
	public class BaseBrowser<T> : IDisposable where T : BaseBrowser<T> {
		public IScriptExecutor ScriptExecutor { get; }

		protected readonly IBrowserComponent browserComponent;

		protected BaseBrowser(IBrowserComponent browserComponent, Func<T, BrowserSetup> setup) {
			this.browserComponent = browserComponent;
			this.browserComponent.Setup(setup((T) this));

			this.ScriptExecutor = browserComponent;
		}

		public virtual void Dispose() {}
	}
}
