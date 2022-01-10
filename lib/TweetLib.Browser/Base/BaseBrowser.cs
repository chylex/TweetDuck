using System;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Browser.Base {
	public class BaseBrowser<T> : IDisposable where T : BaseBrowser<T> {
		protected readonly IBrowserComponent browserComponent;

		protected BaseBrowser(IBrowserComponent browserComponent, Func<T, BrowserSetup> setup) {
			this.browserComponent = browserComponent;
			this.browserComponent.Setup(setup((T) this));
		}

		public virtual void Dispose() {}
	}
}
