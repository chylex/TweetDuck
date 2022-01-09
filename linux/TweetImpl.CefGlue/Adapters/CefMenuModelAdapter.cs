using TweetLib.Browser.CEF.Interfaces;
using Xilium.CefGlue;

namespace TweetImpl.CefGlue.Adapters {
	sealed class CefMenuModelAdapter : IMenuModelAdapter<CefMenuModel> {
		public static CefMenuModelAdapter Instance { get; } = new ();

		public int GetItemCount(CefMenuModel model) {
			return model.Count;
		}

		public void AddCommand(CefMenuModel model, int command, string name) {
			model.AddItem(command, name);
		}

		public int GetCommandAt(CefMenuModel model, int index) {
			return model.GetCommandIdAt(index);
		}

		public void AddCheckCommand(CefMenuModel model, int command, string name) {
			model.AddCheckItem(command, name);
		}

		public void SetChecked(CefMenuModel model, int command, bool isChecked) {
			model.SetChecked(command, isChecked);
		}

		public void AddSeparator(CefMenuModel model) {
			model.AddSeparator();
		}

		public bool IsSeparatorAt(CefMenuModel model, int index) {
			return model.GetItemTypeAt(index) == CefMenuItemType.Separator;
		}

		public void RemoveAt(CefMenuModel model, int index) {
			model.RemoveAt(index);
		}
	}
}
