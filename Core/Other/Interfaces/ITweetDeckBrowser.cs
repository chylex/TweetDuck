using System;
using CefSharp;

namespace TweetDuck.Core.Other.Interfaces{
    interface ITweetDeckBrowser{
        bool IsTweetDeckWebsite { get; }

        void RegisterBridge(string name, object obj);
        void OnFrameLoaded(Action<IFrame> callback);
        void ExecuteFunction(string name, params object[] args);
    }
}
