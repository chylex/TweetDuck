using System;
using System.IO;
using CefSharp;
using TweetLib.Browser.CEF.Logic;

namespace TweetImpl.CefSharp.Handlers {
	sealed class CefResponseFilter : IResponseFilter {
		public static CefResponseFilter Create(ResponseFilterLogic logic) {
			return logic == null ? null : new CefResponseFilter(logic);
		}

		private readonly ResponseFilterLogic logic;

		private CefResponseFilter(ResponseFilterLogic logic) {
			this.logic = logic;
		}

		bool IResponseFilter.InitFilter() {
			return true;
		}

		FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten) {
			return logic.Filter(dataIn, out dataInRead, dataOut, dataOut.Length, out dataOutWritten) switch {
				ResponseFilterLogic.FilterStatus.NeedMoreData => FilterStatus.NeedMoreData,
				ResponseFilterLogic.FilterStatus.Done         => FilterStatus.Done,
				_                                             => FilterStatus.Error
			};
		}

		void IDisposable.Dispose() {}
	}
}
