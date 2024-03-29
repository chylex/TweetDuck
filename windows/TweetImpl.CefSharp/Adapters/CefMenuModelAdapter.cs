using CefSharp;
using TweetLib.Browser.CEF.Interfaces;

namespace TweetImpl.CefSharp.Adapters {
	sealed class CefMenuModelAdapter : IMenuModelAdapter<IMenuModel> {
		public static CefMenuModelAdapter Instance { get; } = new ();

		private CefMenuModelAdapter() {}

		public int GetItemCount(IMenuModel model) {
			return model.Count;
		}

		public void AddCommand(IMenuModel model, int command, string name) {
			model.AddItem((CefMenuCommand) command, name);
		}

		public int GetCommandAt(IMenuModel model, int index) {
			return (int) model.GetCommandIdAt(index);
		}

		public void AddCheckCommand(IMenuModel model, int command, string name) {
			model.AddCheckItem((CefMenuCommand) command, name);
		}

		public void SetChecked(IMenuModel model, int command, bool isChecked) {
			model.SetChecked((CefMenuCommand) command, isChecked);
		}

		public void AddSeparator(IMenuModel model) {
			model.AddSeparator();
		}

		public bool IsSeparatorAt(IMenuModel model, int index) {
			return model.GetTypeAt(index) == MenuItemType.Separator;
		}

		public void RemoveAt(IMenuModel model, int index) {
			model.RemoveAt(index);
		}
	}
}
