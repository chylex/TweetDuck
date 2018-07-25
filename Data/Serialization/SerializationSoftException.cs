using System;
using System.Collections.Generic;

namespace TweetDuck.Data.Serialization{
    sealed class SerializationSoftException : Exception{
        public IList<string> Errors { get; }

        public SerializationSoftException(IList<string> errors) : base(string.Join(Environment.NewLine, errors)){
            this.Errors = errors;
        }
    }
}
