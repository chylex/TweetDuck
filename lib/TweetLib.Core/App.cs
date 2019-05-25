using System;
using TweetLib.Core.Application;

namespace TweetLib.Core{
    public sealed class App{

        // Builder

        public sealed class Builder{

            // Validation

            internal void Initialize(){
            }

            private T Validate<T>(T obj, string name){
                if (obj == null){
                    throw new InvalidOperationException("Missing property " + name + " on the provided App.");
                }

                return obj;
            }
        }
    }
}
