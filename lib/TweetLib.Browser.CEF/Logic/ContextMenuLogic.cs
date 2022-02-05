using System;
using System.Collections.Generic;
using TweetLib.Browser.CEF.Data;
using TweetLib.Browser.CEF.Interfaces;
using TweetLib.Browser.Contexts;
using TweetLib.Browser.Interfaces;

namespace TweetLib.Browser.CEF.Logic {
	public abstract class ContextMenuLogic {
		protected const int CommandCustomFirst = 220;
		protected const int CommandCustomLast = 250;

		protected static readonly HashSet<int> AllowedCommands = new () {
			-1,  // NotFound
			110, // Undo
			111, // Redo
			112, // Cut
			113, // Copy
			114, // Paste
			115, // Delete
			116, // SelectAll
			200, // SpellCheckSuggestion0
			201, // SpellCheckSuggestion1
			202, // SpellCheckSuggestion2
			203, // SpellCheckSuggestion3
			204, // SpellCheckSuggestion4
			205, // SpellCheckNoSuggestions
			206  // AddToDictionary
		};

		private protected sealed class ContextMenuActionRegistry : ContextMenuActionRegistry<int> {
			private const int CommandUserFirst = 26500;

			protected override int NextId(int n) {
				return CommandUserFirst + 500 + n;
			}
		}
	}

	public sealed class ContextMenuLogic<TModel> : ContextMenuLogic {
		private readonly IContextMenuHandler? handler;
		private readonly IMenuModelAdapter<TModel> modelAdapter;
		private readonly ContextMenuActionRegistry actionRegistry;

		public ContextMenuLogic(IContextMenuHandler? handler, IMenuModelAdapter<TModel> modelAdapter) {
			this.handler = handler;
			this.modelAdapter = modelAdapter;
			this.actionRegistry = new ContextMenuActionRegistry();
		}

		private sealed class ContextMenuBuilder : IContextMenuBuilder {
			private readonly IMenuModelAdapter<TModel> modelAdapter;
			private readonly ContextMenuActionRegistry actionRegistry;
			private readonly TModel model;

			public ContextMenuBuilder(IMenuModelAdapter<TModel> modelAdapter, ContextMenuActionRegistry actionRegistry, TModel model) {
				this.model = model;
				this.actionRegistry = actionRegistry;
				this.modelAdapter = modelAdapter;
			}

			public void AddAction(string name, Action action) {
				var id = actionRegistry.AddAction(action);
				modelAdapter.AddCommand(model, id, name);
			}

			public void AddActionWithCheck(string name, bool isChecked, Action action) {
				var id = actionRegistry.AddAction(action);
				modelAdapter.AddCheckCommand(model, id, name);
				modelAdapter.SetChecked(model, id, isChecked);
			}

			public void AddSeparator() {
				int count = modelAdapter.GetItemCount(model);
				if (count > 0 && !modelAdapter.IsSeparatorAt(model, count - 1)) { // do not add separators if there is nothing to separate
					modelAdapter.AddSeparator(model);
				}
			}

			public void RemoveSeparatorIfLast(TModel model) {
				int count = modelAdapter.GetItemCount(model);
				if (count > 0 && modelAdapter.IsSeparatorAt(model, count - 1)) {
					modelAdapter.RemoveAt(model, count - 1);
				}
			}
		}

		public void OnBeforeContextMenu(TModel model, Context context) {
			for (int i = modelAdapter.GetItemCount(model) - 1; i >= 0; i--) {
				int command = modelAdapter.GetCommandAt(model, i);

				if (!(AllowedCommands.Contains(command) || command is >= CommandCustomFirst and <= CommandCustomLast)) {
					modelAdapter.RemoveAt(model, i);
				}
			}

			for (int i = modelAdapter.GetItemCount(model) - 2; i >= 0; i--) {
				if (modelAdapter.IsSeparatorAt(model, i) && modelAdapter.IsSeparatorAt(model, i + 1)) {
					modelAdapter.RemoveAt(model, i);
				}
			}

			if (modelAdapter.GetItemCount(model) > 0 && modelAdapter.IsSeparatorAt(model, 0)) {
				modelAdapter.RemoveAt(model, 0);
			}

			var builder = new ContextMenuBuilder(modelAdapter, actionRegistry, model);
			builder.AddSeparator();
			handler?.Show(builder, context);
			builder.RemoveSeparatorIfLast(model);
		}

		public bool OnContextMenuCommand(int commandId) {
			return actionRegistry.Execute(commandId);
		}

		public void OnContextMenuDismissed() {
			actionRegistry.Clear();
		}

		public bool RunContextMenu() {
			return false;
		}
	}
}
