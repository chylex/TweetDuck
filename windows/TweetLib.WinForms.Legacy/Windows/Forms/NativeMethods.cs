// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	internal static class NativeMethods {
		public const int MIIM_STATE = 0x00000001;
		public const int MIIM_ID = 0x00000002;
		public const int MIIM_SUBMENU = 0x00000004;
		public const int MIIM_TYPE = 0x00000010;
		public const int MIIM_DATA = 0x00000020;
		public const int MF_BYPOSITION = 0x00000400;
		public const int MFT_SEPARATOR = 0x00000800;
		public const int TPM_RIGHTBUTTON = 0x0002;
		public const int TPM_RIGHTALIGN = 0x0008;
		public const int TPM_VERTICAL = 0x0040;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public sealed class MENUITEMINFO_T {
			public int cbSize = Marshal.SizeOf<MENUITEMINFO_T>();
			public int fMask;
			public int fType;
			public int fState;
			public int wID;
			public IntPtr hSubMenu;
			public IntPtr hbmpChecked;
			public IntPtr hbmpUnchecked;
			public IntPtr dwItemData;
			public string dwTypeData;
			public int cch;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct MSAAMENUINFO {
			private const int MSAA_MENU_SIG = (unchecked((int) 0xAA0DF00D));
			
			public readonly int dwMSAASignature;
			public readonly int cchWText;
			public readonly string pszWText;

			public MSAAMENUINFO(string text) {
				dwMSAASignature = MSAA_MENU_SIG;
				cchWText = text.Length;
				pszWText = text;
			}
		}

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern bool TrackPopupMenuEx(HandleRef hmenu, int fuFlags, int x, int y, HandleRef hwnd, IntPtr tpm);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool InsertMenuItem(HandleRef hMenu, int uItem, bool fByPosition, MENUITEMINFO_T lpmii);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool SetMenuItemInfo(HandleRef hMenu, int uItem, bool fByPosition, MENUITEMINFO_T lpmii);

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern int GetMenuItemCount(HandleRef hMenu);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr CreatePopupMenu();

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern bool RemoveMenu(HandleRef hMenu, int uPosition, int uFlags);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern bool DestroyMenu(HandleRef hMenu);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool PostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam);
	}
}
