using System;
using System.Diagnostics.CodeAnalysis;
using TweetLib.Core.Application;

namespace TweetLib.Core {
	public sealed class App {
		#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		public static IAppErrorHandler ErrorHandler { get; private set; }
		public static IAppSystemHandler SystemHandler { get; private set; }
		public static IAppResourceHandler ResourceHandler { get; private set; }
		#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		// Builder

		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		public sealed class Builder {
			public IAppErrorHandler? ErrorHandler { get; set; }
			public IAppSystemHandler? SystemHandler { get; set; }
			public IAppResourceHandler? ResourceHandler { get; set; }

			// Validation

			internal void Initialize() {
				App.ErrorHandler = Validate(ErrorHandler, nameof(ErrorHandler));
				App.SystemHandler = Validate(SystemHandler, nameof(SystemHandler));
				App.ResourceHandler = Validate(ResourceHandler, nameof(ResourceHandler));
			}

			private T Validate<T>(T? obj, string name) where T : class {
				return obj ?? throw new InvalidOperationException("Missing property " + name + " on the provided App.");
			}
		}
	}
}
