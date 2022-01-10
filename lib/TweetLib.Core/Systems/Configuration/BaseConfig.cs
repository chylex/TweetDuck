using System;
using System.Collections.Generic;

namespace TweetLib.Core.Systems.Configuration {
	public abstract class BaseConfig<T> : IConfigObject<T> where T : BaseConfig<T> {
		public abstract T ConstructWithDefaults();

		protected void UpdatePropertyWithEvent<V>(ref V field, V value, EventHandler? eventHandler) {
			if (!EqualityComparer<V>.Default.Equals(field, value)) {
				field = value;
				eventHandler?.Invoke(this, EventArgs.Empty);
			}
		}

		protected void UpdatePropertyWithCallback<V>(ref V field, V value, Action action) {
			if (!EqualityComparer<V>.Default.Equals(field, value)) {
				field = value;
				action();
			}
		}
	}
}
