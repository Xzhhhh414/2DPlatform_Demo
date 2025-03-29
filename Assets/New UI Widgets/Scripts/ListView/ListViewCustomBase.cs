namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using EasyLayoutNS;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	public abstract partial class ListViewCustomBase : ListViewBase, IAutoScroll
	{
		/// <summary>
		/// Virtualization.
		/// </summary>
		[SerializeField]
		protected bool virtualization = true;

		/// <summary>
		/// Virtualization.
		/// </summary>
		public bool Virtualization
		{
			get => virtualization;

			set
			{
				if (virtualization != value)
				{
					virtualization = value;
					UpdateView(forced: true, isNewData: false);
				}
			}
		}

		/// <summary>
		/// Reversed order.
		/// </summary>
		[SerializeField]
		protected bool reversedOrder = false;

		/// <summary>
		/// Reversed order.
		/// </summary>
		public bool ReversedOrder
		{
			get => reversedOrder;

			set
			{
				if (reversedOrder != value)
				{
					reversedOrder = value;

					UpdateView(forced: true, isNewData: false);
				}
			}
		}

		/// <summary>
		/// ListView display type.
		/// </summary>
		[SerializeField]
		protected ListViewType listType = ListViewType.ListViewWithFixedSize;

		/// <summary>
		/// ListView display type.
		/// </summary>
		public abstract ListViewType ListType
		{
			get;
			set;
		}

		/// <summary>
		/// Change EasyLayout.LayoutType together with ListType.
		/// </summary>
		[SerializeField]
		[Tooltip("Change EasyLayout.LayoutType together with ListType.")]
		public bool ChangeLayoutType = false;

		/// <summary>
		/// If data source setted?
		/// </summary>
		protected bool DataSourceSetted;

		/// <summary>
		/// Is data source changed?
		/// </summary>
		protected bool IsDataSourceChanged;

		/// <summary>
		/// Destroy instances of the previous DefaultItem when replacing DefaultItem.
		/// </summary>
		[SerializeField]
		[Tooltip("Destroy instances of the previous DefaultItem when replacing DefaultItem.")]
		protected bool destroyDefaultItemsCache = true;

		/// <summary>
		/// Destroy instances of the previous DefaultItem when replacing DefaultItem.
		/// </summary>
		public abstract bool DestroyDefaultItemsCache
		{
			get;
			set;
		}

		[SerializeField]
		[FormerlySerializedAs("Sort")]
		bool sort = true;

		/// <summary>
		/// Sort items.
		/// Deprecated. Replaced with DataSource.Comparison.
		/// </summary>
		[Obsolete("Replaced with DataSource.Comparison.")]
		public virtual bool Sort
		{
			get => sort;

			set
			{
				sort = value;
				if (sort && IsInited)
				{
					UpdateItems();
				}
			}
		}

		/// <summary>
		/// Disable ScrollRect if ListView is not interactable.
		/// </summary>
		[SerializeField]
		[Tooltip("Disable ScrollRect if not interactable.")]
		protected bool disableScrollRect = false;

		/// <summary>
		/// Disable ScrollRect if not interactable.
		/// </summary>
		public bool DisableScrollRect
		{
			get => disableScrollRect;

			set
			{
				if (disableScrollRect != value)
				{
					disableScrollRect = value;
					ToggleScrollRect();
				}
			}
		}

		/// <summary>
		/// The displayed indices.
		/// </summary>
		protected List<int> DisplayedIndices = new List<int>();

		/// <summary>
		/// The disabled recycling indices.
		/// </summary>
		protected List<int> DisableRecyclingIndices = new List<int>();

		/// <summary>
		/// Gets the first displayed index.
		/// </summary>
		/// <value>The first displayed index.</value>
		[Obsolete("Renamed to DisplayedIndexFirst.")]
		public int DisplayedIndicesFirst => DisplayedIndexFirst;

		/// <summary>
		/// Gets the last displayed index.
		/// </summary>
		/// <value>The last displayed index.</value>
		[Obsolete("Renamed to DisplayedIndexLast.")]
		public int DisplayedIndicesLast => DisplayedIndexLast;

		/// <inheritdoc/>
		public override int DisplayedIndexFirst => DisplayedIndices.Count > 0 ? DisplayedIndices[0] : -1;

		/// <inheritdoc/>
		public override int DisplayedIndexLast => DisplayedIndices.Count > 0 ? DisplayedIndices[DisplayedIndices.Count - 1] : -1;

		/// <summary>
		/// If enabled scroll limited to last item.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ScrollRect.MovementType = Clamped instead.")]
		public bool LimitScrollValue = false;

		/// <summary>
		/// The ScrollRect.
		/// </summary>
		[SerializeField]
		protected ScrollRect scrollRect;

		/// <summary>
		/// Gets or sets the ScrollRect.
		/// </summary>
		/// <value>The ScrollRect.</value>
		public ScrollRect ScrollRect
		{
			get => scrollRect;

			set => SetScrollRect(value);
		}

		ScrollRectData viewport;

		/// <summary>
		/// ScrollRect viewport data.
		/// </summary>
		protected ScrollRectData Viewport
		{
			get
			{
				viewport ??= new ScrollRectData(this, ScrollRect);

				return viewport;
			}
		}

		/// <summary>
		/// The size of the ScrollRect.
		/// </summary>
		[Obsolete("Replaced with Viewport.Size.")]
		protected Vector2 ScrollRectSize => Viewport.Size;

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		protected ListViewDirection direction = ListViewDirection.Vertical;

		/// <summary>
		/// Set content size fitter settings?
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("_setContentSizeFitter")]
		protected bool setContentSizeFitter = true;

		/// <summary>
		/// The set ContentSizeFitter parameters according direction.
		/// </summary>
		public bool SetContentSizeFitter
		{
			get => setContentSizeFitter;

			set
			{
				setContentSizeFitter = value;

				UpdateLayoutBridgeContentSizeFitter();
			}
		}

		/// <summary>
		/// Update LayoutBridge ContentSizeFitter.
		/// </summary>
		protected abstract void UpdateLayoutBridgeContentSizeFitter();

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction.</value>
		public ListViewDirection Direction
		{
			get => direction;

			set => SetDirection(value);
		}

		/// <summary>
		/// The layout.
		/// </summary>
		protected EasyLayout layout;

		/// <summary>
		/// Gets the layout.
		/// </summary>
		/// <value>The layout.</value>
		public EasyLayout Layout
		{
			get
			{
				if (layout == null)
				{
					Container.TryGetComponent(out layout);
				}

				return layout;
			}
		}

		/// <summary>
		/// LayoutBridge.
		/// </summary>
		protected ILayoutBridge layoutBridge;

		/// <summary>
		/// LayoutBridge.
		/// </summary>
		protected abstract ILayoutBridge LayoutBridge
		{
			get;
		}

		/// <summary>
		/// Require EasyLayout.
		/// </summary>
		protected virtual bool RequireEasyLayout => false;

		/// <summary>
		/// Scroll use unscaled time.
		/// </summary>
		[SerializeField]
		public bool ScrollUnscaledTime = true;

		/// <summary>
		/// Scroll movement curve.
		/// </summary>
		[SerializeField]
		[Tooltip("Requirements: start value should be less than end value; Recommended start value = 0; end value = 1;")]
		public AnimationCurve ScrollMovement = AnimationCurve.EaseInOut(0, 0, 0.25f, 1);

		/// <summary>
		/// The scroll coroutine.
		/// </summary>
		protected IEnumerator ScrollCoroutine;

		/// <summary>
		/// The main thread.
		/// </summary>
		protected Thread MainThread;

		/// <summary>
		/// Gets a value indicating whether this instance is executed in main thread.
		/// </summary>
		/// <value><c>true</c> if this instance is executed in main thread; otherwise, <c>false</c>.</value>
		protected bool IsMainThread => MainThread != null && MainThread.Equals(Thread.CurrentThread);

		/// <summary>
		/// Is DefaultItem implements IViewData{TItem}.
		/// </summary>
		protected bool CanSetData;

		/// <summary>
		/// Center the list items if all items visible.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("centerTheItems")]
		[Tooltip("Center the list items if all items visible.")]
		protected bool alignCenter;

		/// <summary>
		/// Center the list items if all items visible.
		/// </summary>
		public virtual bool AlignCenter
		{
			get => alignCenter;

			set
			{
				alignCenter = value;
				UpdateView(forced: true, isNewData: false);
			}
		}

		/// <summary>
		/// Center the list items if all items visible.
		/// </summary>
		[Obsolete("Renamed to AlignCenter")]
		public virtual bool CenterTheItems
		{
			get => AlignCenter;

			set => alignCenter = value;
		}

		/// <summary>
		/// List should be looped.
		/// </summary>
		[SerializeField]
		protected bool loopedList = false;

		/// <summary>
		/// List can be looped.
		/// </summary>
		/// <value><c>true</c> if list can be looped; otherwise, <c>false</c>.</value>
		public abstract bool LoopedList
		{
			get;

			set;
		}

		/// <summary>
		/// Precalculate item sizes.
		/// Disabling this option increase performance with huge lists of items with variable sizes and decrease scroll precision.
		/// </summary>
		[SerializeField]
		public bool PrecalculateItemSizes = true;

		/// <summary>
		/// Header.
		/// </summary>
		[SerializeField]
		protected TableHeader header;

		/// <summary>
		/// Header.
		/// </summary>
		public TableHeader Header
		{
			get => header;

			set
			{
				if (header != value)
				{
					if (header != null)
					{
						header.List = null;
					}

					header = value;

					if (header != null)
					{
						header.List = this;

						if (IsInited)
						{
							header.Refresh();
						}
					}
				}
			}
		}

		/// <summary>
		/// Maximal count of the visible items.
		/// </summary>
		public abstract int MaxVisibleItems
		{
			get;
		}

		/// <summary>
		/// The size of the DefaultItem.
		/// </summary>
		[Obsolete("Renamed to DefaultInstanceSize.")]
		protected Vector2 ItemSize => DefaultInstanceSize;

		/// <summary>
		/// The size of the DefaultItem.
		/// </summary>
		protected Vector2 DefaultInstanceSize;

		/// <inheritdoc/>
		protected override void UpdateComponents<TItemBase>(List<TItemBase> newItems)
		{
			instances.Clear();
			foreach (var item in newItems)
			{
				instances.Add(item);
			}
		}

		/// <summary>
		/// Toggle ScrollRect state.
		/// </summary>
		protected void ToggleScrollRect()
		{
			if (ScrollRect != null)
			{
				var interactable = !DisableScrollRect || IsInteractable();
				ScrollRect.enabled = interactable;

				if (ScrollRect.horizontalScrollbar != null)
				{
					ScrollRect.horizontalScrollbar.interactable = interactable;
				}

				if (ScrollRect.verticalScrollbar != null)
				{
					ScrollRect.verticalScrollbar.interactable = interactable;
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			base.OnInteractableChange(interactableState);
			ToggleScrollRect();
		}

		/// <summary>
		/// Sets the direction.
		/// </summary>
		/// <param name="newDirection">New direction.</param>
		/// <param name="updateView">Update view.</param>
		protected abstract void SetDirection(ListViewDirection newDirection, bool updateView = true);

		/// <summary>
		/// Update view.
		/// </summary>
		public void UpdateView() => UpdateView(forced: false, isNewData: true);

		/// <summary>
		/// Update view.
		/// </summary>
		/// <param name="forced">Forced update.</param>
		public void UpdateView(bool forced) => UpdateView(forced: forced, isNewData: true);

		/// <summary>
		/// Update view.
		/// </summary>
		/// <param name="forced">Forced update.</param>
		/// <param name="isNewData">Is new data.</param>
		public abstract void UpdateView(bool forced, bool isNewData);

		/// <summary>
		/// Set ScrollRect.
		/// </summary>
		/// <param name="newScrollRect">New ScrollRect.</param>
		protected abstract void SetScrollRect(ScrollRect newScrollRect);

		/// <summary>
		/// Get secondary scroll position (for the cross direction).
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Secondary scroll position.</returns>
		protected virtual float GetScrollPositionSecondary(int index)
		{
			var current_position = ContainerAnchoredPosition;

			return IsHorizontal() ? current_position.y : current_position.x;
		}

		/// <summary>
		/// Check currently selected GameObject.
		/// </summary>
		/// <param name="position">Scroll position.</param>
		protected void SelectableCheck(Vector2 position)
		{
			SelectableCheck();
		}

		/// <summary>
		/// Select new selectable GameObject if needed.
		/// </summary>
		/// <param name="position">Scroll position.</param>
		protected void SelectableSet(Vector2 position)
		{
			SelectableSet();
		}

		/// <summary>
		/// Check currently selected GameObject.
		/// </summary>
		protected abstract void SelectableCheck();

		/// <summary>
		/// Select new selectable GameObject if needed.
		/// </summary>
		protected abstract void SelectableSet();

		/// <summary>
		/// Gets the layout margin.
		/// </summary>
		/// <returns>The layout margin.</returns>
		public override Vector4 GetLayoutMargin() => LayoutBridge.GetMarginSize();

		/// <summary>
		/// Gets the spacing between items.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacing() => LayoutBridge.GetSpacing();

		/// <summary>
		/// Gets the horizontal spacing between items.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacingX() => LayoutBridge.GetSpacingX();

		/// <summary>
		/// Gets the vertical spacing between items.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacingY() => LayoutBridge.GetSpacingY();

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		public override bool IsHorizontal() => direction == ListViewDirection.Horizontal;

		#region AutoScroll

		/// <summary>
		/// Auto scroll area.
		/// </summary>
		[SerializeField]
		[Tooltip("Distance from the ScrollRect borders where auto scroll enabled.")]
		public float AutoScrollArea = 40f;

		/// <summary>
		/// Auto scroll speed.
		/// </summary>
		[SerializeField]
		public float AutoScrollSpeed = 200f;

		/// <summary>
		/// Start scroll when pointer is near ScrollRect border.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="callback">Callback.</param>
		/// <returns>true if auto scroll started; otherwise false.</returns>
		public abstract bool AutoScrollStart(PointerEventData eventData, Action<PointerEventData> callback);

		/// <summary>
		/// Stop auto-scroll.
		/// </summary>
		public abstract void AutoScrollStop();
		#endregion

		/// <summary>
		/// Start ListView in editor mode.
		/// </summary>
		public virtual void InEditorStart()
		{
#if UNITY_EDITOR
			Init();
#endif
		}

		/// <summary>
		/// Stop ListView in editor mode.
		/// </summary>
		public virtual void InEditorStop()
		{
#if UNITY_EDITOR
			OnDestroy();
			DisableInit();
#endif
		}

		/// <summary>
		/// Restart ListView in editor mode.
		/// </summary>
		public virtual void InEditorRestart()
		{
#if UNITY_EDITOR
			InEditorStop();
			InEditorStart();
#endif
		}
	}
}