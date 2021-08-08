namespace TweetLib.Api.Data.Notification {
	/// <summary>
	/// Allows extensions to decide which screen to use for notifications.
	///
	/// Every registered provider becomes available in the Options dialog and has to be explicitly selected by the user. Only one provider
	/// can be active at any given time.
	/// </summary>
	public interface IDesktopNotificationScreenProvider {
		/// <summary>
		/// A unique identifier of this provider. Only needs to be unique within the scope of this plugin.
		/// </summary>
		Resource Id { get; }

		/// <summary>
		/// Text displayed in the user interface.
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Returns a screen that will be used to display the next desktop notification.
		///
		/// If the return value is <c>null</c> or a screen that is not present in <see cref="IScreenLayout.AllScreens" />, desktop
		/// notifications will be temporarily paused and this method will be called again after an unspecified amount of time (but
		/// not sooner than 1 second since the last call).
		/// </summary>
		IScreen? PickScreen(IScreenLayout layout);
	}
}
