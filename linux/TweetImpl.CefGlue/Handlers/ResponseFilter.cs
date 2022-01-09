using System.IO;
using TweetLib.Browser.CEF.Logic;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Handlers {
	sealed class ResponseFilter : CefResponseFilter {
		private readonly ResponseFilterLogic logic;

		public ResponseFilter(ResponseFilterLogic logic) {
			this.logic = logic;
		}

		protected override bool InitFilter() {
			return true;
		}

		protected override CefResponseFilterStatus Filter(UnmanagedMemoryStream dataIn, long dataInSize, out long dataInRead, UnmanagedMemoryStream dataOut, long dataOutSize, out long dataOutWritten) {
			return logic.Filter(dataIn, out dataInRead, dataOut, dataOutSize, out dataOutWritten) switch {
				ResponseFilterLogic.FilterStatus.NeedMoreData => CefResponseFilterStatus.NeedMoreData,
				ResponseFilterLogic.FilterStatus.Done         => CefResponseFilterStatus.Done,
				_                                             => CefResponseFilterStatus.Error
			};
		}
	}
}
