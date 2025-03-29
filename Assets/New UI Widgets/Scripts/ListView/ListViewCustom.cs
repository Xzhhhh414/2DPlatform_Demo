namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using UIWidgets.Attributes;
	using UIWidgets.Extensions;
	using UIWidgets.l10n;
	using UIWidgets.Pool;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	/// <typeparam name="TItemView">Type of DefaultItem component.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed.")]
	[DataBindSupport]
	public partial class ListViewCustom<TItemView, TItem> : ListViewCustom<TItem>, IUpdatable, ILateUpdatable, IListViewCallbacks<TItemView>
		where TItemView : ListViewItem
	{
		/// <summary>
		/// Template class.
		/// </summary>
		[Serializable]
		public class Template : ListViewItemTemplate<TItemView>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Template"/> class.
			/// </summary>
			public Template()
				: base()
			{
			}
		}

		/// <summary>
		/// DataSourceEvent.
		/// </summary>
		[Serializable]
		public class DataSourceEvent : UnityEvent<ListViewCustom<TItemView, TItem>>
		{
		}

		/// <inheritdoc/>
		public override ListViewType ListType
		{
			get => listType;

			set
			{
				if (listType == value)
				{
					return;
				}

				listType = value;

				if (listRenderer != null)
				{
					listRenderer.Disable();
					ListRenderer.Init();
				}

				if (IsInited)
				{
					SetDefaultItem(defaultItem);
				}
			}
		}

		/// <inheritdoc/>
		[DataBindField]
		public override ObservableList<TItem> DataSource
		{
			get
			{
				if (dataSource == null)
				{
					dataSource = new ObservableList<TItem>(customItems, true, !Application.isPlaying);
					dataSource.OnChangeMono.AddListener(UpdateItems);
				}

				if (!IsInited)
				{
					Init();
				}

				return dataSource;
			}

			set
			{
				if (!IsInited)
				{
					Init();
				}

				SetNewItems(value, IsMainThread);

				if (IsMainThread)
				{
					ListRenderer.SetPosition(0f);
				}
				else
				{
					DataSourceSetted = true;
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("DefaultItem")]
		TItemView defaultItem;

		/// <summary>
		/// The default item template.
		/// </summary>
		public TItemView DefaultItem
		{
			get => defaultItem;

			set => SetDefaultItem(value);
		}

		#region ComponentPool fields

		/// <summary>
		/// Instances list.
		/// Hidden in inspector.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Components")]
		protected List<TItemView> Instances = new List<TItemView>();

		/// <summary>
		/// Instances list.
		/// </summary>
		[Obsolete("Renamed to Components.")]
		protected List<TItemView> Components => Instances;

		/// <summary>
		/// Own templates list.
		/// Hidden in inspector.
		/// </summary>
		[SerializeField]
		protected List<Template> OwnTemplates = new List<Template>();

		/// <summary>
		/// Shared templates list.
		/// Hidden in inspector.
		/// </summary>
		[SerializeField]
		protected List<Template> SharedTemplates;

		/// <summary>
		/// Templates list.
		/// </summary>
		protected List<Template> Templates
		{
			get
			{
				if (SharedTemplates != null)
				{
					return SharedTemplates;
				}

				return OwnTemplates;
			}
		}

		/// <summary>
		/// Indices of the displayed instances.
		/// Hidden in inspector.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("ComponentsDisplayedIndices")]
		protected List<int> InstancesDisplayedIndices = new List<int>();

		/// <summary>
		/// Indices of the displayed instances.
		/// </summary>
		[Obsolete("Renamed to ComponentsDisplayedIndices.")]
		protected List<int> ComponentsDisplayedIndices => InstancesDisplayedIndices;

		/// <inheritdoc/>
		public override bool DestroyDefaultItemsCache
		{
			get => destroyDefaultItemsCache;

			set => destroyDefaultItemsCache = value;
		}

		ListViewComponentPool componentsPool;

		/// <summary>
		/// The components pool.
		/// Constructor with lists needed to avoid lost connections when instantiated copy of the inited ListView.
		/// </summary>
		protected virtual ListViewComponentPool ComponentsPool
		{
			get
			{
				componentsPool ??= new ListViewComponentPool(this, SetInstanceData);

				return componentsPool;
			}
		}

		#endregion

		/// <summary>
		/// Overridden template sizes.
		/// </summary>
		protected Dictionary<InstanceID, Vector2> overriddenTemplateSizes;

		/// <summary>
		/// Allow to override Template.DefaultItem.
		/// Use case: Multiple ListView with shared templates but different item sizes.
		/// </summary>
		public Dictionary<InstanceID, Vector2> OverriddenTemplateSizes
		{
			get
			{
				overriddenTemplateSizes ??= new Dictionary<InstanceID, Vector2>();

				return overriddenTemplateSizes;
			}
		}

		[Obsolete("Replaced with DataSource.Comparison.")]
		Func<IEnumerable<TItem>, IEnumerable<TItem>> sortFunc;

		/// <summary>
		/// Sort function.
		/// Deprecated. Replaced with DataSource.Comparison.
		/// </summary>
		[Obsolete("Replaced with DataSource.Comparison.")]
		public Func<IEnumerable<TItem>, IEnumerable<TItem>> SortFunc
		{
			get => sortFunc;

			set
			{
				sortFunc = value;
				if (Sort && IsInited)
				{
					UpdateItems();
				}
			}
		}

		/// <inheritdoc/>
		protected override Vector2 ContainerAnchoredPosition
		{
			get
			{
				var pos = Container.anchoredPosition;
				if (ReverseOrderRequired)
				{
					var size = ListRenderer.ListSize() - (Viewport.ScaledAxisSize - LayoutBridge.GetFullMargin());
					if (IsHorizontal())
					{
						pos.x = -size - pos.x;
					}
					else
					{
						pos.y = size - pos.y;
					}
				}

				var scale = Container.localScale;
				return new Vector2(pos.x / scale.x, pos.y / scale.y);
			}

			set
			{
				if (ReverseOrderRequired)
				{
					var size = ListRenderer.ListSize() - (Viewport.ScaledAxisSize - LayoutBridge.GetFullMargin());
					if (IsHorizontal())
					{
						value.x = Mathf.Min(0f, -size - value.x);
					}
					else
					{
						value.y = size - value.y;
					}
				}

				var scale = Container.localScale;
				Container.anchoredPosition = new Vector2(value.x * scale.x, value.y * scale.y);
			}
		}

		/// <summary>
		/// Should be ReverseOrder applied?
		/// </summary>
		public bool ReverseOrderRequired
		{
			get
			{
				if (IsHorizontal())
				{
					var scrollbar = ScrollRect.horizontalScrollbar;
					if (scrollbar == null)
					{
						return ReversedOrder;
					}

					return ReversedOrder && (ReversedOrder == (scrollbar.direction == Scrollbar.Direction.LeftToRight));
				}
				else
				{
					var v_scrollbar = ScrollRect.verticalScrollbar;
					if (v_scrollbar == null)
					{
						return ReversedOrder;
					}

					return ReversedOrder && (ReversedOrder == (v_scrollbar.direction == Scrollbar.Direction.BottomToTop));
				}
			}
		}

		/// <summary>
		/// Called after component instantiated.
		/// </summary>
		public event Action<TItemView> OnComponentCreated;

		/// <summary>
		/// Called before component destroyed.
		/// </summary>
		public event Action<TItemView> OnComponentDestroyed;

		/// <summary>
		/// Called after component activated.
		/// </summary>
		public event Action<TItemView> OnComponentActivated;

		/// <summary>
		/// Called after component cached.
		/// </summary>
		public event Action<TItemView> OnComponentCached;

		/// <summary>
		/// ListView renderer.
		/// Hidden in inspector. Type is not serializable.
		/// </summary>
		[SerializeField]
		ListViewTypeBase listRenderer;

		/// <summary>
		/// ListView renderer.
		/// </summary>
		protected ListViewTypeBase ListRenderer
		{
			get
			{
				if (listRenderer == null)
				{
					listRenderer = GetRenderer(ListType);
					listRenderer.Init();
					listRenderer.ToggleScrollToItemCenter(ScrollInertiaUntilItemCenter);
				}

				return listRenderer;
			}

			set => listRenderer = value;
		}

		/// <inheritdoc/>
		public override int MaxVisibleItems
		{
			get
			{
				Init();

				return ListRenderer.MaxVisibleItems;
			}
		}

		/// <inheritdoc/>
		protected override ILayoutBridge LayoutBridge
		{
			get
			{
				if (layoutBridge == null)
				{
					if (Layout != null)
					{
						layoutBridge = new EasyLayoutBridge(Layout, DefaultItem.transform as RectTransform, setContentSizeFitter && ListRenderer.AllowSetContentSizeFitter, ListRenderer.AllowControlRectTransform)
						{
							IsHorizontal = IsHorizontal(),
						};
						ListRenderer.DirectionChanged();
					}
					else
					{
						if (Container.TryGetComponent<HorizontalOrVerticalLayoutGroup>(out var hv_layout))
						{
							layoutBridge = new StandardLayoutBridge(hv_layout, DefaultItem.transform as RectTransform, setContentSizeFitter && ListRenderer.AllowSetContentSizeFitter);
						}
					}
				}

				return layoutBridge;
			}
		}

		/// <summary>
		/// List can be looped.
		/// </summary>
		/// <value><c>true</c> if list can be looped; otherwise, <c>false</c>.</value>
		public override bool LoopedList
		{
			get => loopedList;

			set
			{
				loopedList = value;

				if (loopedList && IsInited)
				{
					ListRenderer.Init();
				}
			}
		}

		/// <inheritdoc/>
		public override bool LoopedListAvailable => ListRenderer.LoopedListAvailable;

		/// <summary>
		/// Raised when DataSource changed.
		/// </summary>
		public DataSourceEvent OnDataSourceChanged = new DataSourceEvent();

		DefaultSelector defaultTemplateSelector;

		/// <summary>
		/// Default template selector.
		/// </summary>
		protected DefaultSelector DefaultTemplateSelector
		{
			get
			{
				if (defaultTemplateSelector == null)
				{
					if (DefaultItem == null)
					{
						throw new InvalidOperationException("DefaultItem is not specified.");
					}

					defaultTemplateSelector = new DefaultSelector(DefaultItem);
				}

				return defaultTemplateSelector;
			}
		}

		IListViewTemplateSelector<TItemView, TItem> templateSelector;

		/// <summary>
		/// Template selector.
		/// </summary>
		public IListViewTemplateSelector<TItemView, TItem> TemplateSelector
		{
			get
			{
				if (Utilities.IsNull(templateSelector))
				{
					templateSelector = CreateTemplateSelector();
				}

				return templateSelector;
			}

			set
			{
				if (Utilities.IsNull(value))
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (ReferenceEquals(templateSelector, value))
				{
					return;
				}

				templateSelector = value;

				ToggleThemeSupport(templateSelector);

				if ((componentsPool != null) && DestroyDefaultItemsCache)
				{
					componentsPool.Destroy(excludeTemplates: templateSelector.AllTemplates());
				}

				if (IsInited)
				{
					TemplatesChanged();
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("stopScrollAtItemCenter")]
		[Tooltip("Custom scroll inertia until reach item center.")]
		bool scrollInertiaUntilItemCenter;

		/// <summary>
		/// Custom scroll inertia until reach item center.
		/// </summary>
		public bool ScrollInertiaUntilItemCenter
		{
			get => scrollInertiaUntilItemCenter;

			set
			{
				if (scrollInertiaUntilItemCenter != value)
				{
					scrollInertiaUntilItemCenter = value;

					ListRenderer.ToggleScrollToItemCenter(scrollInertiaUntilItemCenter);
				}
			}
		}

		/// <summary>
		/// Stop scroll inertia at item center.
		/// </summary>
		[Obsolete("Renamed to ScrollInertiaUntilItemCenter.")]
		public bool StopScrollAtItemCenter
		{
			get => ScrollInertiaUntilItemCenter;

			set => ScrollInertiaUntilItemCenter = value;
		}

		/// <summary>
		/// Scroll inertia.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("StopScrollInertia")]
		public AnimationCurve ScrollInertia = AnimationCurve.Linear(0, 0, 0.15f, 1);

		/// <summary>
		/// Scroll inertia.
		/// </summary>
		[Obsolete("Renamed to ScrollInertia.")]
		public AnimationCurve StopScrollInertia
		{
			get => ScrollInertia;

			set => ScrollInertia = value;
		}

		/// <summary>
		/// Scroll center state.
		/// </summary>
		protected enum ScrollCenterState
		{
			/// <summary>
			/// None.
			/// </summary>
			None = 0,

			/// <summary>
			/// Active.
			/// </summary>
			Active = 1,

			/// <summary>
			/// Disable.
			/// </summary>
			Disable = 2,
		}

		/// <summary>
		/// Current scroll center state.
		/// </summary>
		protected ScrollCenterState ScrollCenter = ScrollCenterState.None;

		/// <summary>
		/// Is need to handle resize event?
		/// </summary>
		protected bool NeedResize;

		/// <summary>
		/// Is need to update view?
		/// </summary>
		protected bool NeedUpdateView;

		/// <summary>
		/// Is scrolling forward or backward?
		/// </summary>
		protected bool IsForwardScroll;

		/// <summary>
		/// Cached ComponentsHighlightedColoring delegate.
		/// </summary>
		protected Action HighlightedColoringDelegate;

		/// <summary>
		/// Scroll data.
		/// </summary>
		protected ListViewBaseScrollData ScrollData;

		/// <summary>
		/// Retain scroll position.
		/// </summary>
		[SerializeField]
		[Tooltip("Retain scroll position when adding or removing items before the first visible item.")]
		public bool RetainScrollPosition = true;

		/// <summary>
		/// Check if instance is null.
		/// </summary>
		[DomainReloadExclude]
		protected static readonly Predicate<TItemView> InstanceIsNull = x => x == null;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			IsForwardScroll = true;

			templateSelector = null;
			defaultTemplateSelector = null;

			ScrollData = CreateScrollData();

			CanSetData = typeof(IViewData<TItem>).IsAssignableFrom(typeof(TItemView));

			ToggleThemeSupport(TemplateSelector);

			Instances.RemoveAll(InstanceIsNull);

			OwnTemplates.RemoveAll(x => x.Template == null);
			foreach (var template in OwnTemplates)
			{
				template.Restore();
			}

			if (SharedTemplates != null)
			{
				SharedTemplates.RemoveAll(x => x.Template == null);
				foreach (var template in SharedTemplates)
				{
					template.Restore();
				}
			}

			MainThread = Thread.CurrentThread;

			if (DisabledContainer == null)
			{
				var go = new GameObject("DisableContainer");
				go.SetActive(false);
				DisabledContainer = go.AddComponent<RectTransform>();
				DisabledContainer.SetParent(transform, false);
			}

			foreach (var template in OwnTemplates)
			{
				template.SetOwner(this);
			}

			foreach (var template in Templates)
			{
				template.UpdateId();
			}

			base.InitOnce();

			Items = new List<ListViewItem>();

			SelectedItemsCache.Clear();
			GetSelectedItems(SelectedItemsCache);

			DestroyGameObjects = false;

			InitTemplates();

			if (ListRenderer.IsVirtualizationSupported())
			{
				ScrollRect = scrollRect;
				CalculateItemSize();
			}

			SetContentSizeFitter = setContentSizeFitter;

			SetDirection(direction, false);

			UpdateItems();

			if (Layout != null)
			{
				Layout.SettingsChanged.AddListener(SetNeedResize);
			}
		}

		/// <summary>
		/// Create ScrollData.
		/// </summary>
		/// <returns>ScrollData.</returns>
		protected virtual ListViewBaseScrollData CreateScrollData()
		{
			return new ListViewScrollData(this);
		}

		[NonSerialized]
		IListViewTemplateSelector<TItemView, TItem> cachedSelector;

		/// <summary>
		/// Create template selector.
		/// </summary>
		/// <returns>Template selector.</returns>
		protected virtual IListViewTemplateSelector<TItemView, TItem> CreateTemplateSelector()
		{
			if (Utilities.IsNull(cachedSelector))
			{
				TryGetComponent(out cachedSelector);
			}

			return Utilities.IsNull(cachedSelector) ? DefaultTemplateSelector : cachedSelector;
		}

		bool localeSubscription;

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			Init();

			base.OnEnable();

			if (!localeSubscription)
			{
				localeSubscription = true;
				Localization.OnLocaleChanged += LocaleChanged;
				LocaleChanged();
			}

			var old = FadeDuration;
			FadeDuration = 0f;
			ComponentsColoring(true);
			FadeDuration = old;

			Resize();

			Updater.Add(this);
			Updater.AddLateUpdate(this);
			Updater.RunOnceNextFrame(this, ForceRebuild);
		}

		/// <summary>
		/// Add items without scroll.
		/// </summary>
		/// <param name="items">Items.</param>
		public void AddItemsWithoutScroll(IEnumerable<TItem> items)
		{
			var pos = GetScrollPosition();
			var start_size = ListRenderer.ListSize();

			DataSource.AddRange(items);

			if (!ReverseOrderRequired)
			{
				pos += ListRenderer.ListSize() - start_size;
			}

			if (ListRenderer.CanScroll)
			{
				LayoutBridge.UpdateLayout();
				ScrollToPosition(pos);
			}
		}

		/// <summary>
		/// Get template pool.
		/// </summary>
		/// <param name="template">Template.</param>
		/// <returns>Template pool.</returns>
		public virtual Template GetTemplatePool(TItemView template)
		{
			return ComponentsPool.GetTemplate(template);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			Updater.Remove(this);
			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			foreach (var t in Templates)
			{
				t.ClearCache();
			}

			Updater.RemoveRunOnceNextFrame(ComponentsHighlightedColoring);
			Updater.RemoveRunOnceNextFrame(ForceRebuild);
			Updater.RemoveRunOnceNextFrame(Resize);

			Localization.OnLocaleChanged -= LocaleChanged;

			if (dataSource != null)
			{
				dataSource.OnChangeMono.RemoveListener(UpdateItems);
				dataSource = null;
			}

			if (layout != null)
			{
				layout.SettingsChanged.RemoveListener(SetNeedResize);
				layout = null;
			}

			layoutBridge = null;

			ScrollRect = null;

			if (componentsPool != null)
			{
				componentsPool.Destroy();
				componentsPool = null;
			}

			base.OnDestroy();
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		protected override void OnValidate()
		{
			if ((scrollRect == null) && (transform.childCount > 0))
			{
				transform.GetChild(0).TryGetComponent(out scrollRect);
			}

			if ((Container == null) && (scrollRect != null))
			{
				Container = scrollRect.content;
			}

			if ((defaultItem == null) && (Container != null))
			{
				for (int i = 0; i < Container.childCount; i++)
				{
					if (Container.GetChild(i).TryGetComponent(out defaultItem))
					{
						break;
					}
				}
			}

			if (StyleTable && (header == null))
			{
				header = GetComponentInChildren<TableHeader>();
			}

			base.OnValidate();
		}
#endif

		/// <summary>
		/// Set shared templates.
		/// </summary>
		/// <param name="newSharedTemplates">New shared templates.</param>
		public virtual void SetSharedTemplates(List<Template> newSharedTemplates)
		{
			Init();

			if (SharedTemplates == newSharedTemplates)
			{
				return;
			}

			if (SharedTemplates == null)
			{
				SharedTemplates = newSharedTemplates;
				MoveTemplates(OwnTemplates, SharedTemplates);
				OwnTemplates.Clear();
			}
			else
			{
				if (newSharedTemplates == null)
				{
					SeparateTemplates(SharedTemplates, OwnTemplates);
					SharedTemplates = null;
				}
				else
				{
					SeparateTemplates(SharedTemplates, newSharedTemplates);
					SharedTemplates = newSharedTemplates;
				}
			}
		}

		/// <summary>
		/// Find template.
		/// </summary>
		/// <param name="templates">Templates.</param>
		/// <param name="id">Owner ID.</param>
		/// <returns>Template.</returns>
		protected virtual Template FindTemplate(List<Template> templates, InstanceID id)
		{
			foreach (var t in templates)
			{
				if (t.TemplateID == id)
				{
					return t;
				}
			}

			return null;
		}

		/// <summary>
		/// Move templates from source to target.
		/// </summary>
		/// <param name="sourceTemplates">Source templates.</param>
		/// <param name="targetTemplates">Target templates.</param>
		protected virtual void MoveTemplates(List<Template> sourceTemplates, List<Template> targetTemplates)
		{
			foreach (var source in sourceTemplates)
			{
				var target = FindTemplate(targetTemplates, source.TemplateID);
				if (target != null)
				{
					target.CopyFrom(source);
				}
				else
				{
					targetTemplates.Add(source);
				}
			}
		}

		/// <summary>
		/// Separate templates.
		/// </summary>
		/// <param name="sourceTemplates">Source templates.</param>
		/// <param name="targetTemplates">Target templates.</param>
		protected virtual void SeparateTemplates(List<Template> sourceTemplates, List<Template> targetTemplates)
		{
			foreach (var template in TemplateSelector.AllTemplates())
			{
				var source = FindTemplate(sourceTemplates, new InstanceID(template));
				if (source == null)
				{
					continue;
				}

				var target = FindTemplate(targetTemplates, new InstanceID(template));
				if (target == null)
				{
					target = ComponentsPool.CreateTemplate(template);
					targetTemplates.Add(target);
				}

				target.CopyFrom(source, false);
			}
		}

		/// <summary>
		/// Init templates.
		/// </summary>
		protected void InitTemplates()
		{
			foreach (var template in TemplateSelector.AllTemplates())
			{
				if ((template == null) || (template.gameObject == null))
				{
					Debug.LogWarning("ListView.TemplateSelector.AllTemplates() has template without GameObject.", this);
					continue;
				}

				template.gameObject.SetActive(true);
				template.FindSelectableObjects();
				template.gameObject.SetActive(false);

				ComponentsPool.GetTemplate(template);
			}
		}

		/// <summary>
		/// Get template by index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Template.</returns>
		protected virtual TItemView Index2Template(int index)
		{
			return TemplateSelector.Select(index, DataSource[index]);
		}

		/// <inheritdoc/>
		protected override void UpdateLayoutBridgeContentSizeFitter()
		{
			if (LayoutBridge != null)
			{
				LayoutBridge.UpdateContentSizeFitter = SetContentSizeFitter && ListRenderer.AllowSetContentSizeFitter;
			}
		}

		/// <inheritdoc/>
		protected override void SetScrollRect(ScrollRect newScrollRect)
		{
			if (scrollRect != null)
			{
				if (scrollRect.TryGetComponent<ResizeListener>(out var old_resize_listener))
				{
					old_resize_listener.OnResizeNextFrame.RemoveListener(SetNeedResize);
				}

				scrollRect.onValueChanged.RemoveListener(SelectableCheck);
				ListRenderer.Disable();
				scrollRect.onValueChanged.RemoveListener(SelectableSet);
				scrollRect.onValueChanged.RemoveListener(OnScrollRectUpdate);
			}

			scrollRect = newScrollRect;
			Viewport.ScrollRect = scrollRect;

			if (scrollRect != null)
			{
				var resize_listener = Utilities.RequireComponent<ResizeListener>(scrollRect);
				resize_listener.OnResizeNextFrame.AddListener(SetNeedResize);

				scrollRect.onValueChanged.AddListener(SelectableCheck);
				ListRenderer.Init();
				scrollRect.onValueChanged.AddListener(SelectableSet);
				scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);

				ToggleScrollRect();

				Viewport.RecalculateSizes();
			}
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			ComponentsPool.LocaleChanged();
		}

		/// <summary>
		/// Get the rendered of the specified ListView type.
		/// </summary>
		/// <param name="type">ListView type</param>
		/// <returns>Renderer.</returns>
		protected virtual ListViewTypeBase GetRenderer(ListViewType type)
		{
			ListViewTypeBase renderer = type switch
			{
				ListViewType.ListViewWithFixedSize => new ListViewTypeFixed(this),
				ListViewType.ListViewWithVariableSize => new ListViewTypeSize(this),
				ListViewType.TileViewWithFixedSize => new TileViewTypeFixed(this),
				ListViewType.TileViewWithVariableSize => new TileViewTypeSize(this),
				ListViewType.TileViewStaggered => new TileViewStaggered(this),
				ListViewType.ListViewEllipse => new ListViewTypeEllipse(this),
				_ => throw new NotSupportedException(string.Format("Unknown ListView type: {0}", EnumHelper<ListViewType>.ToString(type))),
			};

			return renderer;
		}

		/// <summary>
		/// Sets the default item.
		/// </summary>
		/// <param name="newDefaultItem">New default item.</param>
		protected virtual void SetDefaultItem(TItemView newDefaultItem)
		{
			if (Utilities.IsNull(newDefaultItem))
			{
				throw new ArgumentNullException(nameof(newDefaultItem));
			}

			defaultItem = newDefaultItem;
			DefaultTemplateSelector.Replace(newDefaultItem);

			if (IsInited)
			{
				TemplatesChanged();
			}
		}

		/// <summary>
		/// Process templates changed.
		/// </summary>
		protected virtual void TemplatesChanged()
		{
			InitTemplates();

			CalculateItemSize(true);

			CalculateMaxVisibleItems();

			UpdateView(forced: true, isNewData: true);

			if ((scrollRect != null) && scrollRect.TryGetComponent<ResizeListener>(out var resizeListener))
			{
				resizeListener.OnResize.Invoke();
			}
		}

		/// <inheritdoc/>
		protected override void SetDirection(ListViewDirection newDirection, bool updateView = true)
		{
			direction = newDirection;

			ListRenderer.ResetPosition();

			if (ListRenderer.IsVirtualizationSupported())
			{
				LayoutBridge.IsHorizontal = IsHorizontal();
				ListRenderer.DirectionChanged();

				CalculateMaxVisibleItems();
			}

			if (updateView)
			{
				UpdateView(forced: true, isNewData: false);
			}
		}

		/// <inheritdoc/>
		public override bool IsSortEnabled()
		{
			if (DataSource.Comparison != null)
			{
				return true;
			}

#pragma warning disable 0618
			return Sort && SortFunc != null;
#pragma warning restore 0618
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest index.</returns>
		/// <param name="eventData">Event data.</param>
		/// <param name="type">Preferable nearest index.</param>
		public override int GetNearestIndex(PointerEventData eventData, NearestType type)
		{
			if (IsSortEnabled())
			{
				return -1;
			}

			RectTransformUtility.ScreenPointToLocalPointInRectangle(Container, eventData.position, eventData.pressEventCamera, out var point);

			if (!Container.rect.Contains(point))
			{
				return DataSource.Count;
			}

			return GetNearestIndex(point, type);
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		/// <param name="point">Point.</param>
		/// <param name="type">Preferable nearest index.</param>
		public override int GetNearestIndex(Vector2 point, NearestType type)
		{
			var index = ListRenderer.GetNearestIndex(point, type);
			if (index == DataSource.Count)
			{
				return index;
			}

			index = LoopedListAvailable
				? ListRenderer.VisibleIndex2ItemIndex(index)
				: Mathf.Clamp(index, 0, DataSource.Count);

			return index;
		}

		/// <summary>
		/// Calculates the size of the item.
		/// </summary>
		/// <param name="reset">Reset item size.</param>
		protected virtual void CalculateItemSize(bool reset = false)
		{
			DefaultInstanceSize = ListRenderer.CalculateDefaultInstanceSize(DefaultInstanceSize, reset);
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		protected virtual void CalculateMaxVisibleItems()
		{
			if (!IsInited)
			{
				return;
			}

			ListRenderer.CalculateMaxVisibleItems();

			ListRenderer.ValidateContentSize();
		}

		/// <inheritdoc/>
		public override void Resize()
		{
			NeedResize = false;

			Viewport.RecalculateSizes();
			CalculateItemSize();

			ListRenderer.CalculateInstancesSizes(DataSource, false);

			CalculateMaxVisibleItems();
			UpdateView(forced: false, isNewData: false);
		}

		/// <inheritdoc/>
		protected override void SelectItem(int index)
		{
			var component = GetInstance(index);
			SelectColoring(component);

			if (component != null)
			{
				component.StateSelected();
			}
		}

		/// <inheritdoc/>
		protected override void DeselectItem(int index)
		{
			var component = GetInstance(index);
			DefaultColoring(component);

			if (component != null)
			{
				component.StateDefault();
			}
		}

		/// <inheritdoc/>
		public override void UpdateItems()
		{
			SetNewItems(DataSource, IsMainThread);
			IsDataSourceChanged = !IsMainThread;
		}

		/// <inheritdoc/>
		public override void Clear()
		{
			DataSource.Clear();
			ListRenderer.SetPosition(0f);
		}

		/// <inheritdoc/>
		public override void ScrollTo(int index)
		{
			if (!ListRenderer.IsVirtualizationPossible())
			{
				return;
			}

			index = Mathf.Clamp(index, 0, DataSource.Count - 1);
			ListRenderer.SetPosition(ListRenderer.GetPosition(index));
		}

		/// <summary>
		/// Get scroll position.
		/// </summary>
		/// <returns>Position.</returns>
		public override float GetScrollPosition()
		{
			if (!ListRenderer.IsVirtualizationPossible())
			{
				return 0f;
			}

			return ListRenderer.GetPosition();
		}

		/// <summary>
		/// Scrolls to specified position.
		/// </summary>
		/// <param name="position">Position.</param>
		public override void ScrollToPosition(float position)
		{
			if (!ListRenderer.IsVirtualizationPossible())
			{
				return;
			}

			ListRenderer.SetPosition(position);
		}

		/// <summary>
		/// Scrolls to specified position.
		/// </summary>
		/// <param name="position">Position.</param>
		public override void ScrollToPosition(Vector2 position)
		{
			if (!ListRenderer.IsVirtualizationPossible())
			{
				return;
			}

			ListRenderer.SetPosition(position);
		}

		/// <summary>
		/// Is visible item with specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="minVisiblePart">The minimal visible part of the item to consider item visible.</param>
		/// <returns>true if item visible; false otherwise.</returns>
		public override bool IsVisible(int index, float minVisiblePart = 0f)
		{
			if (!ListRenderer.IsVirtualizationSupported())
			{
				return false;
			}

			return ListRenderer.IsVisible(index, minVisiblePart);
		}

		/// <summary>
		/// Starts the scroll coroutine.
		/// </summary>
		/// <param name="coroutine">Coroutine.</param>
		protected virtual void StartScrollCoroutine(IEnumerator coroutine)
		{
			StopScrollCoroutine();
			ScrollCoroutine = coroutine;
			StartCoroutine(ScrollCoroutine);
		}

		/// <summary>
		/// Stops the scroll coroutine.
		/// </summary>
		protected virtual void StopScrollCoroutine()
		{
			if (ScrollCoroutine != null)
			{
				StopCoroutine(ScrollCoroutine);
			}

			ScrollCenter = ScrollCenterState.None;
		}

		/// <inheritdoc/>
		public override void ScrollStop()
		{
			StopScrollCoroutine();
		}

		/// <inheritdoc/>
		public override void ScrollToAnimated(int index)
		{
			index = Mathf.Clamp(index, 0, DataSource.Count - 1);
			StartScrollCoroutine(ScrollToAnimatedCoroutine(index, ScrollMovement, ScrollUnscaledTime));
		}

		/// <inheritdoc/>
		public override void ScrollToAnimated(int index, AnimationCurve animation, bool unscaledTime, Action after = null)
		{
			index = Mathf.Clamp(index, 0, DataSource.Count - 1);
			StartScrollCoroutine(ScrollToAnimatedCoroutine(index, animation, unscaledTime, after));
		}

		/// <inheritdoc/>
		public override void ScrollToPositionAnimated(float target)
		{
			ScrollToPositionAnimated(target, ScrollMovement, ScrollUnscaledTime);
		}

		/// <inheritdoc/>
		public override void ScrollToPositionAnimated(float target, Action after, Action everyFrame)
		{
			ScrollToPositionAnimated(target, ScrollMovement, ScrollUnscaledTime, after, everyFrame);
		}

		/// <inheritdoc/>
		public override void ScrollToPositionAnimated(float target, AnimationCurve animation, bool unscaledTime, Action after = null, Action everyFrame = null)
		{
#if CSHARP_7_3_OR_NEWER
			Vector2 Position()
#else
			Func<Vector2> Position = () =>
#endif
			{
				var current_position = ListRenderer.GetPositionVector();
				var target_position = IsHorizontal()
					? new Vector2(ListRenderer.ValidatePosition(-target), current_position.y)
					: new Vector2(current_position.x, ListRenderer.ValidatePosition(target));

				return target_position;
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			StartScrollCoroutine(ScrollToAnimatedCoroutine(Position, animation, unscaledTime, after, everyFrame));
		}

		/// <inheritdoc/>
		public override void ScrollToPositionAnimated(Vector2 target)
		{
			ScrollToPositionAnimated(target, ScrollMovement, ScrollUnscaledTime);
		}

		/// <inheritdoc/>
		public override void ScrollToPositionAnimated(Vector2 target, AnimationCurve animation, bool unscaledTime, Action after = null)
		{
#if CSHARP_7_3_OR_NEWER
			Vector2 Position()
#else
			Func<Vector2> Position = () =>
#endif
			{
				return ListRenderer.ValidatePosition(target);
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			StartScrollCoroutine(ScrollToAnimatedCoroutine(Position, animation, unscaledTime, after));
		}

		/// <summary>
		/// Scroll to specified index with time coroutine.
		/// </summary>
		/// <returns>The scroll to index with time coroutine.</returns>
		/// <param name="index">Index.</param>
		/// <param name="animation">Animation curve.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="after">Action to run after animation.</param>
		protected virtual IEnumerator ScrollToAnimatedCoroutine(int index, AnimationCurve animation, bool unscaledTime, Action after = null)
		{
#if CSHARP_7_3_OR_NEWER
			Vector2 Position()
#else
			Func<Vector2> Position = () =>
#endif
			{
				return ListRenderer.GetPosition(index);
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			return ScrollToAnimatedCoroutine(Position, animation, unscaledTime, after);
		}

		/// <summary>
		/// Get start position for the animated scroll.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Start position.</returns>
		protected virtual Vector2 GetScrollStartPosition(Vector2 target)
		{
			var start = ListRenderer.GetPositionVector();
			if (IsHorizontal())
			{
				start.x = -start.x;
			}

			if (LoopedListAvailable)
			{
				// find shortest distance to target for the looped list
				var list_size = ListRenderer.ListSize() + LayoutBridge.GetSpacing();
				var distance_straight = IsHorizontal()
					? (target.x - start.x)
					: (target.y - start.y);
				var distance_reverse_1 = IsHorizontal()
					? (target.x - (start.x + list_size))
					: (target.y - (start.y + list_size));
				var distance_reverse_2 = IsHorizontal()
					? (target.x - (start.x - list_size))
					: (target.y - (start.y - list_size));

				if (Mathf.Abs(distance_reverse_1) < Mathf.Abs(distance_straight))
				{
					if (IsHorizontal())
					{
						start.x += list_size;
					}
					else
					{
						start.y += list_size;
					}
				}

				if (Mathf.Abs(distance_reverse_2) <= Mathf.Abs(distance_straight))
				{
					if (IsHorizontal())
					{
						start.x -= list_size;
					}
					else
					{
						start.y -= list_size;
					}
				}
			}

			return start;
		}

		/// <summary>
		/// Scroll to specified position with time coroutine.
		/// </summary>
		/// <returns>The scroll to index with time coroutine.</returns>
		/// <param name="targetPosition">Target position.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		[Obsolete("Replaced with ScrollToAnimatedCoroutine(Func<Vector2> targetPosition, AnimationCurve animation, bool unscaledTime, Action after = null).")]
		protected virtual IEnumerator ScrollToAnimatedCoroutine(Func<Vector2> targetPosition, bool unscaledTime)
		{
			return ScrollToAnimatedCoroutine(targetPosition, ScrollMovement, unscaledTime);
		}

		/// <summary>
		/// Scroll to specified position with time coroutine.
		/// </summary>
		/// <returns>The scroll to index with time coroutine.</returns>
		/// <param name="targetPosition">Target position.</param>
		/// <param name="animation">Animation curve.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="after">Action to run after animation.</param>
		/// <param name="everyFrame">Action to run every frame.</param>
		protected virtual IEnumerator ScrollToAnimatedCoroutine(Func<Vector2> targetPosition, AnimationCurve animation, bool unscaledTime, Action after = null, Action everyFrame = null)
		{
			var start = GetScrollStartPosition(targetPosition());

			var time = 0f;
			var duration = animation[animation.length - 1].time;

			do
			{
				var target = targetPosition();
				var pos = start + ((target - start) * animation.Evaluate(time));

				ListRenderer.SetPosition(pos);

				everyFrame?.Invoke();

				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			ListRenderer.SetPosition(targetPosition());

			yield return null;

			ListRenderer.SetPosition(targetPosition());

			after?.Invoke();
		}

		/// <summary>
		/// Clamp item position.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <returns>Clamped position.</returns>
		protected virtual float ClampItemPosition(float position)
		{
			if (LoopedListAvailable)
			{
				return position;
			}

			return Mathf.Min(position, Mathf.Max(0, ListRenderer.ListSize() - Viewport.AxisSize));
		}

		/// <inheritdoc/>
		public override float GetItemPosition(int index, bool clampPosition = true)
		{
			var pos = ListRenderer.GetItemPosition(index);
			return clampPosition ? ClampItemPosition(pos) : pos;
		}

		/// <inheritdoc/>
		public override float GetItemPositionBorderEnd(int index, bool clampPosition = true)
		{
			var pos = ListRenderer.GetItemPositionBorderEnd(index);
			return clampPosition ? ClampItemPosition(pos) : pos;
		}

		/// <inheritdoc/>
		public override float GetItemPositionMiddle(int index, bool clampPosition = true)
		{
			var pos = ListRenderer.GetItemPositionMiddle(index);
			return clampPosition ? ClampItemPosition(pos) : pos;
		}

		/// <inheritdoc/>
		public override float GetItemPositionBottom(int index, bool clampPosition = true)
		{
			var pos = ListRenderer.GetItemPositionBottom(index);
			return clampPosition ? ClampItemPosition(pos) : pos;
		}

		/// <summary>
		/// Called after component instantiated.
		/// </summary>
		/// <param name="component">Component.</param>
		public virtual void ComponentCreated(TItemView component)
		{
			OnComponentCreated?.Invoke(component);
		}

		/// <summary>
		/// Called before component destroyed.
		/// </summary>
		/// <param name="component">Component.</param>
		public virtual void ComponentDestroyed(TItemView component)
		{
			OnComponentDestroyed?.Invoke(component);
		}

		/// <summary>
		/// Called after component became activated.
		/// </summary>
		/// <param name="component">Component.</param>
		public virtual void ComponentActivated(TItemView component)
		{
			AddCallback(component);

			OnComponentActivated?.Invoke(component);
		}

		/// <summary>
		/// Called after component moved to cache.
		/// </summary>
		/// <param name="component">Component.</param>
		public virtual void ComponentCached(TItemView component)
		{
			OnComponentCached?.Invoke(component);

			RemoveCallback(component);
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void AddCallback(TItemView item)
		{
			ListRenderer.AddCallback(item);

			#pragma warning disable 0618
			AddCallback(item as ListViewItem);
			#pragma warning restore 0618
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		[Obsolete("Replaced with AddCallback(TItemView item).")]
		protected virtual void AddCallback(ListViewItem item)
		{
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void RemoveCallback(TItemView item)
		{
			if (item == null)
			{
				return;
			}

			ListRenderer.RemoveCallback(item);

			#pragma warning disable 0618
			RemoveCallback(item as ListViewItem);
			#pragma warning restore 0618
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		[Obsolete("Replaced with RemoveCallback(TItemView item).")]
		protected virtual void RemoveCallback(ListViewItem item)
		{
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
		/// Gets the default width of the item.
		/// </summary>
		/// <returns>The default item width.</returns>
		public override float GetDefaultItemWidth()
		{
			return DefaultInstanceSize.x;
		}

		/// <summary>
		/// Gets the default height of the item.
		/// </summary>
		/// <returns>The default item height.</returns>
		public override float GetDefaultItemHeight()
		{
			return DefaultInstanceSize.y;
		}

		/// <summary>
		/// Check instances recycling.
		/// </summary>
		protected virtual void CheckRecycling()
		{
			// process restored recycling
			foreach (var index in DisableRecyclingIndices)
			{
				var instance = GetItemInstance(index);
#pragma warning disable 0618
				var restore_recycling = !(instance.DisableRecycling || instance.IsDragged);
#pragma warning restore 0618
				if (restore_recycling || DisplayedIndices.Contains(index))
				{
					instance.LayoutElement.ignoreLayout = false;
				}
			}

			// process disabled recycling
			DisableRecyclingIndices.Clear();
			foreach (var instance in Instances)
			{
#pragma warning disable 0618
				var disable_recycling = instance.DisableRecycling || instance.IsDragged;
#pragma warning restore 0618
				if (disable_recycling && !DisplayedIndices.Contains(instance.Index) && IsValid(instance.Index))
				{
					DisplayedIndices.Insert(DisableRecyclingIndices.Count, instance.Index);
					DisableRecyclingIndices.Add(instance.Index);

					instance.LayoutElement.ignoreLayout = true;
					instance.RectTransform.anchoredPosition = new Vector2(-90000f, 0f);
				}
			}
		}

		/// <summary>
		/// Processing SetDisplayedIndices.
		/// </summary>
		protected bool ProcessingSetDisplayedIndices = false;

		/// <summary>
		/// Repeat IsNewData value.
		/// </summary>
		protected bool? RepeatIsNewData;

		/// <summary>
		/// Sets the displayed indices.
		/// </summary>
		/// <param name="isNewData">Is new data?</param>
		protected virtual void SetDisplayedIndices(bool isNewData)
		{
			// prevent recursive SetDisplayedIndices() calls because size can be change during SetData() call
			if (ProcessingSetDisplayedIndices)
			{
				RepeatIsNewData = isNewData;
				return;
			}

			ProcessingSetDisplayedIndices = true;

			CheckRecycling();

			using var _ = ListPool<int>.Get(out var indices);
			indices.AddRange(DisplayedIndices);

			SetDisplayedIndices(indices, isNewData);

			while (RepeatIsNewData.HasValue)
			{
				isNewData = RepeatIsNewData.Value;
				RepeatIsNewData = null;

				indices.Clear();
				indices.AddRange(DisplayedIndices);

				SetDisplayedIndices(indices, isNewData);
			}

			HighlightedColoringDelegate ??= ComponentsHighlightedColoring;

			if (gameObject.activeInHierarchy)
			{
				Updater.RunOnceNextFrame(this, HighlightedColoringDelegate);
			}

			ProcessingSetDisplayedIndices = false;

			ScrollData.Update(DisplayedIndices);
		}

		/// <summary>
		/// Sets the displayed indices.
		/// </summary>
		/// <param name="indices">Indices.</param>
		/// <param name="isNewData">Is new data?</param>
		protected virtual void SetDisplayedIndices(List<int> indices, bool isNewData = true)
		{
			if (isNewData)
			{
				ComponentsPool.DisplayedIndicesSet(indices);
			}
			else
			{
				ComponentsPool.DisplayedIndicesUpdate(indices);
			}

			ListRenderer.UpdateInstancesSizes();

			ListRenderer.UpdateLayout(recalculate: true);
		}

		/// <summary>
		/// Process the ScrollRect update event.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void OnScrollRectUpdate(Vector2 position)
		{
			StartScrolling();

			ScrollData.UpdateScrollPosition();
		}

		/// <summary>
		/// Set data to component.
		/// </summary>
		/// <param name="component">Component.</param>
		[Obsolete("Renamed to ComponentSetData.")]
		protected virtual void ComponentSetData(TItemView component)
		{
			SetInstanceData(component);
		}

		/// <summary>
		/// Set data to the instance.
		/// </summary>
		/// <param name="instance">Instance.</param>
		protected virtual void SetInstanceData(TItemView instance)
		{
			instance.ResetDimensionsSize();

			if (IsValid(instance.Index))
			{
				SetData(instance, DataSource[instance.Index]);
			}

			Coloring(instance);

			if (IsSelected(instance.Index))
			{
				instance.StateSelected();
			}
			else
			{
				instance.StateDefault();
			}
		}

		/// <inheritdoc/>
		public override void UpdateView(bool forced, bool isNewData)
		{
			NeedUpdateView = false;

			if (!IsInited)
			{
				return;
			}

			if (forced)
			{
				ListRenderer.ResetDisplayedIndices();
			}

			if (ListRenderer.UpdateDisplayedIndices())
			{
				SetDisplayedIndices(isNewData);

				OnUpdateView.Invoke();
			}
		}

		/// <summary>
		/// Keep selected items on items update.
		/// </summary>
		[SerializeField]
		protected bool KeepSelection = true;

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view.</param>
		protected virtual void SetNewItems(ObservableList<TItem> newItems, bool updateView = true)
		{
			lock (DataSource)
			{
				var different = !ReferenceEquals(DataSource, newItems);
				if (different)
				{
					DataSource.OnChangeMono.RemoveListener(UpdateItems);
				}

				var selected = SelectedIndicesList;
				RecalculateSelectedIndices(newItems);
				DeselectRemoved(selected, NewSelectedIndices);

				dataSource = newItems;

				ListRenderer.CalculateInstancesSizes(DataSource, false);

				CalculateMaxVisibleItems();

				if (KeepSelection)
				{
					SilentSelect(NewSelectedIndices);
				}

				SelectedItemsCache.Clear();
				GetSelectedItems(SelectedItemsCache);

				if (different)
				{
					DataSource.OnChangeMono.AddListener(UpdateItems);
				}

				if (updateView)
				{
					ScrollData.Restore = true;
					UpdateView(forced: true, isNewData: true);
					ResizeToMaxItemSize();
				}
				else
				{
					ListRenderer.ResetDisplayedIndices();
				}
			}
		}

		/// <summary>
		/// Determines if item exists with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public override bool IsValid(int index)
		{
			return (index >= 0) && (index < DataSource.Count);
		}

		/// <summary>
		/// Process the item move event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		/// <param name="eventData">Event data.</param>
		protected override void OnItemMove(int index, ListViewItem item, AxisEventData eventData)
		{
			if (!Navigation)
			{
				return;
			}

			if (ListRenderer.OnItemMove(eventData, item))
			{
				return;
			}

			base.OnItemMove(index, item, eventData);
		}

		/// <summary>
		/// Coloring the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void Coloring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			if (IsSelected(component.Index))
			{
				SelectColoring(component);
			}
			else if (IsHighlighted(component.Index) && IsInteractable())
			{
				HighlightColoring(component);
			}
			else
			{
				DefaultColoring(component);
			}
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			if (IsSelected(component.Index))
			{
				return;
			}

			if (!IsInteractable())
			{
				return;
			}

			HighlightColoring(component as TItemView);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(TItemView component)
		{
			if (component == null)
			{
				return;
			}

			if (!allowColoring)
			{
				return;
			}

			if (!CanSelect(component.Index))
			{
				return;
			}

			if (IsSelected(component.Index))
			{
				return;
			}

			component.GraphicsColoring(HighlightedColor, HighlightedBackgroundColor, FadeDuration);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			SelectColoring(component as TItemView);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(TItemView component)
		{
			if (component == null)
			{
				return;
			}

			if (!allowColoring)
			{
				return;
			}

			if (IsInteractable())
			{
				component.GraphicsColoring(SelectedColor, SelectedBackgroundColor, FadeDuration);
			}
			else
			{
				component.GraphicsColoring(SelectedColor * DisabledColor, SelectedBackgroundColor * DisabledColor, FadeDuration);
			}
		}

		/// <inheritdoc/>
		protected override void DefaultColoring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			DefaultColoring(component as TItemView);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(TItemView component)
		{
			if (component == null)
			{
				return;
			}

			if (!allowColoring)
			{
				return;
			}

			var bg = DefaultBackgroundColor;
			if (ColoringStriped)
			{
				bg = (component.Index % 2) == 0 ? DefaultEvenBackgroundColor : DefaultOddBackgroundColor;
			}

			if (IsInteractable())
			{
				component.GraphicsColoring(DefaultColor, bg, FadeDuration);
			}
			else
			{
				component.GraphicsColoring(DefaultColor * DisabledColor, bg * DisabledColor, FadeDuration);
			}
		}

		/// <inheritdoc/>
		public override void SetTargetOwner()
		{
			ToggleThemeSupport(TemplateSelector);
		}

		/// <inheritdoc/>
		protected override void ToggleThemeSupport()
		{
			ToggleThemeSupport(null);
		}

		/// <summary>
		/// Toggle theme support.
		/// </summary>
		/// <param name="selector">Template selector.</param>
		protected virtual void ToggleThemeSupport(IListViewTemplateSelector<TItemView, TItem> selector)
		{
			if (!Utilities.IsNull(selector))
			{
				foreach (var template in selector.AllTemplates())
				{
					template.SetThemePropertyOwner(allowColoring ? this : null);
				}
			}
			else if (IsInited)
			{
				foreach (var component in ComponentsPool.GetEnumerator(PoolEnumeratorMode.All))
				{
					component.SetThemePropertyOwner(allowColoring ? this : null);
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			base.OnInteractableChange(interactableState);

			foreach (var component in ComponentsPool)
			{
				if (!interactableState)
				{
					component.StateDisabled();
					continue;
				}

				if (IsSelected(component.Index))
				{
					component.StateSelected();
				}
				else if (IsHighlighted(component.Index))
				{
					component.StateHighlighted();
				}
				else
				{
					component.StateDefault();
				}
			}
		}

		/// <inheritdoc/>
		public override void ComponentsColoring(bool instant = false)
		{
			if (!IsInited)
			{
				var old_duration = FadeDuration;
				FadeDuration = 0f;

				var selector = CreateTemplateSelector();
				foreach (var template in selector.AllTemplates())
				{
					if (template == null)
					{
						continue;
					}

					template.SetThemePropertyOwner(allowColoring ? this : null);
					if (allowColoring)
					{
						DefaultColoring(template);
					}
				}

				FadeDuration = old_duration;

				return;
			}

			if (!allowColoring && instant)
			{
				foreach (var component in ComponentsPool)
				{
					DefaultColoring(component);
				}

				return;
			}

			if (instant)
			{
				var old_duration = FadeDuration;
				FadeDuration = 0f;

				foreach (var component in ComponentsPool)
				{
					Coloring(component);
				}

				FadeDuration = old_duration;
			}
			else
			{
				foreach (var component in ComponentsPool)
				{
					Coloring(component);
				}
			}

			ComponentsHighlightedColoring();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the <see cref="ListViewComponentPool" />.
		/// </summary>
		/// <param name="mode">Mode.</param>
		/// <returns>A <see cref="ListViewBase.ListViewComponentEnumerator{TItemView, Template}" /> for the <see cref="ListViewComponentPool" />.</returns>
		public ListViewComponentEnumerator<TItemView, Template> GetComponentsEnumerator(PoolEnumeratorMode mode)
		{
			return ComponentsPool.GetEnumerator(mode);
		}

		/// <inheritdoc/>
		public override void SetTableSizes(IList<float> sizes, bool withHeader = true)
		{
			var width = !IsHorizontal();
			var axis = width ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			foreach (var instance in GetComponentsEnumerator(PoolEnumeratorMode.All))
			{
				for (int i = 0; i < sizes.Count; i++)
				{
					var size = sizes[i];
					var cell = instance.RectTransform.GetChild(i) as RectTransform;
					cell.SetSizeWithCurrentAnchors(axis, size);

					if (cell.TryGetComponent<LayoutElement>(out var le))
					{
						if (width)
						{
							le.preferredWidth = size;
						}
						else
						{
							le.preferredHeight = size;
						}
					}
				}
			}

			if (withHeader && (header != null))
			{
				header.SetCellsSize(sizes, false);
			}
		}

		/// <summary>
		/// Calls the specified action for each component.
		/// </summary>
		/// <param name="func">Action.</param>
		public override void ForEachComponent(Action<ListViewItem> func)
		{
			if (componentsPool != null)
			{
				foreach (var component in GetComponentsEnumerator(PoolEnumeratorMode.All))
				{
					func(component);
				}
			}
		}

		/// <summary>
		/// Calls the specified action for each component.
		/// </summary>
		/// <param name="func">Action.</param>
		public virtual void ForEachComponent(Action<TItemView> func)
		{
			if (componentsPool != null)
			{
				foreach (var component in GetComponentsEnumerator(PoolEnumeratorMode.All))
				{
					func(component);
				}
			}
		}

		/// <summary>
		/// Determines whether item visible.
		/// </summary>
		/// <returns><c>true</c> if item visible; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public override bool IsItemVisible(int index)
		{
			return DisplayedIndices.Contains(index);
		}

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <returns>The visible indices.</returns>
		public List<int> GetVisibleIndices()
		{
			return new List<int>(DisplayedIndices);
		}

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <param name="output">Output.</param>
		public void GetVisibleIndices(List<int> output)
		{
			output.AddRange(DisplayedIndices);
		}

		/// <summary>
		/// Gets the visible instances.
		/// </summary>
		/// <returns>The visible instances.</returns>
		[Obsolete("Renamed to GetVisibleInstances()")]
		public List<TItemView> GetVisibleComponents()
		{
			return new List<TItemView>(Instances);
		}

		/// <summary>
		/// Gets the visible instances.
		/// </summary>
		/// <returns>The visible instances.</returns>
		public List<TItemView> GetVisibleInstances()
		{
			return new List<TItemView>(Instances);
		}

		/// <summary>
		/// Gets the visible instances.
		/// </summary>
		/// <param name="output">Output.</param>
		[Obsolete("Renamed to GetVisibleInstances()")]
		public void GetVisibleComponents(List<TItemView> output)
		{
			output.AddRange(Instances);
		}

		/// <summary>
		/// Gets the visible instances.
		/// </summary>
		/// <param name="output">Output.</param>
		public void GetVisibleInstances(List<TItemView> output)
		{
			output.AddRange(Instances);
		}

		/// <summary>
		/// Gets the item instance.
		/// </summary>
		/// <returns>The item instance.</returns>
		/// <param name="index">Index.</param>
		[Obsolete("Renamed to GetItemInstance()")]
		public TItemView GetItemComponent(int index)
		{
			return GetInstance(index) as TItemView;
		}

		/// <summary>
		/// Gets the item instance.
		/// </summary>
		/// <returns>The item instance.</returns>
		/// <param name="index">Index.</param>
		public TItemView GetItemInstance(int index)
		{
			return GetInstance(index) as TItemView;
		}

		/// <summary>
		/// OnStartScrolling event.
		/// </summary>
		public UnityEvent OnStartScrolling = new UnityEvent();

		/// <summary>
		/// OnEndScrolling event.
		/// </summary>
		public UnityEvent OnEndScrolling = new UnityEvent();

		/// <summary>
		/// Time before raise OnEndScrolling event since last OnScrollRectUpdate event raised.
		/// </summary>
		public float EndScrollDelay = 0.3f;

		/// <summary>
		/// Is ScrollRect now on scrolling state.
		/// </summary>
		protected bool Scrolling;

		/// <summary>
		/// When last scroll event happen?
		/// </summary>
		protected float LastScrollingTime;

		/// <summary>
		/// Previous scroll position.
		/// </summary>
		protected float PreviousScrollRectPosition;

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void RunUpdate()
		{
			var scroll_position = ListRenderer.GetPosition();
			if (scroll_position != PreviousScrollRectPosition)
			{
				IsForwardScroll = ReverseOrderRequired
					? scroll_position < PreviousScrollRectPosition
					: scroll_position > PreviousScrollRectPosition;
				PreviousScrollRectPosition = scroll_position;
			}

			if (DataSourceSetted || IsDataSourceChanged)
			{
				var reset_scroll = DataSourceSetted;

				DataSourceSetted = false;
				IsDataSourceChanged = false;

				lock (DataSource)
				{
					CalculateMaxVisibleItems();
					UpdateView(forced: true, isNewData: true);
					ResizeToMaxItemSize();
				}

				if (reset_scroll)
				{
					ListRenderer.SetPosition(0f);
				}
			}

			if (NeedResize)
			{
				Resize();
			}

			if (NeedUpdateView)
			{
				CalculateMaxVisibleItems();
				UpdateView(forced: false, isNewData: false);
			}

			if (IsStopScrolling())
			{
				EndScrolling();
			}

			SelectableSet();
		}

		/// <summary>
		/// LateUpdate.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (NeedUpdateView)
			{
				CalculateMaxVisibleItems();
				UpdateView(forced: false, isNewData: false);
			}

			SelectableSet();
		}

		/// <summary>
		/// Scroll to the nearest item center.
		/// </summary>
		public void ScrollToItemCenter() => ListRenderer.ScrollToItemCenter();

		/// <summary>
		/// Base size.
		/// </summary>
		protected Vector2? BaseSize;

		/// <summary>
		/// Resize to max item size.
		/// </summary>
		protected virtual void ResizeToMaxItemSize()
		{
			var valid = StretchToMaxItemSize && (ListType == ListViewType.ListViewWithVariableSize);
			if (!valid)
			{
				return;
			}

			var rt = transform as RectTransform;
			var list_size = rt.rect.size;
			if (!BaseSize.HasValue)
			{
				BaseSize = list_size;
			}

			var content_size = Container.rect.size;
			var max_size = ListRenderer.GetMaxSize();

			if (IsHorizontal())
			{
				var size = Mathf.Max(BaseSize.Value.y, max_size.y + (list_size.y - content_size.y) + LayoutBridge.GetFullMarginY());
				rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			}
			else
			{
				var size = Mathf.Max(BaseSize.Value.x, max_size.x + (list_size.x - content_size.x) + LayoutBridge.GetFullMarginX());
				rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}
		}

		/// <summary>
		/// Force layout rebuild.
		/// </summary>
		protected virtual void ForceRebuild()
		{
			ResizeToMaxItemSize();

			foreach (var component in ComponentsPool)
			{
				LayoutRebuilder.MarkLayoutForRebuild(component.RectTransform);
			}

			Updater.RunOnceNextFrame(this, Resize);
		}

		/// <summary>
		/// Start to track scrolling event.
		/// </summary>
		protected virtual void StartScrolling()
		{
			LastScrollingTime = UtilitiesTime.GetTime(true);
			if (Scrolling)
			{
				return;
			}

			Scrolling = true;
			OnStartScrolling.Invoke();
		}

		/// <summary>
		/// Determines whether ScrollRect is stop scrolling.
		/// </summary>
		/// <returns><c>true</c> if ScrollRect is stop scrolling; otherwise, <c>false</c>.</returns>
		protected virtual bool IsStopScrolling()
		{
			if (!Scrolling)
			{
				return false;
			}

			return (LastScrollingTime + EndScrollDelay) <= UtilitiesTime.GetTime(true);
		}

		/// <summary>
		/// Raise OnEndScrolling event.
		/// </summary>
		protected virtual void EndScrolling()
		{
			Scrolling = false;
			OnEndScrolling.Invoke();

			if (ScrollInertiaUntilItemCenter && (ScrollCenter == ScrollCenterState.None) && !AutoScroll.Scrolling)
			{
				ListRenderer.ScrollToItemCenter();
				ScrollCenter = ScrollCenterState.Active;
			}

			if (ScrollCenter == ScrollCenterState.Disable)
			{
				ScrollCenter = ScrollCenterState.None;
			}
		}

		/// <summary>
		/// Sets the need resize.
		/// </summary>
		protected virtual void SetNeedResize()
		{
			if (!ListRenderer.IsVirtualizationSupported())
			{
				return;
			}

			NeedResize = true;
		}

		/// <inheritdoc/>
		public override void ChangeDefaultItemSize(Vector2 size)
		{
			ComponentsPool.SetSize(size);

			CalculateItemSize(true);
			CalculateMaxVisibleItems();
			UpdateView(forced: false, isNewData: false);
		}

		#region DebugInfo

		/// <summary>
		/// Get debug information.
		/// </summary>
		/// <returns>Debug information.</returns>
		public override string GetDebugInfo()
		{
			var sb = new System.Text.StringBuilder();

			sb.AppendValue("Unity Version: ", Application.unityVersion);
			sb.AppendValueEnum("Direction: ", Direction);
			sb.AppendValueEnum("Type: ", ListType);
			sb.AppendValue("Virtualization: ", Virtualization);
			sb.AppendValue("DataSource.Count: ", DataSource.Count);

			sb.AppendValue("Container Size: ", Container.rect.size);
			sb.AppendValue("Container Scale: ", Container.localScale);
			sb.AppendValue("DefaultItem Size: ", DefaultInstanceSize);
			sb.AppendValue("DefaultItem Scale: ", DefaultItem.RectTransform.localScale);

			sb.AppendValue("Viewport Size: ", Viewport.Size);
			sb.AppendValue("Looped: ", LoopedList);
			sb.AppendValue("AlignCenter: ", AlignCenter);
			sb.AppendValue("Precalculate Sizes: ", PrecalculateItemSizes);
			sb.AppendValue("Reversed Order: ", ReversedOrder);
			sb.AppendValue("Reversed Order Required: ", ReverseOrderRequired);

			sb.AppendValue("DisplayedIndices (count: ", DisplayedIndices.Count, "): ", UtilitiesCollections.List2String(DisplayedIndices));
			sb.AppendValue("Components Indices (count: ", Instances.Count, "): ");
			for (int i = 0; i < Instances.Count; i++)
			{
				var c = Instances[i];
				sb.Append(i);
				sb.Append(" ");
				if (c == null)
				{
					sb.Append("component is null");
				}
				else
				{
					sb.Append(c.name);
					sb.Append(": ");
					sb.Append(c.Index);
				}

				sb.AppendLine();
			}

			sb.AppendValue("Templates (count: ", Templates.Count, "): ");
			for (int i = 0; i < Templates.Count; i++)
			{
				var t = Templates[i];
				sb.Append(i);
				sb.Append(" ");
				if (t == null)
				{
					sb.Append("template is null");
				}
				else
				{
					if (t.Template == null)
					{
						sb.Append("template.Template is null");
					}
					else
					{
						sb.Append(t.Template.name);
						sb.Append("; Instances.Count: ");
						sb.Append(t.Instances.Count);
						sb.Append("; Requested.Count: ");
						sb.Append(t.Requested.Count);
						sb.Append("; Cache.Count: ");
						sb.Append(t.Cache.Count);
						sb.Append("; DefaultSize: ");
						sb.Append(t.DefaultSize);
						sb.Append("; OverriddenSize: ");
						if (OverriddenTemplateSizes.TryGetValue(t.TemplateID, out var size))
						{
							sb.Append(size);
						}
						else
						{
							sb.Append("not specified");
						}
					}
				}

				sb.AppendLine();
			}

			sb.AppendValue("StopScrollAtItemCenter: ", ScrollInertiaUntilItemCenter);
			sb.AppendValueEnum("ScrollCenterState: ", ScrollCenter);
			sb.AppendValue("ScrollPosition: ", ListRenderer.GetPosition());
			sb.AppendValue("ScrollVectorPosition: ", ListRenderer.GetPositionVector());

			sb.AppendLine();

			sb.AppendLine("#############");
			sb.AppendLine("**Runtime Info**");
			sb.AppendValue("NeedResize: ", NeedResize);
			sb.AppendValue("NeedUpdateView: ", NeedUpdateView);

			sb.AppendLine("#############");
			sb.AppendLine("**Renderer Info**");
			ListRenderer.GetDebugInfo(sb);
			sb.AppendLine();

			sb.AppendLine("#############");
			sb.AppendLine("**Layout Info**");
			if (Layout != null)
			{
				sb.AppendLine("Layout: EasyLayout");
				Layout.GetDebugInfo(sb);
			}
			else
			{
				var layout_type = Container.TryGetComponent<LayoutGroup>(out var layout)
					? UtilitiesEditor.GetFriendlyTypeName(layout.GetType())
					: "null";
				sb.AppendValue("Layout: ", layout_type);
			}

			return sb.ToString();
		}

		#endregion

		#region ListViewPaginator support

		/// <inheritdoc/>
		public override ScrollRect GetScrollRect() => ScrollRect;

		/// <inheritdoc/>
		public override int GetItemsCount() => DataSource.Count;

		/// <summary>
		/// Gets the items per block count.
		/// </summary>
		/// <returns>The items per block.</returns>
		public override int GetItemsPerBlock() => ListRenderer.GetItemsPerBlock();

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		public override int GetNearestItemIndex() => ListRenderer.GetNearestItemIndex();

		/// <summary>
		/// Gets the size of the DefaultItem.
		/// </summary>
		/// <returns>Size.</returns>
		public override Vector2 GetDefaultItemSize()
		{
			if (!IsInited)
			{
				return (DefaultItem.transform as RectTransform).rect.size;
			}

			return DefaultInstanceSize;
		}

		/// <summary>
		/// Get the size of the instance for the item with the specified index.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <returns>The instance size.</returns>
		public virtual Vector2 GetInstanceSize(int index)
		{
			if (!IsValid(index))
			{
				return DefaultInstanceSize;
			}

			return ListRenderer.GetInstanceFullSize(index);
		}

		/// <summary>
		/// Set the size of the instance for the item with the specified index.
		/// UpdateView() should be called after it to apply changes.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <param name="size">Size.</param>
		public virtual void SetInstanceSize(int index, Vector2 size)
		{
			if (IsValid(index))
			{
				ListRenderer.SetInstanceFullSize(index, size);
			}
		}

		/// <summary>
		/// Reset the size to the default for the item with the specified index.
		/// UpdateView() should be called after it to apply changes.
		/// </summary>
		/// <param name="index">Item index.</param>
		public virtual void ResetInstanceSize(int index)
		{
			if (IsValid(index))
			{
				ListRenderer.ResetInstanceFullSize(index);
			}
		}
		#endregion

		#region Obsolete

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <returns>The visible indices.</returns>
		[Obsolete("Use GetVisibleIndices()")]
		public List<int> GetVisibleIndicies()
		{
			return GetVisibleIndices();
		}
		#endregion

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void SetStyleDefaultItem(Style style)
		{
			if (componentsPool != null)
			{
				componentsPool.SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
			}
			else
			{
				foreach (var template in TemplateSelector.AllTemplates())
				{
					template.Owner = this;
					template.SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
					template.Owner = null;
				}
			}
		}

		/// <summary>
		/// Sets the style colors.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void SetStyleColors(Style style)
		{
			defaultBackgroundColor = style.Collections.DefaultBackgroundColor;
			defaultColor = style.Collections.DefaultColor;
			highlightedBackgroundColor = style.Collections.HighlightedBackgroundColor;
			highlightedColor = style.Collections.HighlightedColor;
			selectedBackgroundColor = style.Collections.SelectedBackgroundColor;
			selectedColor = style.Collections.SelectedColor;
		}

		/// <summary>
		/// Sets the ScrollRect style.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void SetStyleScrollRect(Style style)
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			var viewport = ScrollRect.viewport != null ? ScrollRect.viewport : Container.parent;
#else
			var viewport = Container.parent;
#endif

			if (viewport.TryGetComponent<Image>(out var img))
			{
				style.Collections.Viewport.ApplyTo(img);
			}

			style.HorizontalScrollbar.ApplyTo(ScrollRect.horizontalScrollbar);
			style.VerticalScrollbar.ApplyTo(ScrollRect.verticalScrollbar);
		}

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			SetStyleDefaultItem(style);

			SetStyleColors(style);

			SetStyleScrollRect(style);

			if (TryGetComponent<Image>(out var bg))
			{
				style.Collections.MainBackground.ApplyTo(bg);
			}

			if (StyleTable)
			{
				var image = Utilities.RequireComponent<Image>(Container);
				image.sprite = null;
				image.color = DefaultColor;

				var mask_image = Utilities.RequireComponent<Image>(Container.parent);
				mask_image.sprite = null;

				var mask = Utilities.RequireComponent<Mask>(Container.parent);
				mask.showMaskGraphic = false;

				defaultBackgroundColor = style.Table.Background.Color;
			}

			if (componentsPool != null)
			{
				ComponentsColoring(true);
			}
			else if (defaultItem != null)
			{
				foreach (var template in TemplateSelector.AllTemplates())
				{
					template.SetColors(DefaultColor, DefaultBackgroundColor);
				}
			}

			if (header != null)
			{
				header.SetStyle(style);
			}
			else
			{
				style.ApplyTo(transform.Find("Header"));
			}

			return true;
		}

		/// <summary>
		/// Set style options from the DefaultItem.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void GetStyleDefaultItem(Style style)
		{
			foreach (var template in TemplateSelector.AllTemplates())
			{
				if (!IsInited)
				{
					template.Owner = this;
				}

				template.GetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);

				if (!IsInited)
				{
					template.Owner = null;
				}
			}
		}

		/// <summary>
		/// Get style colors.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void GetStyleColors(Style style)
		{
			style.Collections.DefaultBackgroundColor = defaultBackgroundColor;
			style.Collections.DefaultColor = defaultColor;
			style.Collections.HighlightedBackgroundColor = highlightedBackgroundColor;
			style.Collections.HighlightedColor = highlightedColor;
			style.Collections.SelectedBackgroundColor = selectedBackgroundColor;
			style.Collections.SelectedColor = selectedColor;
		}

		/// <summary>
		/// Get style options from the ScrollRect.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void GetStyleScrollRect(Style style)
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			var viewport = ScrollRect.viewport != null ? ScrollRect.viewport : Container.parent;
#else
			var viewport = Container.parent;
#endif

			if (viewport.TryGetComponent<Image>(out var img))
			{
				style.Collections.Viewport.GetFrom(img);
			}

			style.HorizontalScrollbar.GetFrom(ScrollRect.horizontalScrollbar);
			style.VerticalScrollbar.GetFrom(ScrollRect.verticalScrollbar);
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			GetStyleDefaultItem(style);

			GetStyleColors(style);

			GetStyleScrollRect(style);

			if (TryGetComponent<Image>(out var bg))
			{
				style.Collections.MainBackground.GetFrom(bg);
			}

			if (StyleTable)
			{
				style.Table.Background.Color = defaultBackgroundColor;
			}

			if (header != null)
			{
				header.GetStyle(style);
			}
			else
			{
				style.GetFrom(transform.Find("Header"));
			}

			return true;
		}
		#endregion

		#region Selectable

		/// <summary>
		/// Selectable data.
		/// </summary>
		protected struct SelectableData : IEquatable<SelectableData>
		{
			/// <summary>
			/// Is need to update EventSystem.currentSelectedGameObject?
			/// </summary>
			public bool Update;

			/// <summary>
			/// Index of the item with selectable GameObject.
			/// </summary>
			public int Item
			{
				get;
				private set;
			}

			/// <summary>
			/// Index of the selectable GameObject of the item.
			/// </summary>
			public int SelectableGameObject
			{
				get;
				private set;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="SelectableData"/> struct.
			/// </summary>
			/// <param name="item">Index of the item with selectable GameObject.</param>
			/// <param name="selectableGameObject">Index of the selectable GameObject of the item.</param>
			public SelectableData(int item, int selectableGameObject)
			{
				Update = true;
				Item = item;
				SelectableGameObject = selectableGameObject;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public readonly override int GetHashCode() => Item ^ SelectableGameObject;

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly override bool Equals(object obj) => (obj is SelectableData data) && Equals(data);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly bool Equals(SelectableData other) => (Update == other.Update) && (Item == other.Item) && (SelectableGameObject == other.SelectableGameObject);

			/// <summary>
			/// Compare specified objects.
			/// </summary>
			/// <param name="data1">First object.</param>
			/// <param name="data2">Second object.</param>
			/// <returns>true if the objects are equal; otherwise, false.</returns>
			public static bool operator ==(SelectableData data1, SelectableData data2) => data1.Equals(data2);

			/// <summary>
			/// Compare specified objects.
			/// </summary>
			/// <param name="data1">First object.</param>
			/// <param name="data2">Second object.</param>
			/// <returns>true if the objects not equal; otherwise, false.</returns>
			public static bool operator !=(SelectableData data1, SelectableData data2) => !data1.Equals(data2);
		}

		/// <summary>
		/// Selectable data.
		/// </summary>
		protected SelectableData NewSelectableData;

		/// <summary>
		/// Get current selected GameObject.
		/// </summary>
		/// <returns>Selected GameObject.</returns>
		protected GameObject GetCurrentSelectedGameObject()
		{
			var es = EventSystem.current;
			if (es == null)
			{
				return null;
			}

			var go = es.currentSelectedGameObject;
			if (go == null)
			{
				return null;
			}

			if (!go.transform.IsChildOf(Container))
			{
				return null;
			}

			return go;
		}

		/// <summary>
		/// Get item component with selected GameObject.
		/// </summary>
		/// <param name="go">Selected GameObject.</param>
		/// <returns>Item component.</returns>
		protected TItemView SelectedGameObject2Component(GameObject go)
		{
			if (go == null)
			{
				return null;
			}

			var t = go.transform;
			foreach (var component in ComponentsPool)
			{
				var item_transform = component.RectTransform;
				if (t.IsChildOf(item_transform) && (t.GetInstanceID() != item_transform.GetInstanceID()))
				{
					return component;
				}
			}

			return null;
		}

		/// <summary>
		/// Find index of the next item.
		/// </summary>
		/// <param name="index">Index of the current item with selected GameObject.</param>
		/// <returns>Index of the next item</returns>
		protected virtual int SelectableFindNextObjectIndex(int index)
		{
			for (int i = 0; i < DataSource.Count; i++)
			{
				var prev_index = index - i;
				var next_index = index + i;
				var prev_valid = IsValid(prev_index);
				var next_valid = IsValid(next_index);
				if (!prev_valid && !next_valid)
				{
					return -1;
				}

				if (IsVisible(next_index))
				{
					return next_index;
				}

				if (IsVisible(prev_index))
				{
					return prev_index;
				}
			}

			return -1;
		}

		/// <inheritdoc/>
		protected override void SelectableCheck()
		{
			var go = GetCurrentSelectedGameObject();
			if (go == null)
			{
				return;
			}

			var component = SelectedGameObject2Component(go);
			if (component == null)
			{
				return;
			}

			if (IsVisible(component.Index))
			{
				return;
			}

			var item_index = SelectableFindNextObjectIndex(component.Index);
			if (!IsValid(item_index))
			{
				return;
			}

			NewSelectableData = new SelectableData(item_index, component.GetSelectableIndex(go));
		}

		/// <inheritdoc/>
		protected override void SelectableSet()
		{
			if (!NewSelectableData.Update)
			{
				return;
			}

			var instance = GetItemInstance(NewSelectableData.Item);
			if (instance == null)
			{
				return;
			}

			var go = instance.GetSelectableObject(NewSelectableData.SelectableGameObject);
			NewSelectableData.Update = false;

			SetSelectedGameObject(go);
		}
		#endregion

		#region AutoScroll

		ListViewAutoScroll autoScroll;

		/// <summary>
		/// AutoScroll.
		/// </summary>
		protected virtual ListViewAutoScroll AutoScroll
		{
			get
			{
				autoScroll ??= new ListViewAutoScroll(this, ScrollRect.transform as RectTransform);
				return autoScroll;
			}
		}

		/// <inheritdoc/>
		public override bool AutoScrollStart(PointerEventData eventData, Action<PointerEventData> callback) => AutoScroll.AutoScrollStart(eventData, callback);

		/// <inheritdoc/>
		public override void AutoScrollStop() => AutoScroll.AutoScrollStop();
		#endregion
	}
}