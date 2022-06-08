using System.Windows.Forms;
using CefSharp;
using TweetDuck.Controls;
using TweetLib.Browser.Contexts;
using TweetLib.Core.Features.TweetDeck;
using TweetLib.Core.Features.Twitter;
using TweetLib.Core.Systems.Configuration;
using IContextMenuHandler = TweetLib.Browser.Interfaces.IContextMenuHandler;

namespace TweetDuck.Browser.Base {
	sealed class ContextMenuBrowser : ContextMenuBase {
		private const CefMenuCommand MenuGlobal   = (CefMenuCommand) 26600;
		private const CefMenuCommand MenuMute     = (CefMenuCommand) 26601;
		private const CefMenuCommand MenuSettings = (CefMenuCommand) 26602;
		private const CefMenuCommand MenuPlugins  = (CefMenuCommand) 26003;
		private const CefMenuCommand MenuAbout    = (CefMenuCommand) 26604;

		private const string TitleReloadBrowser = "Reload browser";
		private const string TitleMuteNotifications = "Mute notifications";
		private const string TitleSettings = "Options";
		private const string TitlePlugins = "Plugins";
		private const string TitleAboutProgram = "About " + Program.BrandName;

		private readonly FormBrowser form;
		private readonly TweetDeckExtraContext extraContext;

		public ContextMenuBrowser(FormBrowser form, IContextMenuHandler? handler, TweetDeckExtraContext extraContext) : base(handler) {
			this.form = form;
			this.extraContext = extraContext;
		}

		protected override Context CreateContext(IContextMenuParams parameters) {
			return CreateContext(parameters, extraContext, Config.TwitterImageQuality);
		}

		public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) {
			if (!TwitterUrls.IsTweetDeck(frame.Url) || browser.IsLoading) {
				extraContext.Reset();
			}

			base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);

			bool isSelecting = parameters.TypeFlags.HasFlag(ContextMenuType.Selection);
			bool isEditing = parameters.TypeFlags.HasFlag(ContextMenuType.Editable);

			if (!isSelecting && !isEditing) {
				AddSeparator(model);

				IMenuModel globalMenu = model.Count == 0 ? model : model.AddSubMenu(MenuGlobal, Program.BrandName);

				globalMenu.AddItem(CefMenuCommand.Reload, TitleReloadBrowser);
				globalMenu.AddCheckItem(MenuMute, TitleMuteNotifications);
				globalMenu.SetChecked(MenuMute, Config.MuteNotifications);
				globalMenu.AddSeparator();

				globalMenu.AddItem(MenuSettings, TitleSettings);
				globalMenu.AddItem(MenuPlugins, TitlePlugins);
				globalMenu.AddItem(MenuAbout, TitleAboutProgram);

				AddDebugMenuItems(globalMenu);
			}
		}

		protected override void AddLastContextMenuItems(IMenuModel model) {}

		public override bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags) {
			if (base.OnContextMenuCommand(browserControl, browser, frame, parameters, commandId, eventFlags)) {
				return true;
			}

			switch (commandId) {
				case CefMenuCommand.Reload:
					form.InvokeAsyncSafe(form.ReloadToTweetDeck);
					return true;

				case MenuSettings:
					form.InvokeAsyncSafe(form.OpenSettings);
					return true;

				case MenuAbout:
					form.InvokeAsyncSafe(form.OpenAbout);
					return true;

				case MenuPlugins:
					form.InvokeAsyncSafe(form.OpenPlugins);
					return true;

				case MenuMute:
					form.InvokeAsyncSafe(ToggleMuteNotifications);
					return true;

				default:
					return false;
			}
		}

		public override void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame) {
			base.OnContextMenuDismissed(browserControl, browser, frame);
			extraContext.Reset();
		}

		public static ContextMenuStrip CreateMenu(FormBrowser form) {
			ContextMenuStrip menu = new ContextMenuStrip();

			menu.Items.Add(TitleReloadBrowser, null, (sender, args) => form.ReloadToTweetDeck());
			menu.Items.Add(TitleMuteNotifications, null, (sender, args) => ToggleMuteNotifications());
			menu.Items.Add("-");
			menu.Items.Add(TitleSettings, null, (sender, args) => form.OpenSettings());
			menu.Items.Add(TitlePlugins, null, (sender, args) => form.OpenPlugins());
			menu.Items.Add(TitleAboutProgram, null,  (sender, args) => form.OpenAbout());

			menu.Opening += (sender, args) => {
				((ToolStripMenuItem) menu.Items[1]).Checked = Config.MuteNotifications;
			};

			return menu;
		}

		private static void ToggleMuteNotifications() {
			Config.MuteNotifications = !Config.MuteNotifications;
			Config.Save();
		}
	}
}
