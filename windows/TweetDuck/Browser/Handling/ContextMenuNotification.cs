using CefSharp;
using TweetDuck.Browser.Notification;
using TweetDuck.Controls;
using TweetLib.Browser.Contexts;
using IContextMenuHandler = TweetLib.Browser.Interfaces.IContextMenuHandler;

namespace TweetDuck.Browser.Handling {
	sealed class ContextMenuNotification : ContextMenuBase {
		private readonly FormNotificationBase form;

		public ContextMenuNotification(FormNotificationBase form, IContextMenuHandler handler) : base(handler) {
			this.form = form;
		}

		protected override Context CreateContext(IContextMenuParams parameters) {
			Context context = base.CreateContext(parameters);
			context.Notification = new TweetLib.Browser.Contexts.Notification(form.CurrentTweetUrl, form.CurrentQuoteUrl);
			return context;
		}

		public override void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) {
			base.OnBeforeContextMenu(browserControl, browser, frame, parameters, model);
			form.InvokeAsyncSafe(() => form.ContextMenuOpen = true);
		}

		public override void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame) {
			base.OnContextMenuDismissed(browserControl, browser, frame);
			form.InvokeAsyncSafe(() => form.ContextMenuOpen = false);
		}
	}
}
