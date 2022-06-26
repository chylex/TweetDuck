// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
	public abstract class Menu : Component {
		internal const int CHANGE_ITEMS = 0;
		internal const int CHANGE_VISIBLE = 1;
		internal const int CHANGE_ITEMADDED = 4;

		private MenuItemCollection itemsCollection;
		internal MenuItem[] items;
		internal IntPtr handle;
		internal bool created;

		internal IntPtr Handle {
			get {
				if (handle == IntPtr.Zero) {
					handle = CreateMenuHandle();
				}

				CreateMenuItems();
				return handle;
			}
		}

		protected bool IsParent => items != null && ItemCount > 0;

		internal int ItemCount { get; private set; }

		public MenuItemCollection MenuItems => itemsCollection ??= new MenuItemCollection(this);

		internal void ClearHandles() {
			if (handle != IntPtr.Zero) {
				NativeMethods.DestroyMenu(new HandleRef(this, handle));
			}

			handle = IntPtr.Zero;
			
			if (created) {
				for (int i = 0; i < ItemCount; i++) {
					items[i].ClearHandles();
				}

				created = false;
			}
		}

		protected void CloneMenu(Menu menuSrc) {
			if (menuSrc == null) {
				throw new ArgumentNullException(nameof(menuSrc));
			}

			MenuItem[] newItems = null;
			if (menuSrc.items != null) {
				int count = menuSrc.MenuItems.Count;
				newItems = new MenuItem[count];
				for (int i = 0; i < count; i++) {
					newItems[i] = menuSrc.MenuItems[i].CloneMenu();
				}
			}

			MenuItems.Clear();
			if (newItems != null) {
				MenuItems.AddRange(newItems);
			}
		}

		private IntPtr CreateMenuHandle() {
			return NativeMethods.CreatePopupMenu();
		}

		internal void CreateMenuItems() {
			if (!created) {
				for (int i = 0; i < ItemCount; i++) {
					items[i].CreateMenuItem();
				}

				created = true;
			}
		}

		private void DestroyMenuItems() {
			if (created) {
				for (int i = 0; i < ItemCount; i++) {
					items[i].ClearHandles();
				}

				while (NativeMethods.GetMenuItemCount(new HandleRef(this, handle)) > 0) {
					NativeMethods.RemoveMenu(new HandleRef(this, handle), 0, NativeMethods.MF_BYPOSITION);
				}

				created = false;
			}
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				while (ItemCount > 0) {
					MenuItem item = items[--ItemCount];

					if (item.Site is { Container: {} }) {
						item.Site.Container.Remove(item);
					}

					item.Parent = null;
					item.Dispose();
				}

				items = null;
			}

			if (handle != IntPtr.Zero) {
				NativeMethods.DestroyMenu(new HandleRef(this, handle));
				handle = IntPtr.Zero;
				if (disposing) {
					ClearHandles();
				}
			}

			base.Dispose(disposing);
		}

		internal virtual void ItemsChanged(int change) {
			switch (change) {
				case CHANGE_ITEMS:
				case CHANGE_VISIBLE:
					DestroyMenuItems();
					break;
			}
		}

		public sealed class MenuItemCollection {
			private readonly Menu owner;

			internal MenuItemCollection(Menu owner) {
				this.owner = owner;
			}

			public MenuItem this[int index] {
				get {
					if (index < 0 || index >= owner.ItemCount) {
						throw new ArgumentOutOfRangeException(nameof(index));
					}

					return owner.items[index];
				}
			}

			internal int Count => owner.ItemCount;

			public void Add(string caption) {
				Add(new MenuItem(caption));
			}

			public void Add(string caption, EventHandler onClick) {
				Add(new MenuItem(caption, onClick));
			}

			internal void Add(MenuItem item) {
				Add(owner.ItemCount, item);
			}

			private void Add(int index, MenuItem item) {
				if (item == null) {
					throw new ArgumentNullException(nameof(item));
				}

				// MenuItems can only belong to one menu at a time
				if (item.Parent != null) {
					throw new InvalidOperationException();
				}

				// Validate our index
				if (index < 0 || index > owner.ItemCount) {
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				if (owner.items == null || owner.items.Length == owner.ItemCount) {
					MenuItem[] newItems = new MenuItem[owner.ItemCount < 2 ? 4 : owner.ItemCount * 2];
					if (owner.ItemCount > 0) {
						Array.Copy(owner.items!, 0, newItems, 0, owner.ItemCount);
					}

					owner.items = newItems;
				}

				Array.Copy(owner.items, index, owner.items, index + 1, owner.ItemCount - index);
				owner.items[index] = item;
				owner.ItemCount++;
				item.Parent = owner;
				owner.ItemsChanged(CHANGE_ITEMS);
				if (owner is MenuItem menuItem) {
					menuItem.ItemsChanged(CHANGE_ITEMADDED, item);
				}
			}

			internal void AddRange(MenuItem[] items) {
				if (items == null) {
					throw new ArgumentNullException(nameof(items));
				}

				foreach (MenuItem item in items) {
					Add(item);
				}
			}

			internal bool Contains(MenuItem value) {
				for (int index = 0; index < Count; ++index) {
					if (this[index] == value) {
						return true;
					}
				}

				return false;
			}

			internal void Clear() {
				if (owner.ItemCount > 0) {
					for (int i = 0; i < owner.ItemCount; i++) {
						owner.items[i].Parent = null;
					}

					owner.ItemCount = 0;
					owner.items = null;

					owner.ItemsChanged(CHANGE_ITEMS);

					if (owner is MenuItem item) {
						item.UpdateMenuItem(true);
					}
				}
			}

			internal void Remove(MenuItem item) {
				if (item.Parent == owner) {
					RemoveAt(item.Index);
				}
			}

			private void RemoveAt(int index) {
				if (index < 0 || index >= owner.ItemCount) {
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				MenuItem item = owner.items[index];
				item.Parent = null;
				owner.ItemCount--;
				Array.Copy(owner.items, index + 1, owner.items, index, owner.ItemCount - index);
				owner.items[owner.ItemCount] = null;
				owner.ItemsChanged(CHANGE_ITEMS);

				if (owner.ItemCount == 0) {
					Clear();
				}
			}
		}
	}
}
