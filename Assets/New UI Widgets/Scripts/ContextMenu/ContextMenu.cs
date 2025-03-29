namespace UIWidgets.Menu
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.l10n;
	using UIWidgets.Pool;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
	using UnityEngine.InputSystem;
#endif
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Context menu.
	/// Contains menu items and reference to the menu template.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/New UI Widgets/ContextMenu")]
	[DataBindSupport]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/controls/contextmenu.html")]
	public partial class ContextMenu : UIBehaviourInteractable, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IStylable
		#if !ENABLE_INPUT_SYSTEM
		, IUpdatable
		#endif
	{
		/// <summary>
		/// Serialized menu item.
		/// </summary>
		[Serializable]
		protected class MenuItemSerialized
		{
			/// <summary>
			/// Depth.
			/// </summary>
			[SerializeField]
			public int Depth;

			/// <summary>
			/// Icon.
			/// </summary>
			[SerializeField]
			public Sprite Icon;

			/// <summary>
			/// Checked.
			/// </summary>
			[SerializeField]
			public bool Checked;

			/// <summary>
			/// Name.
			/// </summary>
			[SerializeField]
			public string Name;

			/// <summary>
			/// Template.
			/// </summary>
			[SerializeField]
			public string Template;

			/// <summary>
			/// Name.
			/// </summary>
			[SerializeField]
			public HotKey HotKey;

			/// <summary>
			/// Is item visible?
			/// </summary>
			[SerializeField]
			public bool Visible;

			/// <summary>
			/// Is item interactable.
			/// </summary>
			[SerializeField]
			public bool Interactable;

			/// <summary>
			/// For the editor use only.
			/// </summary>
			[SerializeField]
			[HideInInspector]
#pragma warning disable 0414
			bool showAction = false;
#pragma warning restore 0414

			/// <summary>
			/// Action.
			/// </summary>
			[SerializeField]
			public MenuItemAction Action = new MenuItemAction();

			/// <summary>
			/// Convert specified instance to the menu item.
			/// </summary>
			/// <param name="serialized">Serialized instance.</param>
			/// <returns>Menu item.</returns>
			public static MenuItem ToMenuItem(MenuItemSerialized serialized) => serialized;

			/// <summary>
			/// Convert specified instance to the menu item.
			/// </summary>
			/// <param name="serialized">Serialized instance.</param>
			public static implicit operator MenuItem(MenuItemSerialized serialized)
			{
				return new MenuItem()
				{
					Icon = serialized.Icon,
					Checked = serialized.Checked,
					Name = serialized.Name,
					Template = serialized.Template,
					HotKey = serialized.HotKey,
					Visible = serialized.Visible,
					Interactable = serialized.Interactable,
					Action = serialized.Action,
				};
			}
		}

		[SerializeField]
		ContextMenuView template;

		/// <summary>
		/// Menu template.
		/// </summary>
		public ContextMenuView Template
		{
			get => template;

			set
			{
				template = value;

				if (Instance != null)
				{
					Instance.Template = template;
				}
			}
		}

		/// <summary>
		/// Serialized menu items.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<MenuItemSerialized> MenuItemsSerialized = new List<MenuItemSerialized>();

		ObservableList<MenuItem> menuItems;

		/// <summary>
		/// Menu items.
		/// </summary>
		public ObservableList<MenuItem> MenuItems
		{
			get
			{
				if (menuItems == null)
				{
					menuItems = Deserialize(MenuItemsSerialized);
					menuItems.OnCollectionChangeMono.AddListener(UpdateMenuItems);
				}

				return menuItems;
			}

			set
			{
				if (menuItems != null)
				{
					menuItems.OnCollectionChangeMono.RemoveListener(UpdateMenuItems);
					DisableHotKeys(menuItems);
					Instance.ResetItems();
				}

				menuItems = value ?? throw new ArgumentNullException(nameof(value));
				menuItems.OnCollectionChangeMono.AddListener(UpdateMenuItems);

				UpdateMenuItems();
			}
		}

		/// <summary>
		/// Is default menu?
		/// </summary>
		[SerializeField]
		public bool IsDefault = false;

		/// <summary>
		/// Allow navigation.
		/// </summary>
		[SerializeField]
		public bool Navigation = true;

		/// <summary>
		/// Open menu on pointer right button click.
		/// </summary>
		[SerializeField]
		public bool OpenOnRightButtonClick = true;

		/// <summary>
		/// Open menu on context menu key.
		/// </summary>
		[SerializeField]
		public bool OpenOnContextMenuKey = true;

		/// <summary>
		/// Time to wait before open or close sub-menu.
		/// </summary>
		[SerializeField]
		public float SubmenuDelay = 0.3f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		[SerializeField]
		[FormerlySerializedAs("ParentCanvas")]
		RectTransform parentCanvas;

		/// <summary>
		/// Parent canvas.
		/// </summary>
		public RectTransform ParentCanvas
		{
			get => parentCanvas;

			set
			{
				parentCanvas = value;

				if (instance != null)
				{
					instance.ParentCanvas = value;
				}
			}
		}

		/// <summary>
		/// Event on menu open.
		/// </summary>
		[SerializeField]
		[DataBindEvent]
		public MenuEvent OnOpen = new MenuEvent();

		/// <summary>
		/// Event on menu close.
		/// </summary>
		[SerializeField]
		[DataBindEvent]
		public MenuEvent OnClose = new MenuEvent();

		/// <summary>
		/// Event on pointer over menu item.
		/// </summary>
		[SerializeField]
		[DataBindEvent]
		public MenuItemEvent OnItemSelect = new MenuItemEvent();

		/// <summary>
		/// Event on pointer out menu item.
		/// </summary>
		[SerializeField]
		[DataBindEvent]
		public MenuItemEvent OnItemDeselect = new MenuItemEvent();

		MenuInstance instance;

		/// <summary>
		/// Menu instance.
		/// </summary>
		protected MenuInstance Instance
		{
			get
			{
				if (instance == null)
				{
					if (ParentCanvas == null)
					{
						ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
					}

					instance = new MenuInstance(this, Template, MenuItems.AsReadOnly(), ParentCanvas);
				}

				return instance;
			}
		}

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Active menu.
		/// </summary>
		protected static readonly List<ContextMenu> ActiveMenu = new List<ContextMenu>();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}

			Template.Init();

			CreateToggleAction();

			UpdateMenuItems();
		}

		bool localeSubscription;

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (!localeSubscription)
			{
				Init();

				localeSubscription = true;
				Localization.OnLocaleChanged += LocaleChanged;
				LocaleChanged();
			}

			if (IsDefault)
			{
				for (int i = 0; i < ActiveMenu.Count; i++)
				{
					ActiveMenu[i].IsDefault = false;
				}
			}

			ActiveMenu.Add(this);

			#if !ENABLE_INPUT_SYSTEM
			Updater.Add(this);
			#endif
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			ActiveMenu.Remove(this);

			#if !ENABLE_INPUT_SYSTEM
			Updater.Remove(this);
			#endif
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (menuItems != null)
			{
				DisableHotKeys(menuItems);
				Instance.ResetItems();

				menuItems.OnCollectionChangeMono.RemoveListener(UpdateMenuItems);
				menuItems = null;
			}

			Localization.OnLocaleChanged -= LocaleChanged;
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			instance?.LocaleChanged();
		}

		/// <summary>
		/// Update menu items.
		/// </summary>
		protected void UpdateMenuItems()
		{
			EnableHotKeys(MenuItems);

			Instance.UpdateItems(MenuItems.AsReadOnly());
		}

		/// <summary>
		/// Enable hot keys.
		/// Supported for the InputSystem only.
		/// </summary>
		/// <param name="items">Items.</param>
		protected virtual void EnableHotKeys(ObservableList<MenuItem> items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].EnableHotKey();

				if (items[i].Items != null)
				{
					EnableHotKeys(items[i].Items);
				}
			}
		}

		/// <summary>
		/// Disable hot keys.
		/// Supported for the InputSystem only.
		/// </summary>
		/// <param name="items">Items.</param>
		protected virtual void DisableHotKeys(ObservableList<MenuItem> items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].DisableHotKey();

				if (items[i].Items != null)
				{
					DisableHotKeys(items[i].Items);
				}
			}
		}

		/// <summary>
		/// Enable hot keys.
		/// Supported for the InputSystem only.
		/// </summary>
		public virtual void EnableHotKeys()
		{
			EnableHotKeys(MenuItems);
		}

		/// <summary>
		/// Disable hot keys.
		/// Supported for the InputSystem only.
		/// </summary>
		public virtual void DisableHotKeys()
		{
			EnableHotKeys(MenuItems);
		}

		/// <summary>
		/// Open.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void Open(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentCanvas, eventData.position, eventData.pressEventCamera, out var position);
			var size = ParentCanvas.rect.size;
			var pivot = ParentCanvas.pivot;

			position.x += size.x * pivot.x;
			position.y -= size.y * pivot.y;

			Open(position);
		}

		/// <summary>
		/// Open menu in the specified position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void Open(Vector2 position)
		{
			Instance.Open(position);

			CurrentMenu = this;
		}

		/// <summary>
		/// Close menu.
		/// </summary>
		public virtual void Close()
		{
			Instance.Close();

			CurrentMenu = null;
		}

		/// <summary>
		/// Get default position.
		/// </summary>
		/// <returns>Position.</returns>
		protected virtual Vector2 GetDefaultPosition()
		{
			var position = UtilitiesRectTransform.GetTopLeftCornerGlobalPosition(RectTransform, ParentCanvas);
			if (CompatibilityInput.MousePresent)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, CompatibilityInput.MousePosition, Camera.main, out var delta);

				var size = RectTransform.rect.size;
				var pivot = RectTransform.pivot;
				delta += new Vector2(size.x * pivot.x, size.y * (pivot.y - 1f));

				return position + delta;
			}

			return position;
		}

		/// <summary>
		/// Toggle menu.
		/// </summary>
		protected virtual void Toggle()
		{
			if (Instance.IsOpened)
			{
				CurrentMenu = null;
				Instance.Close();
			}
			else
			{
				CurrentMenu = this;
				Instance.Open(GetDefaultPosition());
				Instance.SelectFirst();
			}
		}

		/// <summary>
		/// Process the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			// required by OnPointerClick
		}

		/// <summary>
		/// Process the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			// required by OnPointerClick
		}

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (!IsActive() || !OpenOnRightButtonClick)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Right)
			{
				Open(eventData);
			}
		}

		/// <summary>
		/// Deserialize menu items.
		/// </summary>
		/// <param name="serialized">Serializes items.</param>
		/// <returns>Items.</returns>
		protected static ObservableList<MenuItem> Deserialize(List<MenuItemSerialized> serialized)
		{
			var result = new ObservableList<MenuItem>();

			Stack<MenuItem> current_branch = new Stack<MenuItem>();
			var prev_depth = 0;
			for (int i = 0; i < serialized.Count; i++)
			{
				var current = serialized[i];

				if (current.Depth == 0)
				{
					current_branch.Clear();
					current_branch.Push(current);
					result.Add(current_branch.Peek());
				}
				else if (current.Depth == (prev_depth + 1))
				{
					MenuItem item = current;
					current_branch.Peek().Items.Add(item);
					current_branch.Push(item);
				}
				else if (current.Depth <= prev_depth)
				{
					var n = prev_depth + 1 - current.Depth;

					for (int j = 0; j < n; j++)
					{
						current_branch.Pop();
					}

					MenuItem item = current;
					current_branch.Peek().Items.Add(item);
					current_branch.Push(item);
				}
				else
				{
					// Debug.LogWarning("Unknown case");
				}

				prev_depth = current.Depth;
			}

			return result;
		}

		/// <summary>
		/// Is current menu active?
		/// </summary>
		/// <returns>true if current menu active; otherwise false.</returns>
		protected virtual bool IsActiveMenu()
		{
			if (EventSystem.current.currentSelectedGameObject == null)
			{
				return false;
			}

			var t = EventSystem.current.currentSelectedGameObject.transform;

			return (t == transform) || t.IsChildOf(transform);
		}

		/// <summary>
		/// Last frame when toggle was called.
		/// </summary>
		protected static int LastToggledByKey = -2;

		/// <summary>
		/// Current menu.
		/// </summary>
		protected static ContextMenu CurrentMenu;

		/// <summary>
		/// Toggle active menu.
		/// </summary>
		protected static void ToggleActiveMenuByKey()
		{
			if (LastToggledByKey == UtilitiesTime.GetFrameCount())
			{
				return;
			}

			if ((CurrentMenu != null) && CurrentMenu.OpenOnContextMenuKey && CurrentMenu.Instance.IsOpened)
			{
				CurrentMenu.Toggle();
			}
			else
			{
				var menu = FindActiveMenu();
				if (menu != null)
				{
					menu.Toggle();
				}
			}

			LastToggledByKey = UtilitiesTime.GetFrameCount();
		}

		/// <summary>
		/// Check if RectTransform contains screen point.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <param name="camera">Camera.</param>
		/// <returns>true if RectTransform contains screen point; otherwise false.</returns>
		protected virtual bool ContainsScreenPoint(Vector2 point, Camera camera)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, point, camera, out var position);
			return RectTransform.rect.Contains(position);
		}

		/// <summary>
		/// Is menu open.
		/// </summary>
		[DomainReloadExclude]
		protected static readonly Predicate<ContextMenu> IsOpenMenu = menu => menu.IsActiveMenu() && menu.OpenOnContextMenuKey;

		/// <summary>
		/// Is default menu.
		/// </summary>
		[DomainReloadExclude]
		protected static readonly Predicate<ContextMenu> IsDefaultMenu = menu => menu.OpenOnContextMenuKey && menu.gameObject.activeInHierarchy && menu.IsDefault;

		/// <summary>
		/// Find menu.
		/// </summary>
		/// <param name="predicate">Predicate.</param>
		/// <returns>true if menu matches predicate; otherwise false.</returns>
		protected static ContextMenu FindMenu(Predicate<ContextMenu> predicate)
		{
			using var _ = ListPool<ContextMenu>.Get(out var nested);

			foreach (var active in ActiveMenu)
			{
				if (predicate(active))
				{
					nested.Add(active);
				}
			}

			if (nested.Count == 0)
			{
				return null;
			}

			StableSort.Sort(nested, MenuComparison);

			return nested[0];
		}

		/// <summary>
		/// Find active menu.
		/// </summary>
		/// <returns>Active menu.</returns>
		protected static ContextMenu FindActiveMenu()
		{
			var menu = FindMenu(IsOpenMenu);
			if (menu != null)
			{
				return menu;
			}

			var camera = Camera.main;
			var cursor = CompatibilityInput.MousePosition;
			var position = Display.RelativeMouseAt(cursor);
			if (position == Vector3.zero)
			{
				position = cursor;
			}

			bool UnderCursor(ContextMenu x) => x.OpenOnContextMenuKey && x.gameObject.activeInHierarchy && x.ContainsScreenPoint(position, camera);

			menu = FindMenu(UnderCursor);
			if (menu != null)
			{
				return menu;
			}

			return FindMenu(IsDefaultMenu);
		}

		/// <summary>
		/// Menu comparison.
		/// </summary>
		[DomainReloadExclude]
		protected static readonly Comparison<ContextMenu> MenuComparison = (x, y) =>
		{
			var x_depth = Utilities.GetDepth(x.transform);
			var y_depth = Utilities.GetDepth(y.transform);

			return -x_depth.CompareTo(y_depth);
		};

		#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(ActiveMenu), nameof(LastToggledByKey), nameof(CurrentMenu), "ToggleAction")]
		static void StaticInit()
		{
			ActiveMenu.Clear();
			LastToggledByKey = -2;
			CurrentMenu = null;

			#if ENABLE_INPUT_SYSTEM
			ToggleAction = null;
			#endif
		}
		#endif

		#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// Input action.
		/// </summary>
		protected static InputAction ToggleAction;

		/// <summary>
		/// Create toggle action.
		/// </summary>
		protected virtual void CreateToggleAction()
		{
			if (ToggleAction == null)
			{
				ToggleAction = new InputAction(name);
				ToggleAction.AddBinding(string.Format("<Keyboard>/{0}", EnumHelper<Key>.ToString(Key.ContextMenu)));
				ToggleAction.performed += Toggle;
				ToggleAction.Enable();
			}
		}

		/// <summary>
		/// Toggle active menu.
		/// </summary>
		/// <param name="context">Context.</param>
		protected static void Toggle(InputAction.CallbackContext context)
		{
			ToggleActiveMenuByKey();
		}
		#else
		/// <summary>
		/// Create toggle action.
		/// </summary>
		protected virtual void CreateToggleAction()
		{
		}

		/// <summary>
		/// Process the update event.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (CompatibilityInput.ContextMenuUp)
			{
				ToggleActiveMenuByKey();
			}

			CheckHotKeys(MenuItems);
		}

		/// <summary>
		/// Check hot keys status.
		/// </summary>
		/// <param name="items">Items.</param>
		protected virtual void CheckHotKeys(ObservableList<MenuItem> items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];

				if (item.Visible && item.Interactable && item.HotKey.IsUp)
				{
					item.Action.Invoke(item);
					Close();
				}

				if (item.Items != null)
				{
					CheckHotKeys(item.Items);
				}
			}
		}
		#endif

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected override void Reset()
		{
			base.Reset();

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}
		#endif

		#region IStyleable

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			Template.SetStyle(style);

			instance?.SetStyle(style);

			return false;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			Template.GetStyle(style);

			return false;
		}
		#endregion
	}
}