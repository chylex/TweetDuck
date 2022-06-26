// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public sealed class MenuItem : Menu {
		private const int StateBarBreak = 0x00000020;
		private const int StateBreak = 0x00000040;
		private const int StateChecked = 0x00000008;
		private const int StateDefault = 0x00001000;
		private const int StateDisabled = 0x00000003;
		private const int StateRadioCheck = 0x00000200;
		private const int StateHidden = 0x00010000;
		private const int StateCloneMask = 0x0003136B;
		private const int StateOwnerDraw = 0x00000100;

		private bool _hasHandle;
		private MenuItemData _data;
		private MenuItem _nextLinkedItem; // Next item linked to the same MenuItemData.

		private const uint FirstUniqueID = 0xC0000000;
		private uint _uniqueID = 0;
		private IntPtr _msaaMenuInfoPtr = IntPtr.Zero;

		private MenuItem() : this(0, null, null, null, null) {}

		internal MenuItem(string text) : this(0, text, null, null, null) {}

		internal MenuItem(string text, EventHandler onClick) : this(0, text, onClick, null, null) {}

		private MenuItem(Shortcut shortcut, string text, EventHandler onClick, EventHandler onPopup, EventHandler onSelect) {
			var _ = new MenuItemData(this, shortcut, true, text, onClick, onPopup, onSelect, null, null);
		}

		internal Menu Parent { get; set; }

		internal int Index {
			get {
				if (Parent != null) {
					for (int i = 0; i < Parent.ItemCount; i++) {
						if (Parent.items[i] == this) {
							return i;
						}
					}
				}

				return -1;
			}
		}

		private	int MenuID {
			get {
				CheckIfDisposed();
				return _data.GetMenuID();
			}
		}

		public bool Enabled {
			get {
				CheckIfDisposed();
				return (_data.State & StateDisabled) == 0;
			}
			set {
				CheckIfDisposed();
				_data.SetState(StateDisabled, !value);
			}
		}

		public bool Checked {
			get {
				CheckIfDisposed();
				return (_data.State & StateChecked) != 0;
			}
			set {
				CheckIfDisposed();

				if (value && ItemCount != 0) {
					throw new ArgumentException(null, nameof(value));
				}

				_data.SetState(StateChecked, value);
			}
		}

		public bool RadioCheck {
			get {
				CheckIfDisposed();
				return (_data.State & StateRadioCheck) != 0;
			}
			set {
				CheckIfDisposed();
				_data.SetState(StateRadioCheck, value);
			}
		}

		private string Text {
			get {
				CheckIfDisposed();
				return _data.caption;
			}
		}

		private Shortcut Shortcut {
			get {
				CheckIfDisposed();
				return _data.shortcut;
			}
		}

		private bool ShowShortcut {
			get {
				CheckIfDisposed();
				return _data.showShortcut;
			}
		}

		public bool Visible {
			get {
				CheckIfDisposed();
				return _data.Visible;
			}
			set {
				CheckIfDisposed();
				_data.Visible = value;
			}
		}

		internal MenuItem CloneMenu() {
			var newItem = new MenuItem();
			newItem.CloneMenu(this);
			return newItem;
		}

		private	void CloneMenu(MenuItem itemSrc) {
			base.CloneMenu(itemSrc);
			int state = itemSrc._data.State;
			var _ = new MenuItemData(this, itemSrc.Shortcut, itemSrc.ShowShortcut, itemSrc.Text, itemSrc._data.onClick, itemSrc._data.onPopup, itemSrc._data.onSelect, itemSrc._data.onDrawItem, itemSrc._data.onMeasureItem);
			_data.SetState(state & StateCloneMask, true);
		}

		internal void CreateMenuItem() {
			if ((_data.State & StateHidden) == 0) {
				NativeMethods.MENUITEMINFO_T info = CreateMenuItemInfo();
				NativeMethods.InsertMenuItem(new HandleRef(Parent, Parent.handle), -1, true, info);
				_hasHandle = info.hSubMenu != IntPtr.Zero;
			}
		}

		private NativeMethods.MENUITEMINFO_T CreateMenuItemInfo() {
			var info = new NativeMethods.MENUITEMINFO_T {
				fMask = NativeMethods.MIIM_ID | NativeMethods.MIIM_STATE | NativeMethods.MIIM_SUBMENU | NativeMethods.MIIM_TYPE | NativeMethods.MIIM_DATA,
				fType = _data.State & (StateBarBreak | StateBreak | StateRadioCheck | StateOwnerDraw)
			};

			if (_data.caption.Equals("-")) {
				info.fType |= NativeMethods.MFT_SEPARATOR;
			}

			info.fState = _data.State & (StateChecked | StateDefault | StateDisabled);

			info.wID = MenuID;
			if (IsParent) {
				info.hSubMenu = Handle;
			}

			info.hbmpChecked = IntPtr.Zero;
			info.hbmpUnchecked = IntPtr.Zero;

			if (IntPtr.Size == 4) {
				// Store the unique ID in the dwItemData..
				// For simple menu items, we can just put the unique ID in the dwItemData.
				// But for owner-draw items, we need to point the dwItemData at an MSAAMENUINFO
				// structure so that MSAA can get the item text.
				// To allow us to reliably distinguish between IDs and structure pointers later
				// on, we keep IDs in the 0xC0000000-0xFFFFFFFF range. This is the top 1Gb of
				// unmananged process memory, where an app's heap allocations should never come
				// from. So that we can still get the ID from the dwItemData for an owner-draw
				// item later on, a copy of the ID is tacked onto the end of the MSAAMENUINFO
				// structure.
				if (_data.OwnerDraw) {
					info.dwItemData = AllocMsaaMenuInfo();
				}
				else {
					info.dwItemData = (IntPtr) unchecked((int) _uniqueID);
				}
			}
			else {
				// On Win64, there are no reserved address ranges we can use for menu item IDs. So instead we will
				// have to allocate an MSAMENUINFO heap structure for all menu items, not just owner-drawn ones.
				info.dwItemData = AllocMsaaMenuInfo();
			}

			// We won't render the shortcut if: 1) it's not set, 2) we're a parent, 3) we're toplevel
			if (_data.showShortcut && _data.shortcut != 0 && !IsParent) {
				info.dwTypeData = _data.caption + "\t" + TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString((Keys) (int) _data.shortcut);
			}
			else {
				// Windows issue: Items with empty captions sometimes block keyboard
				// access to other items in same menu.
				info.dwTypeData = (_data.caption.Length == 0 ? " " : _data.caption);
			}

			info.cch = 0;

			return info;
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				Parent?.MenuItems.Remove(this);
				_data?.RemoveItem(this);
				_uniqueID = 0;
			}

			FreeMsaaMenuInfo();
			base.Dispose(disposing);
		}

		[StructLayout(LayoutKind.Sequential)]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
		private struct MsaaMenuInfoWithId {
			public readonly NativeMethods.MSAAMENUINFO _msaaMenuInfo;
			public readonly uint _uniqueID;

			public MsaaMenuInfoWithId(string text, uint uniqueID) {
				_msaaMenuInfo = new NativeMethods.MSAAMENUINFO(text);
				_uniqueID = uniqueID;
			}
		}

		private IntPtr AllocMsaaMenuInfo() {
			FreeMsaaMenuInfo();
			_msaaMenuInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MsaaMenuInfoWithId>());

			if (IntPtr.Size == 4) {
				// We only check this on Win32, irrelevant on Win64 (see CreateMenuItemInfo)
				// Check for incursion into menu item ID range (unlikely!)
				Debug.Assert(((uint) (ulong) _msaaMenuInfoPtr) < FirstUniqueID);
			}

			MsaaMenuInfoWithId msaaMenuInfoStruct = new MsaaMenuInfoWithId(_data.caption, _uniqueID);
			Marshal.StructureToPtr(msaaMenuInfoStruct, _msaaMenuInfoPtr, false);
			Debug.Assert(_msaaMenuInfoPtr != IntPtr.Zero);
			return _msaaMenuInfoPtr;
		}

		private void FreeMsaaMenuInfo() {
			if (_msaaMenuInfoPtr != IntPtr.Zero) {
				Marshal.DestroyStructure(_msaaMenuInfoPtr, typeof(MsaaMenuInfoWithId));
				Marshal.FreeHGlobal(_msaaMenuInfoPtr);
				_msaaMenuInfoPtr = IntPtr.Zero;
			}
		}

		internal override void ItemsChanged(int change) {
			base.ItemsChanged(change);

			if (change == CHANGE_ITEMS) {
				// when the menu collection changes deal with it locally
				Debug.Assert(!created, "base.ItemsChanged should have wiped out our handles");
				if (Parent is { created: true }) {
					UpdateMenuItem(force: true);
					CreateMenuItems();
				}
			}
			else {
				if (!_hasHandle && IsParent) {
					UpdateMenuItem(force: true);
				}
			}
		}

		internal void ItemsChanged(int change, MenuItem item) {
			if (change == CHANGE_ITEMADDED && _data is { baseItem: {} } && _data.baseItem.MenuItems.Contains(item)) {
				if (Parent is { created: true }) {
					UpdateMenuItem(force: true);
					CreateMenuItems();
				}
				else if (_data != null) {
					MenuItem currentMenuItem = _data.firstItem;
					while (currentMenuItem != null) {
						if (currentMenuItem.created) {
							MenuItem newItem = item.CloneMenu();
							item._data.AddItem(newItem);
							currentMenuItem.MenuItems.Add(newItem);
							break;
						}

						currentMenuItem = currentMenuItem._nextLinkedItem;
					}
				}
			}
		}

		private	void OnClick(EventArgs e) {
			CheckIfDisposed();

			if (_data.baseItem != this) {
				_data.baseItem.OnClick(e);
			}
			else {
				_data.onClick?.Invoke(this, e);
			}
		}

		internal void UpdateMenuItem(bool force) {
			if (Parent is not { created: true }) {
				return;
			}

			if (force || Parent is ContextMenu) {
				NativeMethods.MENUITEMINFO_T info = CreateMenuItemInfo();
				NativeMethods.SetMenuItemInfo(new HandleRef(Parent, Parent.handle), MenuID, false, info);

				if (_hasHandle && info.hSubMenu == IntPtr.Zero) {
					ClearHandles();
				}

				_hasHandle = info.hSubMenu != IntPtr.Zero;
			}
		}

		private void CheckIfDisposed() {
			if (_data == null) {
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		private sealed class MenuItemData : ICommandExecutor {
			internal MenuItem baseItem;
			internal MenuItem firstItem;

			internal readonly string caption;
			internal readonly Shortcut shortcut;
			internal readonly bool showShortcut;
			internal EventHandler onClick;
			internal EventHandler onPopup;
			internal EventHandler onSelect;
			internal DrawItemEventHandler onDrawItem;
			internal MeasureItemEventHandler onMeasureItem;

			private Command2 cmd;

			internal MenuItemData(MenuItem baseItem, Shortcut shortcut, bool showShortcut, string caption, EventHandler onClick, EventHandler onPopup, EventHandler onSelect, DrawItemEventHandler onDrawItem, MeasureItemEventHandler onMeasureItem) {
				AddItem(baseItem);
				this.shortcut = shortcut;
				this.showShortcut = showShortcut;
				this.caption = caption ?? string.Empty;
				this.onClick = onClick;
				this.onPopup = onPopup;
				this.onSelect = onSelect;
				this.onDrawItem = onDrawItem;
				this.onMeasureItem = onMeasureItem;
			}

			internal int State { get; private set; }

			internal bool OwnerDraw => (State & StateOwnerDraw) != 0;

			internal bool Visible {
				get => (State & StateHidden) == 0;
				set {
					if (((State & StateHidden) == 0) != value) {
						State = value ? State & ~StateHidden : State | StateHidden;
						ItemsChanged(CHANGE_VISIBLE);
					}
				}
			}

			internal void AddItem(MenuItem item) {
				if (item._data != this) {
					item._data?.RemoveItem(item);

					item._nextLinkedItem = firstItem;
					firstItem = item;
					baseItem ??= item;

					item._data = this;
					item.UpdateMenuItem(false);
				}
			}

			public void Execute() {
				baseItem?.OnClick(EventArgs.Empty);
			}

			internal int GetMenuID() {
				cmd ??= new Command2(this);
				return cmd.ID;
			}

			private void ItemsChanged(int change) {
				for (MenuItem item = firstItem; item != null; item = item._nextLinkedItem) {
					item.Parent?.ItemsChanged(change);
				}
			}

			internal void RemoveItem(MenuItem item) {
				Debug.Assert(item._data == this, "bad item passed to MenuItemData.removeItem");

				if (item == firstItem) {
					firstItem = item._nextLinkedItem;
				}
				else {
					MenuItem itemT;
					for (itemT = firstItem; item != itemT._nextLinkedItem;) {
						itemT = itemT._nextLinkedItem;
					}

					itemT._nextLinkedItem = item._nextLinkedItem;
				}

				item._nextLinkedItem = null;
				item._data = null;

				if (item == baseItem) {
					baseItem = firstItem;
				}

				if (firstItem == null) {
					// No longer needed. Toss all references and the Command object.
					Debug.Assert(baseItem == null, "why isn't baseItem null?");
					onClick = null;
					onPopup = null;
					onSelect = null;
					onDrawItem = null;
					onMeasureItem = null;
					if (cmd != null) {
						cmd.Dispose();
						cmd = null;
					}
				}
			}

			internal void SetState(int flag, bool value) {
				if (((State & flag) != 0) != value) {
					State = value ? State | flag : State & ~flag;
					UpdateMenuItems();
				}
			}

			private void UpdateMenuItems() {
				for (MenuItem item = firstItem; item != null; item = item._nextLinkedItem) {
					item.UpdateMenuItem(force: true);
				}
			}
		}
	}
}
