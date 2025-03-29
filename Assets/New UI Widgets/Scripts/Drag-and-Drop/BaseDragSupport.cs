namespace UIWidgets
{
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Used only to attach custom editor to DragSupport.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/drag-and-drop.html")]
	public abstract class BaseDragSupport : UIBehaviourInitiable
	{
		/// <summary>
		/// Delay.
		/// </summary>
		protected class Delay
		{
			readonly BaseDragSupport owner;

			float timer;

			ScrollRect scrollRect;

			IEnumerator DragTimerCoroutine;

			/// <summary>
			/// Is delay finished?
			/// </summary>
			public bool Done => timer >= owner.DragDelay;

			/// <summary>
			/// Initializes a new instance of the <see cref="Delay"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public Delay(BaseDragSupport owner)
			{
				this.owner = owner;
				timer = 0f;
				scrollRect = null;
				DragTimerCoroutine = null;

				UpdateScrollRect();
			}

			/// <summary>
			/// Update ScrollRect.
			/// </summary>
			public void UpdateScrollRect()
			{
				scrollRect = owner.GetComponentInParent<ScrollRect>();
			}

			/// <summary>
			/// Start delay counter.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			public void Start(PointerEventData eventData)
			{
				Stop();

				if (owner.DragDelay > 0f)
				{
					DragTimerCoroutine = DragTimer();
					owner.StartCoroutine(DragTimerCoroutine);
					OnInitializePotentialDrag(eventData);
				}
			}

			/// <summary>
			/// Stop.
			/// </summary>
			public void Stop()
			{
				if (DragTimerCoroutine != null)
				{
					owner.StopCoroutine(DragTimerCoroutine);
					DragTimerCoroutine = null;
				}
			}

			IEnumerator DragTimer()
			{
				timer = 0f;

				while (timer < owner.DragDelay)
				{
					yield return null;
					timer += UtilitiesTime.GetDeltaTime(owner.UnscaledTime);
				}

				DragTimerCoroutine = null;
			}

			void OnInitializePotentialDrag(PointerEventData eventData)
			{
				if (owner.RedirectDragToScrollRect && (scrollRect != null))
				{
					scrollRect.OnInitializePotentialDrag(eventData);
				}
			}

			/// <summary>
			/// Send OnBeginDrag event to the ScrollRect.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			public void OnBeginDrag(PointerEventData eventData)
			{
				if (owner.RedirectDragToScrollRect && (scrollRect != null))
				{
					scrollRect.OnBeginDrag(eventData);
				}
			}

			/// <summary>
			/// Send OnDrag event to the ScrollRect.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			public void OnDrag(PointerEventData eventData)
			{
				if (owner.RedirectDragToScrollRect && (scrollRect != null))
				{
					scrollRect.OnDrag(eventData);
				}
			}

			/// <summary>
			/// Send OnEndDrag event to the ScrollRect.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			public void OnEndDrag(PointerEventData eventData)
			{
				if (owner.RedirectDragToScrollRect && (scrollRect != null))
				{
					scrollRect.OnEndDrag(eventData);
				}
			}
		}

		/// <summary>
		/// Allow drag.
		/// </summary>
		[SerializeField]
		public bool AllowDrag = true;

		/// <summary>
		/// Cursors.
		/// </summary>
		[SerializeField]
		public Cursors Cursors;

		[SerializeField]
		[FormerlySerializedAs("dragHandle")]
		DragSupportHandle handle;

		/// <summary>
		/// Select this GameObject by EventSystem on start drag.
		/// </summary>
		[SerializeField]
		[Tooltip("Select this GameObject by EventSystem on start drag.")]
		public bool EventSystemSelect = true;

		/// <summary>
		/// Drag handle.
		/// </summary>
		public DragSupportHandle Handle
		{
			get => handle;

			set
			{
				if (handle != null)
				{
					handle.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
					handle.OnBeginDragEvent.RemoveListener(OnBeginDrag);
					handle.OnDragEvent.RemoveListener(OnDrag);
					handle.OnEndDragEvent.RemoveListener(OnEndDrag);
				}

				handle = (value != null)
					? value
					: Utilities.RequireComponent<DragSupportHandle>(this);

				if (handle != null)
				{
					handle.OnInitializePotentialDragEvent.AddListener(OnInitializePotentialDrag);
					handle.OnBeginDragEvent.AddListener(OnBeginDrag);
					handle.OnDragEvent.AddListener(OnDrag);
					handle.OnEndDragEvent.AddListener(OnEndDrag);
				}
			}
		}

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// How many seconds must pass from the click to the start of dragging.
		/// </summary>
		[SerializeField]
		[Tooltip("How many seconds must pass from the click to the start of dragging.")]
		public float DragDelay = 0.0f;

		/// <summary>
		/// Redirects the drag events to the ScrollRect if the DragDelay time is not passed.
		/// </summary>
		[Tooltip("Redirects the drag events to the ScrollRect if the DragDelay time is not passed.")]
		[SerializeField]
		public bool RedirectDragToScrollRect = false;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Event on start drag.
		/// </summary>
		[SerializeField]
		public UnityEvent StartDragEvent = new UnityEvent();

		/// <summary>
		/// Event on end drag.
		/// </summary>
		[SerializeField]
		public UnityEvent EndDragEvent = new UnityEvent();

		Delay delayTimer;

		/// <summary>
		/// Delay timer.
		/// </summary>
		protected Delay DelayTimer
		{
			get
			{
				delayTimer ??= new Delay(this);

				return delayTimer;
			}
		}

		/// <summary>
		/// Process parent changed event.
		/// </summary>
		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();

			DelayTimer.UpdateScrollRect();
		}

		/// <summary>
		/// The current drop target.
		/// </summary>
		protected IAutoScroll CurrentAutoScrollTarget;

		/// <summary>
		/// If this object is dragged?
		/// </summary>
		protected bool IsDragged;

		/// <summary>
		/// The drag points.
		/// </summary>
		protected static Dictionary<InstanceID, RectTransform> DragPoints = new Dictionary<InstanceID, RectTransform>();

		RectTransform parentCanvas;

		/// <summary>
		/// Gets a canvas transform of current GameObject.
		/// </summary>
		protected RectTransform ParentCanvas
		{
			get
			{
				if (parentCanvas == null)
				{
					parentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
				}

				return parentCanvas;
			}
		}

		/// <summary>
		/// Gets the drag point.
		/// </summary>
		public RectTransform DragPoint
		{
			get
			{
				var key = new InstanceID(ParentCanvas);
				var contains_key = DragPoints.ContainsKey(key);
				if (!contains_key || (DragPoints[key] == null))
				{
					var go = new GameObject("DragPoint");
					var dragPoint = go.AddComponent<RectTransform>();
					dragPoint.SetParent(ParentCanvas, false);
					dragPoint.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
					dragPoint.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
					dragPoint.pivot = new Vector2(0f, 1f);

					DragPoints[key] = dragPoint;
				}

				return DragPoints[key];
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Handle = handle;
		}

		/// <summary>
		/// Determines whether this instance can be dragged.
		/// </summary>
		/// <returns><c>true</c> if this instance can be dragged; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Current event data.</param>
		public virtual bool CanDrag(PointerEventData eventData)
		{
			return AllowDrag && (eventData.button == DragButton);
		}

		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected abstract void InitDrag(PointerEventData eventData);

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public abstract void Dropped(bool success);

		/// <summary>
		/// Process OnInitializePotentialDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected abstract void OnInitializePotentialDrag(PointerEventData eventData);

		/// <summary>
		/// Process OnBeginDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected virtual void OnBeginDrag(PointerEventData eventData)
		{
			Init();
			DelayTimer.Stop();

			if (!DelayTimer.Done)
			{
				DelayTimer.OnBeginDrag(eventData);
				return;
			}

			if (!CanDrag(eventData))
			{
				return;
			}

			StartDragEvent.Invoke();

			if (EventSystemSelect)
			{
				EventSystem.current.SetSelectedGameObject(gameObject);
			}

			IsDragged = true;
			InitDrag(eventData);

			FindCurrentTarget(eventData);
		}

		/// <summary>
		/// Find current target.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected abstract void FindCurrentTarget(PointerEventData eventData);

		/// <summary>
		/// Get allowed cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetAllowedCursor()
		{
			if (Cursors != null)
			{
				return Cursors.Allowed;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.Allowed;
			}

			return default;
		}

		/// <summary>
		/// Get denied cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetDeniedCursor()
		{
			if (Cursors != null)
			{
				return Cursors.Denied;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.Denied;
			}

			return default;
		}

		/// <summary>
		/// Process OnDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected virtual void OnDrag(PointerEventData eventData)
		{
			if (!DelayTimer.Done)
			{
				DelayTimer.OnDrag(eventData);
				return;
			}

			if (!IsDragged)
			{
				return;
			}

			if (!CanDrag(eventData))
			{
				OnEndDrag(eventData);
				return;
			}

			FindCurrentTarget(eventData);

			RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentCanvas, eventData.position, eventData.pressEventCamera, out var point);

			DragPoint.localPosition = point;

			var target = FindAutoScrollTarget(eventData);

			if (target != CurrentAutoScrollTarget)
			{
				if (!Utilities.IsNull(CurrentAutoScrollTarget))
				{
					CurrentAutoScrollTarget.AutoScrollStop();
				}

				CurrentAutoScrollTarget = target;
			}

			if (!Utilities.IsNull(CurrentAutoScrollTarget))
			{
				CurrentAutoScrollTarget.AutoScrollStart(eventData, OnDrag);
			}
		}

		/// <summary>
		/// Finds the auto-scroll target.
		/// </summary>
		/// <returns>The auto-scroll target.</returns>
		/// <param name="eventData">Event data.</param>
		protected virtual IAutoScroll FindAutoScrollTarget(PointerEventData eventData)
		{
			using var _ = ListPool<RaycastResult>.Get(out var ray_casts);

			EventSystem.current.RaycastAll(eventData, ray_casts);

			foreach (var ray_cast in ray_casts)
			{
				if (!ray_cast.isValid)
				{
					continue;
				}

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				var target = ray_cast.gameObject.GetComponent<IAutoScroll>();
#else
				var target = raycast.gameObject.GetComponent(typeof(IAutoScroll)) as IAutoScroll;
#endif
				if (!Utilities.IsNull(target))
				{
					return target;
				}
			}

			return null;
		}

		/// <summary>
		/// Process OnEndDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected abstract void OnEndDrag(PointerEventData eventData);

		/// <summary>
		/// Reset cursor.
		/// </summary>
		protected void ResetCursor()
		{
			IsDragged = false;
			UICursor.Reset(this);
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			if ((DragPoints != null) && (ParentCanvas != null))
			{
				var key = new InstanceID(ParentCanvas);
				if (DragPoints.ContainsKey(key))
				{
					DragPoints.Remove(key);
				}
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Process the cancel event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public abstract void OnCancel(BaseEventData eventData);

#if UNITY_EDITOR

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected override void Reset()
		{
			CursorsDPISelector.Require(this);

			base.Reset();
		}

#endif

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(DragPoints))]
		static void StaticInit()
		{
			DragPoints.Clear();
		}
#endif
	}
}