using System.Collections.Generic;
using System.Drawing;
using CefSharp;
using TweetDuck.Browser.Adapters;
using TweetDuck.Configuration;
using TweetDuck.Utils;
using TweetLib.Browser.Contexts;

namespace TweetDuck.Browser.Handling {
	abstract class ContextMenuBase : IContextMenuHandler {
		private const CefMenuCommand MenuOpenDevTools = (CefMenuCommand) 26500;

		private static readonly HashSet<CefMenuCommand> AllowedCefCommands = new HashSet<CefMenuCommand> {
			CefMenuCommand.NotFound,
			CefMenuCommand.Undo,
			CefMenuCommand.Redo,
			CefMenuCommand.Cut,
			CefMenuCommand.Copy,
			CefMenuCommand.Paste,
			CefMenuCommand.Delete,
			CefMenuCommand.SelectAll,
			CefMenuCommand.SpellCheckSuggestion0,
			CefMenuCommand.SpellCheckSuggestion1,
			CefMenuCommand.SpellCheckSuggestion2,
			CefMenuCommand.SpellCheckSuggestion3,
			CefMenuCommand.SpellCheckSuggestion4,
			CefMenuCommand.SpellCheckNoSuggestions,
			CefMenuCommand.AddToDictionary
		};

		protected static UserConfig Config => Program.Config.User;

		private readonly TweetLib.Browser.Interfaces.IContextMenuHandler handler;
		private readonly CefContextMenuActionRegistry actionRegistry;

		protected ContextMenuBase(TweetLib.Browser.Interfaces.IContextMenuHandler handler) {
			this.handler = handler;
			this.actionRegistry = new CefContextMenuActionRegistry();
		}

		protected virtual Context CreateContext(IContextMenuParams parameters) {
			return CefContextMenuModel.CreateContext(parameters, null, Config.TwitterImageQuality);
		}

		public virtual void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) {
			for (int i = model.Count - 1; i >= 0; i--) {
				CefMenuCommand command = model.GetCommandIdAt(i);

				if (!AllowedCefCommands.Contains(command) && !(command >= CefMenuCommand.CustomFirst && command <= CefMenuCommand.CustomLast)) {
					model.RemoveAt(i);
				}
			}

			for (int i = model.Count - 2; i >= 0; i--) {
				if (model.GetTypeAt(i) == MenuItemType.Separator && model.GetTypeAt(i + 1) == MenuItemType.Separator) {
					model.RemoveAt(i);
				}
			}

			if (model.Count > 0 && model.GetTypeAt(0) == MenuItemType.Separator) {
				model.RemoveAt(0);
			}

			AddSeparator(model);
			handler.Show(new CefContextMenuModel(model, actionRegistry), CreateContext(parameters));
			RemoveSeparatorIfLast(model);
		}

		public virtual bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags) {
			if (actionRegistry.Execute(commandId)) {
				return true;
			}

			if (commandId == MenuOpenDevTools) {
				browserControl.OpenDevToolsCustom(new Point(parameters.XCoord, parameters.YCoord));
				return true;
			}

			return false;
		}

		public virtual void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame) {
			actionRegistry.Clear();
		}

		public virtual bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback) {
			return false;
		}

		protected static void AddDebugMenuItems(IMenuModel model) {
			if (Config.DevToolsInContextMenu) {
				AddSeparator(model);
				model.AddItem(MenuOpenDevTools, "Open dev tools");
			}
		}

		protected static void RemoveSeparatorIfLast(IMenuModel model) {
			if (model.Count > 0 && model.GetTypeAt(model.Count - 1) == MenuItemType.Separator) {
				model.RemoveAt(model.Count - 1);
			}
		}

		protected static void AddSeparator(IMenuModel model) {
			if (model.Count > 0 && model.GetTypeAt(model.Count - 1) != MenuItemType.Separator) { // do not add separators if there is nothing to separate
				model.AddSeparator();
			}
		}
	}
}
