// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
	public sealed class ContextMenu : Menu {
		private static readonly FieldInfo NotifyIconWindowField = typeof(NotifyIcon).GetField("_window", BindingFlags.Instance | BindingFlags.NonPublic);

		internal static void EnsureValid() {
			if (NotifyIconWindowField == null) {
				throw new InvalidOperationException();
			}
		}

		public event EventHandler Popup;

		public void Show(Control control, Point pos) {
			if (control == null) {
				throw new ArgumentNullException(nameof(control));
			}

			if (!control.IsHandleCreated || !control.Visible) {
				throw new ArgumentException(null, nameof(control));
			}

			Popup?.Invoke(this, EventArgs.Empty);
			pos = control.PointToScreen(pos);
			NativeMethods.TrackPopupMenuEx(new HandleRef(this, Handle), NativeMethods.TPM_VERTICAL | NativeMethods.TPM_RIGHTBUTTON, pos.X, pos.Y, new HandleRef(control, control.Handle), IntPtr.Zero);
		}

		public void Show(NotifyIcon icon, Point pos) {
			Popup?.Invoke(this, EventArgs.Empty);
			NativeWindow window = (NativeWindow) NotifyIconWindowField.GetValue(icon);
			NativeMethods.TrackPopupMenuEx(new HandleRef(this, Handle), NativeMethods.TPM_VERTICAL | NativeMethods.TPM_RIGHTALIGN, pos.X, pos.Y, new HandleRef(window, window.Handle), IntPtr.Zero);
			NativeMethods.PostMessage(new HandleRef(window, window.Handle), 0, IntPtr.Zero, IntPtr.Zero);
		}
	}
}
