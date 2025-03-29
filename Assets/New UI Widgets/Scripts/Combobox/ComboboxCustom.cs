namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for custom combobox.
	/// </summary>
	/// <typeparam name="TListViewCustom">Type of ListView.</typeparam>
	/// <typeparam name="TItemView">Type of ListView component.</typeparam>
	/// <typeparam name="TItem">Type of ListView item.</typeparam>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/combobox.html")]
	public class ComboboxCustom<TListViewCustom, TItemView, TItem> : ComboboxBase
			where TListViewCustom : ListViewCustom<TItemView, TItem>
			where TItemView : ListViewItem
	{
		/// <summary>
		/// Position changes.
		/// </summary>
		protected readonly struct PositionChanges
		{
			/// <summary>
			/// Object was moved horizontally.
			/// </summary>
			public readonly bool Horizontal;

			/// <summary>
			/// Object was moved vertically.
			/// </summary>
			public readonly bool Vertical;

			/// <summary>
			/// Initializes a new instance of the <see cref="PositionChanges"/> struct.
			/// </summary>
			/// <param name="horizontal">Object was moved horizontally.</param>
			/// <param name="vertical">Object was moved vertically.</param>
			public PositionChanges(bool horizontal, bool vertical)
			{
				Horizontal = horizontal;
				Vertical = vertical;
			}
		}

		/// <summary>
		/// Custom Combobox event.
		/// </summary>
		[Serializable]
		public class ComboboxCustomEvent : UnityEvent<int, TItem>
		{
		}

		[SerializeField]
		TListViewCustom listView;

		/// <summary>
		/// Gets or sets the ListView.
		/// </summary>
		/// <value>ListView component.</value>
		public TListViewCustom ListView
		{
			get => listView;

			set
			{
				ListViewUnsubscribe();
				listView = value;
				ListViewSubscribe();

				ListViewRectTransform = listView.transform as RectTransform;
				if (listView.TryGetComponent(out ListViewCorners))
				{
					ListViewRadius = ListViewCorners.Radius;
				}
			}
		}

		/// <summary>
		/// If enabled ListView is automatically positioned to be completely visible if partially hidden by the bottom or right side of the screen.
		/// </summary>
		[SerializeField]
		[EditorConditionObjectNotNull(nameof(listView))]
		[Tooltip("If enabled ListView is automatically positioned to be completely visible if partially hidden by the bottom or right side of the screen.")]
		public bool RepositionListView = true;

		/// <summary>
		/// Corner radiuses will be changed to match the repositioned ListView.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(RepositionListView))]
		[Tooltip("If enabled corner radiuses will be changed to match the repositioned ListView.")]
		public bool ChangeRounderCorners = true;

		[SerializeField]
		Button toggleButton;

		/// <summary>
		/// Gets or sets the toggle button.
		/// </summary>
		/// <value>The toggle button.</value>
		public Button ToggleButton
		{
			get => toggleButton;

			set => SetToggleButton(value);
		}

		[SerializeField]
		TItemView current;

		/// <summary>
		/// Gets or sets the current component.
		/// </summary>
		/// <value>The current.</value>
		public TItemView Current
		{
			get => current;

			set => SetCurrent(value);
		}

		/// <summary>
		/// Widget root.
		/// </summary>
		[SerializeField]
		protected RectTransform Root;

		/// <summary>
		/// Is Current implements IViewData{TItem}.
		/// </summary>
		protected bool CanSetData;

		/// <summary>
		/// Parent canvas.
		/// </summary>
		[SerializeField]
		public RectTransform ParentCanvas;

		/// <summary>
		/// Root corners.
		/// </summary>
		protected RoundedCornersX4 RootCorners;

		/// <summary>
		/// Root corners radius.
		/// </summary>
		protected RoundedCornersX4.BorderRadius RootRadius;

		/// <summary>
		/// ListView RectTransform.
		/// </summary>
		protected RectTransform ListViewRectTransform;

		/// <summary>
		/// ListView corners.
		/// </summary>
		protected RoundedCornersX4 ListViewCorners;

		/// <summary>
		/// ListView corners radius.
		/// </summary>
		protected RoundedCornersX4.BorderRadius ListViewRadius;

		/// <summary>
		/// Combobox original position.
		/// </summary>
		protected HierarchyPosition ComboboxPosition;

		/// <summary>
		/// ListView original position;
		/// </summary>
		protected Vector2 ListViewPosition;

		/// <summary>
		/// Canvas resize subscription.
		/// </summary>
		protected Subscription CanvasResize;

		/// <summary>
		/// The components list.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TItemView> components = new List<TItemView>();

		/// <summary>
		/// The components cache list.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TItemView> componentsCache = new List<TItemView>();

		/// <summary>
		/// Hide ListView on item select or deselect.
		/// </summary>
		[SerializeField]
		[Tooltip("Hide ListView on item select or deselect.")]
		public bool HideAfterItemToggle = true;

		/// <summary>
		/// Hide ListView on Combobox submit.
		/// </summary>
		[SerializeField]
		public bool ShowListViewOnSubmit = true;

		/// <summary>
		/// OnSelect event.
		/// </summary>
		[Obsolete("Use Combobox.ListView.OnSelect instead.")]
		public ComboboxCustomEvent OnSelect = new ComboboxCustomEvent();

		/// <summary>
		/// Raised when ListView opened.
		/// </summary>
		[SerializeField]
		public UnityEvent OnShowListView = new UnityEvent();

		/// <summary>
		/// Raised when ListView closed.
		/// </summary>
		[SerializeField]
		public UnityEvent OnHideListView = new UnityEvent();

		/// <summary>
		/// Raised when click on current.
		/// </summary>
		[SerializeField]
		public ComboboxCustomEvent OnCurrentClick = new ComboboxCustomEvent();

		/// <summary>
		/// The modal ID.
		/// </summary>
		protected InstanceID? ModalKey;

		/// <summary>
		/// ListView is displayed and active.
		/// </summary>
		protected bool ListViewActive;

		/// <summary>
		/// Placeholder.
		/// Replaces combobox position when ListView displayed if combobox under layout group control.
		/// </summary>
		[NonSerialized]
		protected LayoutPlaceholder Placeholder;

		/// <summary>
		/// Data source.
		/// </summary>
		public ObservableList<TItem> DataSource
		{
			get => ListView.DataSource;
			set => ListView.DataSource = value;
		}

		/// <summary>
		/// Index of the last selected item.
		/// </summary>
		public int SelectedIndex
		{
			get => ListView.SelectedIndex;
			set => ListView.SelectedIndex = value;
		}

		/// <summary>
		/// Indices of the selected items.
		/// </summary>
		public List<int> SelectedIndices
		{
			get => ListView.SelectedIndices;
			set => ListView.SelectedIndices = value;
		}

		/// <summary>
		/// Selected item.
		/// </summary>
		public TItem SelectedItem => ListView.SelectedItem;

		/// <summary>
		/// Selected items.
		/// </summary>
		public List<TItem> SelectedItems => ListView.SelectedItems;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			CanSetData = typeof(IViewData<TItem>).IsAssignableFrom(typeof(TItemView));

			SetToggleButton(toggleButton);

			ListViewSubscribe();

			Current.SetThemeImagesPropertiesOwner(this);

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform as RectTransform);
			}

			if (Root == null)
			{
				Root = transform.parent as RectTransform;
			}

			if (Root.TryGetComponent(out RootCorners))
			{
				RootRadius = RootCorners.Radius;
			}

			if (listView != null)
			{
				ListViewRectTransform = ListView.transform as RectTransform;
				if (ListView.TryGetComponent(out ListViewCorners))
				{
					ListViewRadius = ListViewCorners.Radius;
				}

				Current.Owner = ListView;
				Current.ComboboxOwner = this;

				current.gameObject.SetActive(false);

				listView.OnSelectInternal.RemoveListener(UpdateView);
				listView.OnDeselectInternal.RemoveListener(UpdateView);
				listView.OnUpdateView.RemoveListener(UpdateViewBase);

				listView.gameObject.SetActive(true);
				listView.Init();
				if ((listView.SelectedIndex == -1) && (listView.DataSource.Count > 0) && (!listView.MultipleSelect))
				{
					listView.SelectedIndex = 0;
				}

				if (listView.SelectedIndex != -1)
				{
					UpdateViewBase();
				}

				InitCustomWidgets();

				listView.gameObject.SetActive(false);

				listView.OnSelectInternal.AddListener(UpdateView);
				listView.OnDeselectInternal.AddListener(UpdateView);
				listView.OnUpdateView.AddListener(UpdateViewBase);
			}
		}

		/// <summary>
		/// Refresh positions on canvas resize.
		/// </summary>
		protected virtual void RefreshPositions()
		{
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

			ComboboxPosition.Refresh();
		}

		/// <summary>
		/// Init custom widgets.
		/// </summary>
		protected virtual void InitCustomWidgets()
		{
		}

		bool localeSubscription;

		/// <inheritdoc/>
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
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			Current.LocaleChanged();

			for (int i = 0; i < components.Count; i++)
			{
				components[i].LocaleChanged();
			}
		}

		/// <summary>
		/// Set new component to display current item.
		/// </summary>
		/// <param name="newCurrent">New component.</param>
		protected virtual void SetCurrent(TItemView newCurrent)
		{
			foreach (var c in components)
			{
				DeactivateComponent(c);
				Destroy(c);
			}

			components.Clear();

			foreach (var c in componentsCache)
			{
				Destroy(c);
			}

			componentsCache.Clear();

			current = newCurrent;
			current.SetThemeImagesPropertiesOwner(this);
			current.gameObject.SetActive(false);
		}

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			base.OnInteractableChange(interactableState);

			SetViewInteractable(interactableState);

			HideList();
		}

		/// <summary>
		/// Set view interactable.
		/// </summary>
		/// <param name="interactableState">Interactable state.</param>
		protected virtual void SetViewInteractable(bool interactableState)
		{
			foreach (var component in components)
			{
				if (interactableState)
				{
					component.StateDefault();
					component.GraphicsInteractableState(Color.white);
				}
				else
				{
					component.StateDisabled();
					component.GraphicsInteractableState(ListView.DisabledColor);
				}
			}
		}

		/// <summary>
		/// Sets the toggle button.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetToggleButton(Button value)
		{
			if (toggleButton != null)
			{
				toggleButton.onClick.RemoveListener(ToggleList);
			}

			toggleButton = value;

			if (toggleButton != null)
			{
				toggleButton.onClick.AddListener(ToggleList);
			}
		}

		/// <summary>
		/// Subscribe on ListView events.
		/// </summary>
		protected virtual void ListViewSubscribe()
		{
			if (listView == null)
			{
				return;
			}

			listView.KeepHighlight = false;

			listView.OnSelectInternal.AddListener(UpdateView);
			listView.OnDeselectInternal.AddListener(UpdateView);
			listView.OnUpdateView.AddListener(UpdateViewBase);

			listView.OnFocusOut.AddListener(OnFocusHideList);

			listView.onCancel.AddListener(OnListViewCancel);
			listView.InstancesEventsInternal.Cancel.AddListener(OnListViewCancel);
			listView.InstancesEventsInternal.PointerUp.AddListener(OnListViewPointerUp);

			AddDeselectCallbacks();
		}

		/// <summary>
		/// Unsubscribe off ListView events.
		/// </summary>
		/// <param name="restorePosition">Restore position.</param>
		protected virtual void ListViewUnsubscribe(bool restorePosition = true)
		{
			if (listView == null)
			{
				return;
			}

			listView.OnSelectInternal.RemoveListener(UpdateView);
			listView.OnDeselectInternal.RemoveListener(UpdateView);
			listView.OnUpdateView.RemoveListener(UpdateViewBase);

			listView.OnFocusOut.RemoveListener(OnFocusHideList);

			listView.onCancel.RemoveListener(OnListViewCancel);
			listView.InstancesEventsInternal.Cancel.RemoveListener(OnListViewCancel);
			listView.InstancesEventsInternal.PointerUp.RemoveListener(OnListViewPointerUp);

			RemoveDeselectCallbacks();
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public virtual int Set(TItem item, bool allowDuplicate = true)
		{
			return listView.Set(item, allowDuplicate);
		}

		/// <summary>
		/// Clear ListView and selected item.
		/// </summary>
		public virtual void Clear()
		{
			listView.DataSource.Clear();
			UpdateView();
		}

		/// <summary>
		/// Toggles the list visibility.
		/// </summary>
		public virtual void ToggleList()
		{
			if (listView == null)
			{
				return;
			}

			if (listView.gameObject.activeSelf)
			{
				HideList();
			}
			else
			{
				ShowList();
			}
		}

		/// <summary>
		/// Show current values.
		/// </summary>
		public virtual void ShowCurrent()
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Hide current values.
		/// </summary>
		public virtual void HideCurrent()
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Process click on current.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void CurrentClick(ListViewItem item)
		{
			OnCurrentClick.Invoke(item.Index, ListView.DataSource[item.Index]);
		}

		/// <summary>
		/// Shows the list.
		/// </summary>
		public virtual void ShowList()
		{
			if (ListViewActive || listView == null)
			{
				return;
			}

			ModalKey = ModalHelper.Open(this, null, new Color(0, 0, 0, 0f), HideList, ParentCanvas);

			if (ParentCanvas != null)
			{
				if (Root.parent.TryGetComponent<LayoutGroup>(out var _))
				{
					if (Placeholder == null)
					{
						Placeholder = LayoutPlaceholder.Create(Root);
					}

					Placeholder.Show();
				}

				ComboboxPosition = HierarchyPosition.SetParent(Root, ParentCanvas);

				var resize = Utilities.RequireComponent<ResizeListener>(ParentCanvas);
				CanvasResize.Clear();
				CanvasResize = new Subscription(resize.OnResize, RefreshPositions);
			}

			listView.gameObject.SetActive(true);

			// prevent instant close of the ListView on first open, happens because of select/deselect events
			if (!listView.gameObject.activeSelf)
			{
				listView.gameObject.SetActive(true);
			}

			if (listView.Layout != null)
			{
				listView.Layout.UpdateLayout();
			}

			var changes = EnsureVisiblePosition();
			UpdateCorners(changes);

			listView.ScrollToPosition(listView.GetScrollPosition());
			if (listView.SelectComponent())
			{
				SetChildDeselectListener(EventSystem.current.currentSelectedGameObject);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(listView.gameObject);
			}

			// should be at the end to prevent close on select/deselect cause be ListView.OnEnable() locale changes
			ListViewActive = true;

			OnShowListView.Invoke();
		}

		/// <summary>
		/// Update rounded corners.
		/// </summary>
		/// <param name="changes">Position changes.</param>
		protected virtual void UpdateCorners(PositionChanges changes)
		{
			if (!RepositionListView || !ChangeRounderCorners)
			{
				return;
			}

			if (RootCorners != null)
			{
				var max_top = Mathf.Max(RootCorners.Radius.TopLeft, RootCorners.Radius.TopRight);
				var max_bottom = Mathf.Max(RootCorners.Radius.BottomLeft, RootCorners.Radius.BottomRight);
				var max = Mathf.Max(max_top, max_bottom);

				var radius = default(RoundedCornersX4.BorderRadius);
				if (changes.Vertical)
				{
					radius.BottomLeft = max;
					radius.BottomRight = max;
				}
				else
				{
					radius.TopLeft = max;
					radius.TopRight = max;
				}

				RootCorners.Radius = radius;
			}

			if (ListViewCorners != null)
			{
				var max_top = Mathf.Max(ListViewCorners.Radius.TopLeft, ListViewCorners.Radius.TopRight);
				var max_bottom = Mathf.Max(ListViewCorners.Radius.BottomLeft, ListViewCorners.Radius.BottomRight);
				var max = Mathf.Max(max_top, max_bottom);

				var radius = default(RoundedCornersX4.BorderRadius);
				if (changes.Vertical || changes.Horizontal)
				{
					radius.TopLeft = max;
					radius.TopRight = max;
				}

				if (!changes.Vertical || changes.Horizontal)
				{
					radius.BottomLeft = max;
					radius.BottomRight = max;
				}

				ListViewCorners.Radius = radius;
			}
		}

		/// <summary>
		/// Restore corners.
		/// </summary>
		protected virtual void RestoreCorners()
		{
			if (!RepositionListView || !ChangeRounderCorners)
			{
				return;
			}

			if (RootCorners != null)
			{
				RootCorners.Radius = RootRadius;
			}

			if (ListViewCorners != null)
			{
				ListViewCorners.Radius = ListViewRadius;
			}
		}

		/// <summary>
		/// Get target position relative to the canvas.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="canvasID">Canvas ID.</param>
		/// <returns>Position.</returns>
		protected virtual Vector3 CanvasRelativePosition(Transform target, int canvasID)
		{
			var pos = target.localPosition;

			while (true)
			{
				target = target.parent;

				if ((target == null) || (target.GetInstanceID() == canvasID))
				{
					break;
				}

				pos += target.localPosition;
			}

			return pos;
		}

		/// <summary>
		/// Ensure ListView is visible (will be moved to top or left if partially invisible).
		/// </summary>
		/// <returns>Position changes.</returns>
		protected virtual PositionChanges EnsureVisiblePosition()
		{
			if (!RepositionListView)
			{
				return new PositionChanges(false, false);
			}

			ListViewPosition = ListViewRectTransform.anchoredPosition;

			var base_position = (Vector2)CanvasRelativePosition(ListViewRectTransform, ParentCanvas.GetInstanceID());
			var position = base_position;

			var canvas_size = ParentCanvas.rect.size;
			var pivot = ParentCanvas.pivot;
			var offset = new Vector2(canvas_size.x * pivot.x, -canvas_size.y * pivot.y);
			position += offset;

			var width = ListViewRectTransform.rect.width;
			var height = ListViewRectTransform.rect.height;

			var overflow_horizontal = (position.x + width) > canvas_size.x;
			if (overflow_horizontal)
			{
				position.x = canvas_size.x - width;
			}

			var overflow_vertical = (-(position.y - height)) > canvas_size.y;
			if (overflow_vertical)
			{
				position.y += height + Root.rect.height - 1;
			}

			ListViewRectTransform.anchoredPosition += position - base_position - offset;

			return new PositionChanges(overflow_horizontal, overflow_vertical);
		}

		/// <summary>
		/// Restore position.
		/// </summary>
		protected virtual void RestorePosition()
		{
			if (!RepositionListView)
			{
				return;
			}

			ListViewRectTransform.anchoredPosition = ListViewPosition;
		}

		/// <summary>
		/// Hides the list.
		/// </summary>
		public virtual void HideList()
		{
			if (!ListViewActive)
			{
				return;
			}

			ListViewActive = false;

			RestorePosition();
			RestoreCorners();
			ModalHelper.Close(ref ModalKey);

			if (!Utilities.IsNull(Placeholder))
			{
				Placeholder.Hide();
			}

			ComboboxPosition.Restore();
			CanvasResize.Clear();

			if (!Utilities.IsNull(listView))
			{
				listView.gameObject.SetActive(false);
			}

			if (!Utilities.IsNull(toggleButton))
			{
				toggleButton.transform.SetAsLastSibling();
			}

			OnHideListView.Invoke();

			var evs = EventSystem.current;
			if ((evs != null)
				&& (ToggleButton != null)
				&& (evs.currentSelectedGameObject != ToggleButton.gameObject))
			{
				Updater.RunOnceNextFrame(ToggleButton, () => EventSystem.current.SetSelectedGameObject(ToggleButton.gameObject));
			}
		}

		/// <summary>
		/// The children deselect.
		/// </summary>
		protected List<SelectListener> childrenDeselect = new List<SelectListener>();

		/// <summary>
		/// Hide list when focus lost.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnFocusHideList(BaseEventData eventData)
		{
			if (eventData.selectedObject == gameObject)
			{
				return;
			}

			if (eventData is ListViewItemEventData ev_item)
			{
				if (ev_item.NewSelectedObject != null)
				{
					SetChildDeselectListener(ev_item.NewSelectedObject);
				}

				return;
			}

			if ((eventData is AxisEventData) && ListView.Navigation)
			{
				return;
			}

			if (!(eventData is PointerEventData ev_pointer))
			{
				HideList();
				return;
			}

			var go = ev_pointer.pointerPressRaycast.gameObject;
			if (go == null)
			{
				HideList();
				return;
			}

			if (go.Equals(toggleButton.gameObject) || go.transform.IsChildOf(toggleButton.transform) || go.transform.IsChildOf(current.RectTransform.parent))
			{
				return;
			}

			if (go.transform.IsChildOf(listView.transform))
			{
				SetChildDeselectListener(go);
				return;
			}

			HideList();
		}

		/// <summary>
		/// Sets the child deselect listener.
		/// </summary>
		/// <param name="child">Child.</param>
		protected void SetChildDeselectListener(GameObject child)
		{
			var deselectListener = Utilities.RequireComponent<SelectListener>(child);
			if (!childrenDeselect.Contains(deselectListener))
			{
				deselectListener.onDeselect.AddListener(OnFocusHideList);
				childrenDeselect.Add(deselectListener);
			}
		}

		/// <summary>
		/// Adds the deselect callbacks.
		/// </summary>
		protected void AddDeselectCallbacks()
		{
			if (listView.ScrollRect == null)
			{
				return;
			}

			if (listView.ScrollRect.verticalScrollbar == null)
			{
				return;
			}

			var scrollbar = listView.ScrollRect.verticalScrollbar.gameObject;
			var deselectListener = Utilities.RequireComponent<SelectListener>(scrollbar);

			deselectListener.onDeselect.AddListener(OnFocusHideList);
			childrenDeselect.Add(deselectListener);
		}

		/// <summary>
		/// Removes the deselect callbacks.
		/// </summary>
		protected void RemoveDeselectCallbacks()
		{
			foreach (var c in childrenDeselect)
			{
				RemoveDeselectCallback(c);
			}

			childrenDeselect.Clear();
		}

		/// <summary>
		/// Removes the deselect callback.
		/// </summary>
		/// <param name="listener">Listener.</param>
		protected void RemoveDeselectCallback(SelectListener listener)
		{
			if (listener != null)
			{
				listener.onDeselect.RemoveListener(OnFocusHideList);
			}
		}

		void UpdateView(int index) => UpdateView();

		/// <summary>
		/// The current indices.
		/// </summary>
		protected List<int> currentIndices = new List<int>();

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateViewBase()
		{
			currentIndices.Clear();
			ListView.GetSelectedIndices(currentIndices);

			UpdateComponentsCount();

			for (int i = 0; i < components.Count; i++)
			{
				SetData(components[i], i);
			}

			SetViewInteractable(IsInteractable());
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			UpdateViewBase();

			if (HideAfterItemToggle)
			{
				HideList();
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="i">The index.</param>
		protected virtual void SetData(TItemView component, int i)
		{
			component.Index = currentIndices[i];
			SetData(component, ListView.DataSource[currentIndices[i]]);
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected virtual void SetData(TItemView component, TItem item)
		{
			if (CanSetData)
			{
				(component as IViewData<TItem>).SetData(item);
			}
		}

		/// <summary>
		/// Hide list view.
		/// </summary>
		void OnListViewPointerUp(int index, ListViewItem item, BaseEventData eventData)
		{
			if (index == ListView.SelectedIndex && !ListView.MultipleSelect)
			{
				HideList();
			}
		}

		/// <summary>
		/// Hide list view.
		/// </summary>
		void OnListViewCancel(int index, ListViewItem item, BaseEventData eventData)
		{
			HideList();
		}

		void OnListViewCancel()
		{
			HideList();
		}

		/// <summary>
		/// Adds the component.
		/// </summary>
		protected virtual void AddComponent()
		{
			TItemView component;
			if (componentsCache.Count > 0)
			{
				component = componentsCache[componentsCache.Count - 1];
				componentsCache.RemoveAt(componentsCache.Count - 1);
			}
			else
			{
				component = Compatibility.Instantiate(current);
				component.SetThemeImagesPropertiesOwner(this);
				component.RectTransform.SetParent(current.RectTransform.parent, false);

				Utilities.FixInstantiated(current.RectTransform, component.RectTransform);
			}

			component.Index = -2;
			component.RectTransform.SetAsLastSibling();
			component.gameObject.SetActive(true);
			component.onClickItem.AddListener(CurrentClick);
			component.Owner = ListView;
			component.ComboboxOwner = this;
			components.Add(component);

			ToggleButton.transform.SetAsLastSibling();
		}

		/// <summary>
		/// Deactivates the component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DeactivateComponent(TItemView component)
		{
			if (component != null)
			{
				component.onClickItem.RemoveListener(CurrentClick);
				component.MovedToCache();
				component.Index = -1;
				component.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Updates the components count.
		/// </summary>
		protected void UpdateComponentsCount()
		{
			components.RemoveAll(IsNullComponent);

			if (components.Count == currentIndices.Count)
			{
				return;
			}

			if (components.Count < currentIndices.Count)
			{
				componentsCache.RemoveAll(IsNullComponent);

				for (int i = components.Count; i < currentIndices.Count; i++)
				{
					AddComponent();
				}
			}
			else
			{
				for (int i = currentIndices.Count; i < components.Count; i++)
				{
					DeactivateComponent(components[i]);
					componentsCache.Add(components[i]);
				}

				components.RemoveRange(currentIndices.Count, components.Count - currentIndices.Count);
			}
		}

		/// <summary>
		/// Determines whether the specified component is null.
		/// </summary>
		/// <returns><c>true</c> if the specified component is null; otherwise, <c>false</c>.</returns>
		/// <param name="component">Component.</param>
		protected bool IsNullComponent(TItemView component)
		{
			return component == null;
		}

		/// <summary>
		/// Gets the index of the component.
		/// </summary>
		/// <returns>The component index.</returns>
		/// <param name="item">Item.</param>
		protected int GetComponentIndex(TItemView item)
		{
			return item.Index;
		}

		/// <inheritdoc/>
		public override void OnSubmit(BaseEventData eventData)
		{
			if (ShowListViewOnSubmit)
			{
				ShowList();
			}
		}

		/// <summary>
		/// Updates the current component.
		/// </summary>
		[Obsolete("Use SetData() instead.")]
		protected virtual void UpdateCurrent()
		{
			HideList();
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			CanvasResize.Clear();

			ComboboxPosition.ParentDestroyed();

			Localization.OnLocaleChanged -= LocaleChanged;

			ListViewUnsubscribe(restorePosition: false);
			listView = null;

			ToggleButton = null;
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			if (!Utilities.IsNull(Current))
			{
				Current.SetThemeImagesPropertiesOwner(this);
			}

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}

			if (Root == null)
			{
				Root = transform.parent as RectTransform;
			}
		}

		/// <inheritdoc/>
		protected override void Reset()
		{
			base.Reset();

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}

			if (Root == null)
			{
				Root = transform.parent as RectTransform;
			}
		}
		#endif

		#region IStylable implementation

		/// <summary>
		/// Set components style.
		/// </summary>
		/// <param name="style">Style.</param>
		/// <param name="component">Component.</param>
		protected virtual void SetComponentStyle(Style style, TItemView component)
		{
			if (component == null)
			{
				return;
			}

			if (listView.MultipleSelect)
			{
				component.SetStyle(style.Combobox.MultipleDefaultItemBackground, style.Combobox.MultipleDefaultItemText, style);
			}
			else
			{
				component.SetStyle(style.Combobox.SingleDefaultItemBackground, style.Combobox.SingleDefaultItemText, style);
			}

			var remove_button = component.transform.Find("Remove");
			if (remove_button != null)
			{
				style.Combobox.RemoveBackground.ApplyTo(remove_button);
				style.Combobox.RemoveText.ApplyTo(remove_button.Find("Text"));
			}
		}

		/// <summary>
		/// Get button image.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <returns>Image component.</returns>
		protected virtual Image GetButtonImage(Transform button)
		{
			for (int i = 0; i < button.childCount; i++)
			{
				var child = button.GetChild(i);
				if (child.TryGetComponent<Image>(out var img))
				{
					return img;
				}
			}

			button.TryGetComponent<Image>(out var main_img);
			return main_img;
		}

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				var s = listView.MultipleSelect ? style.Combobox.MultipleInputBackground : style.Combobox.SingleInputBackground;
				s.ApplyTo(bg);
			}

			if (toggleButton != null)
			{
				style.Combobox.ToggleButton.ApplyTo(GetButtonImage(toggleButton.transform));
			}

			if (current != null)
			{
				SetComponentStyle(style, current);
			}

			for (int i = 0; i < components.Count; i++)
			{
				SetComponentStyle(style, components[i]);
			}

			for (int i = 0; i < componentsCache.Count; i++)
			{
				SetComponentStyle(style, componentsCache[i]);
			}

			if (!Utilities.IsNull(listView))
			{
				listView.SetStyle(style);
			}

			return true;
		}

		/// <summary>
		/// Set style options from the specified component.
		/// </summary>
		/// <param name="style">Style.</param>
		/// <param name="component">Component.</param>
		protected virtual void GetComponentStyle(Style style, TItemView component)
		{
			if (component == null)
			{
				return;
			}

			if (listView.MultipleSelect)
			{
				component.GetStyle(style.Combobox.MultipleDefaultItemBackground, style.Combobox.MultipleDefaultItemText, style);
			}
			else
			{
				component.GetStyle(style.Combobox.SingleDefaultItemBackground, style.Combobox.SingleDefaultItemText, style);
			}

			var remove_button = component.transform.Find("Remove");
			if (remove_button != null)
			{
				style.Combobox.RemoveBackground.GetFrom(remove_button);
				style.Combobox.RemoveText.GetFrom(remove_button.Find("Text"));
			}
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				var s = listView.MultipleSelect ? style.Combobox.MultipleInputBackground : style.Combobox.SingleInputBackground;
				s.GetFrom(bg);
			}

			if (toggleButton != null)
			{
				style.Combobox.ToggleButton.GetFrom(GetButtonImage(toggleButton.transform));
			}

			if (!Utilities.IsNull(current))
			{
				GetComponentStyle(style, current);
			}

			if (!Utilities.IsNull(listView))
			{
				listView.GetStyle(style);
			}

			return true;
		}
		#endregion
	}
}