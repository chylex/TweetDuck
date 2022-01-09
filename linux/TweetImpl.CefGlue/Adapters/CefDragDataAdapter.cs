using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefDragDataAdapter : IDragDataAdapter<CefDragData> {
		public static CefDragDataAdapter Instance { get; } = new ();

		private CefDragDataAdapter() {}

		public bool IsLink(CefDragData data) {
			return data.IsLink;
		}

		public string GetLink(CefDragData data) {
			return data.LinkUrl;
		}

		public bool IsFragment(CefDragData data) {
			return data.IsFragment;
		}

		public string GetFragmentAsText(CefDragData data) {
			return data.FragmentText;
		}
	}
}
