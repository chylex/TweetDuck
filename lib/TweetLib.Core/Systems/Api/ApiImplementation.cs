using System;
using System.Collections.Generic;
using TweetLib.Api;

namespace TweetLib.Core.Systems.Api {
	public class ApiImplementation : ITweetDuckApi {
		public TweetDuckExtension? CurrentExtension { get; internal set; }

		private readonly Dictionary<Type, ITweetDuckService> services = new Dictionary<Type, ITweetDuckService>();

		internal ApiImplementation() {}

		public void RegisterService<T>(T service) where T : class, ITweetDuckService {
			if (!typeof(T).IsInterface) {
				throw new ArgumentException("Api service implementation must be registered with its interface type.");
			}

			services.Add(typeof(T), service);
		}

		public T? FindService<T>() where T : class, ITweetDuckService {
			return services.TryGetValue(typeof(T), out ITweetDuckService? service) ? service as T : null;
		}
	}
}
