using System;
using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Adapters {
	sealed class CefErrorCodeAdapter : IErrorCodeAdapter<CefErrorCode> {
		public static CefErrorCodeAdapter Instance { get; } = new CefErrorCodeAdapter();

		private CefErrorCodeAdapter() {}

		public bool IsAborted(CefErrorCode errorCode) {
			return errorCode == CefErrorCode.Aborted;
		}

		public string? GetName(CefErrorCode errorCode) {
			return Enum.GetName(typeof(CefErrorCode), errorCode);
		}
	}
}
