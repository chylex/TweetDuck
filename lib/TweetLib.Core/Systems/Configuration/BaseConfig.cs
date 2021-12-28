using System;
using System.Collections.Generic;

namespace TweetLib.Core.Systems.Configuration {
	public abstract class BaseConfig {
		internal T ConstructWithDefaults<T>() where T : BaseConfig {
			return (T) ConstructWithDefaults();
		}

		protected abstract BaseConfig ConstructWithDefaults();

		protected void UpdatePropertyWithEvent<T>(ref T field, T value, EventHandler? eventHandler) {
			if (!EqualityComparer<T>.Default.Equals(field, value)) {
				field = value;
				eventHandler?.Invoke(this, EventArgs.Empty);
			}
		}

		protected void UpdatePropertyWithCallback<T>(ref T field, T value, Action action) {
			if (!EqualityComparer<T>.Default.Equals(field, value)) {
				field = value;
				action();
			}
		}
	}
}
