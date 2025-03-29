namespace UIWidgets.Menu
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UnityEngine;
#if ENABLE_INPUT_SYSTEM
	using UnityEngine.InputSystem;
#endif

	/// <summary>
	/// Menu item.
	/// </summary>
	public class MenuItem : IObservable, INotifyPropertyChanged
	{
		string name = string.Empty;

		/// <summary>
		/// Name.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				Change(ref name, value, "Name");
			}
		}

		string template = string.Empty;

		/// <summary>
		/// Template.
		/// </summary>
		public string Template
		{
			get
			{
				return template;
			}

			set
			{
				Change(ref template, value, "Template");
			}
		}

		bool isChecked;

		/// <summary>
		/// Checked.
		/// </summary>
		public bool Checked
		{
			get
			{
				return isChecked;
			}

			set
			{
				Change(ref isChecked, value, "Checked");
			}
		}

		Sprite icon;

		/// <summary>
		/// Icon.
		/// </summary>
		public Sprite Icon
		{
			get
			{
				return icon;
			}

			set
			{
				Change(ref icon, value, "Icon");
			}
		}

		HotKey hotKey = new HotKey(HotKeyCode.None);

		/// <summary>
		/// Hot Key.
		/// </summary>
		public HotKey HotKey
		{
			get
			{
				return hotKey;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "HotKey cannot be null");
				}

				if (hotKey != value)
				{
					DestroyInputAction();

					hotKey = value;

					CreateHotKeyAction();

					NotifyPropertyChanged("HotKey");
				}
			}
		}

		bool visible = true;

		/// <summary>
		/// Is item visible?
		/// </summary>
		public bool Visible
		{
			get
			{
				return visible;
			}

			set
			{
				Change(ref visible, value, "Visible");
			}
		}

		bool interactable = true;

		/// <summary>
		/// Is item interactable?
		/// </summary>
		public bool Interactable
		{
			get
			{
				return interactable;
			}

			set
			{
				Change(ref interactable, value, "Interactable");
			}
		}

		object tag;

		/// <summary>
		/// Icon.
		/// </summary>
		public object Tag
		{
			get
			{
				return tag;
			}

			set
			{
				Change(ref tag, value, "Tag");
			}
		}

		ObservableList<MenuItem> items;

		/// <summary>
		/// Nested items.
		/// </summary>
		public ObservableList<MenuItem> Items
		{
			get
			{
				return items;
			}

			set
			{
				if (items == value)
				{
					return;
				}

				if (items != null)
				{
					items.OnCollectionChange -= ItemsChanged;
				}

				items = value;

				if (items != null)
				{
					items.OnCollectionChange += ItemsChanged;
				}

				NotifyPropertyChanged("Items");
			}
		}

		MenuItemAction action = new MenuItemAction();

		/// <summary>
		/// Action.
		/// </summary>
		public MenuItemAction Action
		{
			get
			{
				return action;
			}

			set
			{
				Change(ref action, value, "Action");
			}
		}

		/// <summary>
		/// Has visible items?
		/// </summary>
		public bool HasVisibleItems
		{
			get
			{
				if (Items == null)
				{
					return false;
				}

				for (int i = 0; i < Items.Count; i++)
				{
					if (Items[i].Visible)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Property changed event.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Property changed event.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuItem"/> class.
		/// </summary>
		public MenuItem()
		{
			Items = new ObservableList<MenuItem>();
		}

		/// <summary>
		/// Items changed.
		/// </summary>
		protected void ItemsChanged()
		{
			NotifyPropertyChanged("Items");
		}

		/// <summary>
		/// Change value.
		/// </summary>
		/// <typeparam name="T">Type of field.</typeparam>
		/// <param name="field">Field value.</param>
		/// <param name="value">New value.</param>
		/// <param name="propertyName">Property name.</param>
		protected void Change<T>(ref T field, T value, string propertyName)
		{
			if (!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				NotifyPropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// Property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void NotifyPropertyChanged(string propertyName)
		{
			OnChange?.Invoke();

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// HotKey action.
		/// </summary>
		protected InputAction HotKeyAction;
		#endif

		/// <summary>
		/// Is hot key enabled.
		/// Supported for the InputSystem only.
		/// </summary>
		public bool IsEnabledHotKey
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				if (HotKeyAction == null)
				{
					return false;
				}

				return HotKeyAction.enabled;
				#else
				return false;
				#endif
			}
		}

		/// <summary>
		/// Enable hot key.
		/// Supported for the InputSystem only.
		/// </summary>
		public void EnableHotKey()
		{
			#if ENABLE_INPUT_SYSTEM
			if (HotKeyAction == null)
			{
				CreateHotKeyAction();
			}

			HotKeyAction?.Enable();
			#endif
		}

		/// <summary>
		/// Disable hot key.
		/// Supported for the InputSystem only.
		/// </summary>
		public void DisableHotKey()
		{
			#if ENABLE_INPUT_SYSTEM
			if (HotKeyAction == null)
			{
				return;
			}

			HotKeyAction.Disable();
			#endif
		}

		/// <summary>
		/// Destroy current HotKey action.
		/// </summary>
		protected virtual void DestroyInputAction()
		{
			#if ENABLE_INPUT_SYSTEM
			if (HotKeyAction != null)
			{
				HotKeyAction.Disable();
				HotKeyAction.Dispose();
				HotKeyAction = null;
			}
			#endif
		}

		/// <summary>
		/// Create HotKey action.
		/// </summary>
		protected virtual void CreateHotKeyAction()
		{
			#if ENABLE_INPUT_SYSTEM
			if (!hotKey.Valid)
			{
				return;
			}

			HotKeyAction = new InputAction(Name);
			var group = CompatibilityInput.KeysGroup(hotKey.Key);

			if (hotKey.Modifiers > 0)
			{
				InputActionSetupExtensions.CompositeSyntax composite;

				if (hotKey.Modifiers == 3)
				{
					composite = HotKeyAction.AddCompositeBinding("CustomButtonWithThreeModifiers");

					composite = composite.With("Modifier1", "<Keyboard>/Ctrl");
					composite = composite.With("Modifier2", "<Keyboard>/Alt");
					composite = composite.With("Modifier3", "<Keyboard>/Shift");
				}
				else if (hotKey.Modifiers == 2)
				{
					composite = HotKeyAction.AddCompositeBinding("ButtonWithTwoModifiers");

					var index = 1;
					if (hotKey.Ctrl)
					{
						composite = composite.With(string.Format("Modifier{0}", index.ToString()), "<Keyboard>/Ctrl");
						index += 1;
					}

					if (hotKey.Alt)
					{
						composite = composite.With(string.Format("Modifier{0}", index.ToString()), "<Keyboard>/Alt");
						index += 1;
					}

					if (hotKey.Shift)
					{
						composite = composite.With(string.Format("Modifier{0}", index.ToString()), "<Keyboard>/Shift");
						index += 1;
					}
				}
				else
				{
					composite = HotKeyAction.AddCompositeBinding("ButtonWithOneModifier");

					if (hotKey.Ctrl)
					{
						composite = composite.With("Modifier", "<Keyboard>/Ctrl");
					}
					else if (hotKey.Alt)
					{
						composite = composite.With("Modifier", "<Keyboard>/Alt");
					}
					else if (hotKey.Shift)
					{
						composite = composite.With("Modifier", "<Keyboard>/Shift");
					}
				}

				for (int i = 0; i < group.Keys.Length; i++)
				{
					composite = composite.With("Button", string.Format("<Keyboard>/{0}", Key2String(group.Keys[i])));
				}
			}
			else
			{
				for (int i = 0; i < group.Keys.Length; i++)
				{
					HotKeyAction.AddBinding(string.Format("<Keyboard>/{0}", Key2String(group.Keys[i])));
				}
			}

			HotKeyAction.performed += InvokeAction;
			#endif
		}

#if ENABLE_INPUT_SYSTEM

		/// <summary>
		/// InputSystem key to string.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <returns>String.</returns>
		protected virtual string Key2String(Key key)
		{
			return key switch
			{
				Key.Digit0 => "0",
				Key.Digit1 => "1",
				Key.Digit2 => "2",
				Key.Digit3 => "3",
				Key.Digit4 => "4",
				Key.Digit5 => "5",
				Key.Digit6 => "6",
				Key.Digit7 => "7",
				Key.Digit8 => "8",
				Key.Digit9 => "9",
				_ => EnumHelper<Key>.ToString(key),
			};
		}

		/// <summary>
		/// Invoke action.
		/// </summary>
		/// <param name="context">Context.</param>
		protected virtual void InvokeAction(InputAction.CallbackContext context)
		{
			if (Visible && Interactable)
			{
				Action.Invoke(this);
			}
		}
#endif
	}
}