using System;
using System.Collections.Generic;

namespace TweetLib.Utils.Serialization {
	public sealed class SerializationSoftException : Exception {
		public IList<string> Errors { get; }

		public SerializationSoftException(IList<string> errors) : base(string.Join(Environment.NewLine, errors)) {
			this.Errors = errors;
		}
	}
}
