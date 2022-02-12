using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Adapters {
	sealed class CefDragDataAdapter : IDragDataAdapter<IDragData> {
		public static CefDragDataAdapter Instance { get; } = new CefDragDataAdapter();

		private CefDragDataAdapter() {}

		public bool IsLink(IDragData data) {
			return data.IsLink;
		}

		public string GetLink(IDragData data) {
			return data.LinkUrl;
		}

		public bool IsFragment(IDragData data) {
			return data.IsFragment;
		}

		public string GetFragmentAsText(IDragData data) {
			return data.FragmentText;
		}
	}
}
