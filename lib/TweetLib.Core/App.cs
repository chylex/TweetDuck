﻿using System;
using TweetLib.Core.Application;

namespace TweetLib.Core{
    public sealed class App{
        public static IAppErrorHandler ErrorHandler { get; private set; }

        // Builder

        public sealed class Builder{
            public IAppErrorHandler? ErrorHandler { get; set; }

            // Validation

            internal void Initialize(){
                App.ErrorHandler = Validate(ErrorHandler, nameof(ErrorHandler))!;
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
