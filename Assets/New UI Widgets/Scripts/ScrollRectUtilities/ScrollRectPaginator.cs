namespace UIWidgets
{
	using System;
	using System.Collections;
	using EasyLayoutNS;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRect Paginator.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/controls/paginator.html")]
	public class ScrollRectPaginator : PaginatorBase, IUpgradeable
	{
		/// <summary>
		/// ScrollRect for pagination.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("ScrollRect")]
		protected ScrollRect scrollRect;

		/// <summary>
		/// ScrollRect for pagination.
		/// </summary>
		public virtual ScrollRect ScrollRect
		{
			get => scrollRect;
			set => SetScrollRect(value);
		}

		/// <inheritdoc/>
		protected override bool IsValid => ScrollRect != null && ScrollRect.content != null;

		#region Obsolete

		/// <summary>
		/// DefaultPage template.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with View.DefaultPage")]
		protected RectTransform DefaultPage;

		/// <summary>
		/// ScrollRectPage component of DefaultPage.
		/// </summary>
		[Obsolete("Replaced with View.DefaultPage")]
		protected ScrollRectPage SRDefaultPage => view.DefaultPage;

		/// <summary>
		/// ActivePage.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with View.ActivePage")]
		protected RectTransform ActivePage;

		/// <summary>
		/// ScrollRectPage component of ActivePage.
		/// </summary>
		[Obsolete("Replaced with View.ActivePage")]
		protected ScrollRectPage SRActivePage => view.ActivePage;

		/// <summary>
		/// The previous page.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with View.PrevPage")]
		protected RectTransform PrevPage;

		/// <summary>
		/// ScrollRectPage component of PrevPage.
		/// </summary>
		[Obsolete("Replaced with View.PrevPage")]
		protected ScrollRectPage SRPrevPage => view.PrevPage;

		/// <summary>
		/// The next page.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with View.NextPage")]
		protected RectTransform NextPage;

		/// <summary>
		/// ScrollRectPage component of NextPage.
		/// </summary>
		[Obsolete("Replaced with View.NextPage")]
		protected ScrollRectPage SRNextPage => view.NextPage;

		#pragma warning disable 0414
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with View.VisiblePagesCount")]
		int visiblePagesCount = 0;
		#pragma warning restore

		/// <summary>
		/// Skip page template.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with View.SkipPage")]
		protected RectTransform SkipPage;

		#pragma warning disable 0649
		[SerializeField]
		[FormerlySerializedAs("ForceScrollOnPage")]
		bool forceScrollOnPage;
		#pragma warning restore 0649

		/// <summary>
		/// The force scroll position to page.
		/// </summary>
		[Obsolete("Replace with ForcedPosition.")]
		public bool ForceScrollOnPage
		{
			get => ForcedPosition == PaginatorPagePosition.OnStart;

			set => ForcedPosition = value ? PaginatorPagePosition.OnStart : PaginatorPagePosition.None;
		}

		/// <summary>
		/// Pages container.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with View.PagesContainer")]
		protected RectTransform pagesContainer;

		/// <summary>
		/// Pages container.
		/// </summary>
		[Obsolete("Replaced with View.PagesContainer")]
		protected RectTransform PagesContainer => view.PagesContainer;

		/// <summary>
		/// Set shared default pages.
		/// </summary>
		/// <param name="shared">Pool.</param>
		/// <returns>Previously used pages.</returns>
		[Obsolete("Replaced with View.SetSharedDefaultPages()")]
		public BasePool<ScrollRectPage> SetSharedDefaultPages(BasePool<ScrollRectPage> shared) => view.SetSharedDefaultPages(shared);

		/// <summary>
		/// Set shared skip pages.
		/// </summary>
		/// <param name="shared">Pool.</param>
		/// <returns>Previously used pages.</returns>
		[Obsolete("Replaced with View.SetSharedSkipPages()")]
		public BasePool<RectTransform> SetSharedSkipPages(BasePool<RectTransform> shared) => view.SetSharedSkipPages(shared);

		/// <summary>
		/// Get ScrollRect.
		/// </summary>
		/// <returns>ScrollRect</returns>
		[Obsolete("Replaced with ScrollRect property.")]
		public ScrollRect GetScrollRect() => ScrollRect;

		#endregion

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		public PaginatorDirection Direction = PaginatorDirection.Auto;

		/// <summary>
		/// The type of the page size.
		/// </summary>
		[SerializeField]
		protected PageSizeType pageSizeType = PageSizeType.Auto;

		/// <summary>
		/// Space between pages.
		/// </summary>
		[SerializeField]
		protected float pageSpacing = 0f;

		/// <summary>
		/// Space between pages.
		/// </summary>
		public float PageSpacing
		{
			get => pageSpacing;

			set
			{
				pageSpacing = value;

				RecalculatePages();
			}
		}

		/// <summary>
		/// Current page rounding.
		/// </summary>
		[SerializeField]
		protected PaginatorRounding pageRounding = PaginatorRounding.Round;

		/// <summary>
		/// Current page rounding.
		/// </summary>
		public PaginatorRounding PageRounding
		{
			get => pageRounding;

			set
			{
				pageRounding = value;

				CurrentPage = GetPage();
			}
		}

		/// <summary>
		/// Minimal drag distance to fast scroll to next slide.
		/// </summary>
		[SerializeField]
		public float FastDragDistance = 30f;

		/// <summary>
		/// Max drag time to fast scroll to next slide.
		/// </summary>
		[SerializeField]
		public float FastDragTime = 0.5f;

		/// <summary>
		/// Gets or sets the type of the page size.
		/// </summary>
		/// <value>The type of the page size.</value>
		public virtual PageSizeType PageSizeType
		{
			get => pageSizeType;

			set
			{
				pageSizeType = value;
				RecalculatePages();
			}
		}

		/// <summary>
		/// The size of the page.
		/// </summary>
		[SerializeField]
		protected float pageSize;

		/// <summary>
		/// Gets or sets the size of the page.
		/// </summary>
		/// <value>The size of the page.</value>
		public virtual float PageSize
		{
			get => pageSize;

			set
			{
				pageSize = value;
				RecalculatePages();
			}
		}

		/// <summary>
		/// The current page number.
		/// </summary>
		[SerializeField]
		protected int currentPage;

		/// <inheritdoc/>
		public override int CurrentPage
		{
			get => currentPage;

			set
			{
				Init();
				GoToPage(value);
			}
		}

		/// <summary>
		/// Scroll to specified page position after user drag or scroll.
		/// </summary>
		public PaginatorPagePosition ForcedPosition = PaginatorPagePosition.None;

		/// <summary>
		/// Use animation.
		/// </summary>
		[SerializeField]
		public bool Animation = true;

		/// <summary>
		/// Movement curve.
		/// </summary>
		[SerializeField]
		[Tooltip("Requirements: start value should be less than end value; Recommended start value = 0; end value = 1;")]
		[FormerlySerializedAs("Curve")]
		public AnimationCurve Movement = AnimationCurve.EaseInOut(0, 0, 1, 1);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// OnMovement event.
		/// Parameters:
		/// - page
		/// - relative distance between this page and next page in range 0..1
		/// </summary>
		[SerializeField]
		public PaginatorMovement OnMovement = new PaginatorMovement();

		/// <summary>
		/// Change the last page size to full-page size.
		/// </summary>
		[SerializeField]
		protected bool lastPageFullSize = true;

		/// <summary>
		/// Change the last page size to full-page size.
		/// </summary>
		public bool LastPageFullSize
		{
			get => lastPageFullSize;

			set
			{
				lastPageFullSize = value;
				UpdateLastPageMargin();
			}
		}

		/// <summary>
		/// Pages rounding error.
		/// </summary>
		[SerializeField]
		protected float roundingError = 2f;

		/// <summary>
		/// Pages rounding error.
		/// </summary>
		public float RoundingError
		{
			get => roundingError;

			set => roundingError = value;
		}

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// ScrollRect.content layout.
		/// </summary>
		protected EasyLayout Layout;

		/// <summary>
		/// Default margin value.
		/// </summary>
		protected float DefaultMargin;

		/// <summary>
		/// The current animation.
		/// </summary>
		protected IEnumerator currentAnimation;

		/// <summary>
		/// Is animation running?
		/// </summary>
		protected bool isAnimationRunning;

		/// <summary>
		/// Is dragging ScrollRect?
		/// </summary>
		protected bool isDragging;

		/// <summary>
		/// The cursor position at drag start.
		/// </summary>
		protected Vector2 CursorStartPosition;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (IsValid)
			{
				SetScrollRect(scrollRect);
			}
			else
			{
				Debug.LogWarning("ScrollRect is not specified or ScrollRect.content is not specified.", this);
			}
		}

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			base.OnEnable();

			Refresh();
		}

		/// <summary>
		/// Refresh this instance.
		/// </summary>
		public override void Refresh()
		{
			Init();
			RecalculatePages();
			base.Refresh();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		/// <param name="scroll">ScrollRect.</param>
		protected virtual void RemoveListeners(ScrollRect scroll)
		{
			if (scroll == null)
			{
				return;
			}

			scroll.onValueChanged.RemoveListener(OnScrollRectValueChanged);

			if (scroll.TryGetComponent<ResizeListener>(out var resize))
			{
				resize.OnResizeNextFrame.RemoveListener(RecalculatePages);
			}

			if (scroll.TryGetComponent<DragListener>(out var drag))
			{
				drag.OnBeginDragEvent.RemoveListener(OnScrollRectDragStart);
				drag.OnDragEvent.RemoveListener(OnScrollRectDrag);
				drag.OnEndDragEvent.RemoveListener(OnScrollRectDragEnd);
			}

			if (scroll.TryGetComponent<ScrollListener>(out var scroll_listener))
			{
				scroll_listener.ScrollEvent.RemoveListener(ContainerScroll);
			}

			if ((scroll.content != null) && scroll.content.TryGetComponent<ResizeListener>(out var content_resize))
			{
				content_resize.OnResizeNextFrame.RemoveListener(RecalculatePages);
			}
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		/// <param name="scroll">ScrollRect.</param>
		protected virtual void AddListeners(ScrollRect scroll)
		{
			scroll.onValueChanged.AddListener(OnScrollRectValueChanged);

			var resize = Utilities.RequireComponent<ResizeListener>(scroll);
			resize.OnResizeNextFrame.AddListener(RecalculatePages);

			var drag = Utilities.RequireComponent<DragListener>(scroll);
			drag.OnBeginDragEvent.AddListener(OnScrollRectDragStart);
			drag.OnDragEvent.AddListener(OnScrollRectDrag);
			drag.OnEndDragEvent.AddListener(OnScrollRectDragEnd);

			var scroll_listener = Utilities.RequireComponent<ScrollListener>(scroll);
			scroll_listener.ScrollEvent.AddListener(ContainerScroll);

			var content_resize = Utilities.RequireComponent<ResizeListener>(scroll.content);
			content_resize.OnResizeNextFrame.AddListener(RecalculatePages);
		}

		/// <summary>
		/// Set ScrollRect.
		/// </summary>
		/// <param name="value">ScrollRect.</param>
		protected virtual void SetScrollRect(ScrollRect value)
		{
			RemoveListeners(scrollRect);

			if (Layout != null)
			{
				SetLayoutMargin(0f);
				Layout = null;
			}

			if ((value != null) && (value.content != null))
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(value.content);
				AddListeners(value);

				scrollRect = value;

				if (scrollRect.content.TryGetComponent(out Layout))
				{
					DefaultMargin = IsHorizontal() ? Layout.MarginInner.Right : Layout.MarginInner.Bottom;
				}
			}
			else
			{
				scrollRect = null;
				DefaultMargin = 0f;
			}

			RecalculatePages();
			GoToPage(currentPage, true);
		}

		/// <summary>
		/// Invoke OnMovement event.
		/// </summary>
		protected virtual void MovementInvoke()
		{
			var position = Mathf.Round(GetCalculatedPosition());
			var page_size = GetPageSize();
			var prev_page = Mathf.FloorToInt(position / page_size);
			var ratio = (position - (page_size * prev_page)) / page_size;

			OnMovement.Invoke(prev_page, ratio);
		}

		/// <summary>
		/// Update layout margin to change the last page size to full-page size.
		/// </summary>
		protected virtual void UpdateLastPageMargin()
		{
			if (!IsValid)
			{
				return;
			}

			if (!LastPageFullSize)
			{
				return;
			}

			if (Layout == null)
			{
				Debug.LogWarning("LastPageFullSize requires EasyLayout component at the ScrollRect.content gameobject.");
				return;
			}

			SetLayoutMargin(GetLastPageMargin());
		}

		/// <summary>
		/// Set layout margin.
		/// </summary>
		/// <param name="margin">Margin.</param>
		protected virtual void SetLayoutMargin(float margin)
		{
			var layout_margin = Layout.MarginInner;
			if (IsHorizontal())
			{
				layout_margin.Right = margin + DefaultMargin;
			}
			else
			{
				layout_margin.Bottom = margin + DefaultMargin;
			}

			Layout.MarginInner = layout_margin;
		}

		/// <summary>
		/// Get margin to change the last page size to full-page size.
		/// </summary>
		/// <returns>Margin.</returns>
		protected virtual float GetLastPageMargin()
		{
			if (!IsValid)
			{
				return 0f;
			}

			var content_size = IsHorizontal() ? ScrollRect.content.rect.width : ScrollRect.content.rect.height;
			content_size -= IsHorizontal() ? Layout.MarginFullHorizontal : Layout.MarginFullVertical;
			var page_size = GetPageSize();

			var last_page_size = content_size % page_size;
			var margin = last_page_size > 0.1f
				? page_size - last_page_size - PageSpacing
				: 0f;

			return margin;
		}

		/// <summary>
		/// Handle ScrollRect scroll event.
		/// Open previous or next page depend of scroll direction.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void ContainerScroll(PointerEventData eventData)
		{
			if (ForcedPosition == PaginatorPagePosition.None)
			{
				UpdateObjects(GetPage());
				return;
			}

			var direction = (Mathf.Abs(eventData.scrollDelta.x) > Mathf.Abs(eventData.scrollDelta.y))
				? eventData.scrollDelta.x
				: eventData.scrollDelta.y;
			if (direction > 0)
			{
				Next();
			}
			else
			{
				Prev();
			}
		}

		/// <summary>
		/// Determines whether direction is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		public virtual bool IsHorizontal()
		{
			if (Direction == PaginatorDirection.Horizontal)
			{
				return true;
			}

			if (Direction == PaginatorDirection.Vertical)
			{
				return false;
			}

			if (!IsValid)
			{
				return true;
			}

			if (ScrollRect.horizontal)
			{
				return true;
			}

			if (ScrollRect.vertical)
			{
				return false;
			}

			var rect = ScrollRect.content.rect;

			return rect.width >= rect.height;
		}

		/// <summary>
		/// Gets the size of the page.
		/// </summary>
		/// <returns>The page size.</returns>
		protected virtual float GetPageSize()
		{
			if (!IsValid)
			{
				return 1f;
			}

			if (PageSizeType == PageSizeType.Fixed)
			{
				return PageSize + PageSpacing;
			}

			var size = IsHorizontal()
				? ScrollRect.viewport.rect.width + PageSpacing
				: ScrollRect.viewport.rect.height + PageSpacing;

			if (Layout != null)
			{
				size -= IsHorizontal() ? Layout.MarginHorizontal : Layout.MarginVertical;
			}

			return size;
		}

		/// <summary>
		/// Get ScrollRect size.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ScrollRectSize()
		{
			if (!IsValid)
			{
				return 0f;
			}

			var size = ScrollRect.viewport.rect.size;

			return IsHorizontal() ? size.x : size.y;
		}

		/// <summary>
		/// Can be dragged?
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise, false.</returns>
		protected virtual bool IsValidDrag(PointerEventData eventData)
		{
			if (!gameObject.activeInHierarchy)
			{
				return false;
			}

			if (eventData.button != DragButton)
			{
				return false;
			}

			if (!ScrollRect.IsActive())
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Happens when ScrollRect OnDragStart event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnScrollRectDragStart(PointerEventData eventData)
		{
			if (!IsValidDrag(eventData))
			{
				return;
			}

			DragDelta = Vector2.zero;

			isDragging = true;

			CursorStartPosition = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(ScrollRect.viewport, eventData.position, eventData.pressEventCamera, out CursorStartPosition);

			DragStarted = UtilitiesTime.GetTime(UnscaledTime);

			StopAnimation();
		}

		/// <summary>
		/// The drag delta.
		/// </summary>
		protected Vector2 DragDelta = Vector2.zero;

		/// <summary>
		/// Time when drag started.
		/// </summary>
		protected float DragStarted;

		/// <summary>
		/// Happens when ScrollRect OnDrag event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnScrollRectDrag(PointerEventData eventData)
		{
			MovementInvoke();

			if (!isDragging)
			{
				return;
			}

			if (!IsValidDrag(eventData))
			{
				OnScrollRectDragEnd(eventData);
				return;
			}

			RectTransformUtility.ScreenPointToLocalPointInRectangle(ScrollRect.viewport, eventData.position, eventData.pressEventCamera, out var current_cursor);

			DragDelta = current_cursor - CursorStartPosition;
		}

		/// <summary>
		/// Happens when ScrollRect OnDragEnd event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnScrollRectDragEnd(PointerEventData eventData)
		{
			if (!isDragging)
			{
				return;
			}

			isDragging = false;
			if (ForcedPosition != PaginatorPagePosition.None)
			{
				ScrollChanged();
			}
		}

		/// <summary>
		/// Happens when ScrollRect onValueChanged event occurs.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void OnScrollRectValueChanged(Vector2 value)
		{
			if (isAnimationRunning || !gameObject.activeInHierarchy || isDragging)
			{
				return;
			}

			if (ForcedPosition == PaginatorPagePosition.None)
			{
				var page = Mathf.Clamp(GetPage(), 0, Pages - 1);
				if (currentPage != page)
				{
					UpdateObjects(page);
					currentPage = page;
				}
			}
			else
			{
				ScrollChanged();
			}

			MovementInvoke();
		}

		/// <summary>
		/// Get current page.
		/// </summary>
		/// <returns>Page.</returns>
		protected virtual int GetPage()
		{
			var position = GetCalculatedPosition();
			var page = Rounding(position / GetPageSize());
			if (page >= Pages)
			{
				page = Pages - 1;
			}

			return page;
		}

		/// <summary>
		/// Rounding page.
		/// </summary>
		/// <param name="page">Page.</param>
		/// <returns>Rounded page.</returns>
		protected int Rounding(float page)
		{
			return PageRounding switch
			{
				PaginatorRounding.Floor => Mathf.FloorToInt(page),
				PaginatorRounding.Round => Mathf.RoundToInt(page),
				PaginatorRounding.Ceil => Mathf.CeilToInt(page),
				_ => throw new NotSupportedException(string.Format("Unknown PageRounding: {0}", EnumHelper<PaginatorRounding>.ToString(PageRounding))),
			};
		}

		/// <summary>
		/// Handle scroll changes.
		/// </summary>
		protected virtual void ScrollChanged()
		{
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

			var distance = Mathf.Abs(IsHorizontal() ? DragDelta.x : DragDelta.y);
			var time = UtilitiesTime.GetTime(UnscaledTime) - DragStarted;

			var is_fast = (distance >= FastDragDistance) && (time <= FastDragTime);
			if (is_fast)
			{
				var direction = IsHorizontal() ? DragDelta.x : -DragDelta.y;
				DragDelta = Vector2.zero;

				if (direction == 0f)
				{
					return;
				}

				var page = direction < 0 ? CurrentPage + 1 : CurrentPage - 1;
				GoToPage(page, true);
			}
			else
			{
				GoToPage(GetPage(), true);

				DragDelta = Vector2.zero;
				DragStarted = 0f;
			}
		}

		/// <summary>
		/// Gets the size of the content.
		/// </summary>
		/// <returns>The content size.</returns>
		public virtual float GetContentSize()
		{
			if (!IsValid)
			{
				return 0f;
			}

			return IsHorizontal() ? ScrollRect.content.rect.width : ScrollRect.content.rect.height;
		}

		/// <summary>
		/// Go to page without animation.
		/// </summary>
		/// <param name="page">Page.</param>
		public virtual void SetPage(int page)
		{
			if (!IsValid)
			{
				currentPage = 0;
				return;
			}

			page = Mathf.Clamp(page, 0, Pages - 1);

			StopAnimation();
			ScrollRect.StopMovement();

			SetPosition(Page2Position(page), IsHorizontal());

			UpdateObjects(page);

			currentPage = page;

			OnPageSelect.Invoke(currentPage);
		}

		/// <summary>
		/// Recalculate the pages count.
		/// </summary>
		protected virtual void RecalculatePages()
		{
			SetScrollRectMaxDrag();

			var size = GetContentSize() + PageSpacing;
			if (Layout != null)
			{
				size -= IsHorizontal() ? Layout.MarginFullHorizontal : Layout.MarginFullVertical;
			}

			var page_size = GetPageSize();
			Pages = IsValid ? Mathf.Max(1, Mathf.CeilToInt((size - RoundingError) / page_size)) : 0;

			UpdateLastPageMargin();

			if (currentPage >= Pages)
			{
				GoToPage(Pages - 1);
			}
			else if (ForcedPosition != PaginatorPagePosition.None)
			{
				SetPage(CurrentPage);
			}
		}

		/// <summary>
		/// Set ScrollRect max drag value.
		/// </summary>
		protected virtual void SetScrollRectMaxDrag()
		{
			if (!IsValid)
			{
				return;
			}

			var scrollRectDrag = ScrollRect as ScrollRectRestrictedDrag;
			if (scrollRectDrag != null)
			{
				if (IsHorizontal())
				{
					scrollRectDrag.MaxDrag.x = GetPageSize();
					scrollRectDrag.MaxDrag.y = 0;
				}
				else
				{
					scrollRectDrag.MaxDrag.x = 0;
					scrollRectDrag.MaxDrag.y = GetPageSize();
				}
			}
		}

		/// <summary>
		/// Gets the page position.
		/// </summary>
		/// <returns>The page position.</returns>
		/// <param name="page">Page.</param>
		public virtual float Page2Position(int page)
		{
			var result = page * GetPageSize();

			var delta = ScrollRectSize() - GetPageSize();
			switch (ForcedPosition)
			{
				case PaginatorPagePosition.None:
				case PaginatorPagePosition.OnStart:
					break;
				case PaginatorPagePosition.OnCenter:
					result -= delta / 2f;
					break;
				case PaginatorPagePosition.OnEnd:
					result -= delta;
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown forced position: {0}", EnumHelper<PaginatorPagePosition>.ToString(ForcedPosition)));
			}

			return result;
		}

		/// <summary>
		/// Stop animation.
		/// </summary>
		public virtual void StopAnimation()
		{
			if (!isAnimationRunning)
			{
				return;
			}

			isAnimationRunning = false;
			if (currentAnimation != null)
			{
				StopCoroutine(currentAnimation);
				currentAnimation = null;

				var position = Page2Position(currentPage);
				SetPosition(position, IsHorizontal());

				ScrollRectRestore();
			}
		}

		/// <inheritdoc/>
		protected override void GoToPage(int page, bool forceUpdate)
		{
			if (!IsValid)
			{
				UpdateObjects(0);
				return;
			}

			page = Mathf.Clamp(page, 0, Pages - 1);
			if ((currentPage == page) && (!forceUpdate))
			{
				UpdateObjects(page);
				return;
			}

			StopAnimation();

			var end_position = Page2Position(page);
			if (GetPosition() == end_position)
			{
				UpdateObjects(page);
				return;
			}

			ScrollRect.StopMovement();

			if (CanAnimate())
			{
				StartAnimation(end_position);
			}
			else
			{
				SetPosition(end_position, IsHorizontal());
			}

			UpdateObjects(page);

			currentPage = page;

			OnPageSelect.Invoke(currentPage);
		}

		/// <summary>
		/// Can animate?
		/// </summary>
		/// <returns>true if animation allowed; otherwise false.</returns>
		protected virtual bool CanAnimate() => Animation;

		/// <summary>
		/// Start animation.
		/// </summary>
		/// <param name="target">Target position.</param>
		protected virtual void StartAnimation(float target)
		{
			isAnimationRunning = true;
			ScrollRectStop();
			currentAnimation = RunAnimation(IsHorizontal(), GetPosition(), target, UnscaledTime);
			StartCoroutine(currentAnimation);
		}

		/// <summary>
		/// Saved ScrollRect.horizontal value.
		/// </summary>
		protected bool ScrollRectHorizontal;

		/// <summary>
		/// Saved ScrollRect.vertical value.
		/// </summary>
		protected bool ScrollRectVertical;

		/// <summary>
		/// Save ScrollRect state and disable scrolling.
		/// </summary>
		protected virtual void ScrollRectStop()
		{
			if (!IsValid)
			{
				return;
			}

			ScrollRectHorizontal = ScrollRect.horizontal;
			ScrollRectVertical = ScrollRect.vertical;

			ScrollRect.horizontal = false;
			ScrollRect.vertical = false;
		}

		/// <summary>
		/// Restore ScrollRect state.
		/// </summary>
		protected virtual void ScrollRectRestore()
		{
			if (!IsValid)
			{
				return;
			}

			ScrollRect.horizontal = ScrollRectHorizontal;
			ScrollRect.vertical = ScrollRectVertical;
		}

		/// <summary>
		/// Set ScrollRect content position.
		/// </summary>
		/// <returns>Position.</returns>
		public virtual float GetPosition()
		{
			if (!IsValid)
			{
				return 0f;
			}

			return IsHorizontal() ? -ScrollRect.content.anchoredPosition.x : ScrollRect.content.anchoredPosition.y;
		}

		/// <summary>
		/// Get position with PaginatorPagePosition included.
		/// </summary>
		/// <returns>Position.</returns>
		protected virtual float GetCalculatedPosition()
		{
			var position = GetPosition();
			var delta = ScrollRectSize() - GetPageSize();
			switch (ForcedPosition)
			{
				case PaginatorPagePosition.None:
				case PaginatorPagePosition.OnStart:
					break;
				case PaginatorPagePosition.OnCenter:
					position += (IsHorizontal() ? delta : -delta) / 2f;
					break;
				case PaginatorPagePosition.OnEnd:
					position += IsHorizontal() ? delta : -delta;
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown forced position: {0}", EnumHelper<PaginatorPagePosition>.ToString(ForcedPosition)));
			}

			return position;
		}

		/// <summary>
		/// Set ScrollRect content position.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="isHorizontal">Is horizontal direction.</param>
		protected virtual void SetPosition(float position, bool isHorizontal)
		{
			if (!IsValid)
			{
				return;
			}

			ScrollRect.content.anchoredPosition = isHorizontal
				? new Vector2(-position, ScrollRect.content.anchoredPosition.y)
				: new Vector2(ScrollRect.content.anchoredPosition.x, position);

			MovementInvoke();
		}

		/// <summary>
		/// Set ScrollRect content position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void SetPosition(float position) => SetPosition(position, IsHorizontal());

		/// <summary>
		/// Runs the animation.
		/// </summary>
		/// <returns>The animation.</returns>
		/// <param name="isHorizontal">If set to <c>true</c> is horizontal.</param>
		/// <param name="startPosition">Start position.</param>
		/// <param name="endPosition">End position.</param>
		/// <param name="unscaledTime">If set to <c>true</c> use unscaled time.</param>
		protected virtual IEnumerator RunAnimation(bool isHorizontal, float startPosition, float endPosition, bool unscaledTime)
		{
			var duration = Movement[Movement.length - 1].time;
			var time = 0f;

			do
			{
				var position = Mathf.Lerp(startPosition, endPosition, Movement.Evaluate(time));
				SetPosition(position, isHorizontal);

				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			SetPosition(endPosition, isHorizontal);

			ScrollRectRestore();

			isAnimationRunning = false;
			currentAnimation = null;
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			RemoveListeners(ScrollRect);
		}

		#if UNITY_EDITOR
		/// <inheritdoc/>
		protected override void OnValidate()
		{
			base.OnValidate();

			Compatibility.Upgrade(this);
		}
		#endif

		[SerializeField]
		[HideInInspector]
		int version = 0;

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public virtual void Upgrade()
		{
			if (version == 0)
			{
				if (forceScrollOnPage)
				{
					ForcedPosition = PaginatorPagePosition.OnStart;
				}

				version = 1;
			}

			#if UNITY_EDITOR
			if (version == 1)
			{
				#pragma warning disable 0618
				UnityEditor.Undo.RecordObject(this, "Paginator Upgrade");
				view.SetTemplates(DefaultPage, ActivePage, NextPage, PrevPage, SkipPage, pagesContainer, visiblePagesCount);
				DefaultPage = null;
				ActivePage = null;
				NextPage = null;
				PrevPage = null;
				SkipPage = null;
				pagesContainer = null;
				#pragma warning restore 0618

				version = 2;
			}
			#endif
		}
	}
}