using System;
using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefErrorCodeAdapter : IErrorCodeAdapter<CefErrorCode> {
		public static CefErrorCodeAdapter Instance { get; } = new ();

		private CefErrorCodeAdapter() {}

		public bool IsAborted(CefErrorCode errorCode) {
			return errorCode == CefErrorCode.Aborted;
		}

		public string GetName(CefErrorCode errorCode) {
			return Enum.GetName(typeof(CefErrorCode), errorCode) ?? string.Empty;
		}
	}
}
