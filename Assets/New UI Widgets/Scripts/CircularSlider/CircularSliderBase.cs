namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for CircularSliders.
	/// </summary>
	/// <typeparam name="T">Type of value.</typeparam>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(RingEffect))]
	[DataBindSupport]
	#if UNITY_2018_4_OR_NEWER
	[ExecuteAlways]
	#else
	[ExecuteInEditMode]
	#endif
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/circular-slider.html")]
	public abstract class CircularSliderBase<T> : UIBehaviourInteractable, IStylable,
		IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerDownHandler, IPointerClickHandler, IPointerUpHandler,
		IScrollHandler
	{
		/// <summary>
		/// Scroll modes.
		/// </summary>
		public enum ScrollModes
		{
			/// <summary>
			/// Ignore.
			/// </summary>
			Ignore = 0,

			/// <summary>
			/// Increase on scroll up.
			/// </summary>
			UpIncrease = 1,

			/// <summary>
			/// Increase on scroll down.
			/// </summary>
			UpDecrease = 2,
		}

		[SerializeField]
		DragListener handle;

		/// <summary>
		/// Handle.
		/// </summary>
		public DragListener Handle
		{
			get => handle;

			set
			{
				if (handle != null)
				{
					handle.OnBeginDragEvent.RemoveListener(OnBeginDrag);
					handle.OnDragEvent.RemoveListener(OnDrag);
					handle.OnEndDragEvent.RemoveListener(OnEndDrag);
				}

				handle = value;

				if (handle != null)
				{
					handle.OnBeginDragEvent.AddListener(OnBeginDrag);
					handle.OnDragEvent.AddListener(OnDrag);
					handle.OnEndDragEvent.AddListener(OnEndDrag);
				}

				UpdateTracker();
				UpdatePositions();
			}
		}

		[SerializeField]
		RectTransform arrow;

		/// <summary>
		/// Arrow.
		/// </summary>
		public RectTransform Arrow
		{
			get => arrow;

			set
			{
				arrow = value;

				UpdateTracker();
				UpdatePositions();
			}
		}

		[SerializeField]
		[Range(0, 359)]
		float startAngle = 0;

		/// <summary>
		/// Start angle.
		/// </summary>
		public float StartAngle
		{
			get => startAngle;

			set
			{
				startAngle = value;
				UpdatePositions();
			}
		}

		[SerializeField]
		T minValue;

		/// <summary>
		/// Min value.
		/// </summary>
		[DataBindField]
		public T MinValue
		{
			get => minValue;

			set
			{
				minValue = value;
				Value = this.value;
			}
		}

		[SerializeField]
		T maxValue;

		/// <summary>
		/// Max value.
		/// </summary>
		[DataBindField]
		public T MaxValue
		{
			get => maxValue;

			set
			{
				maxValue = value;
				Value = this.value;
			}
		}

		[SerializeField]
		T value;

		/// <summary>
		/// Value.
		/// </summary>
		[DataBindField]
		public T Value
		{
			get => value;

			set
			{
				var v = ClampValue(value);
				if (!EqualityComparer<T>.Default.Equals(this.value, v))
				{
					this.value = v;
					UpdatePositions();
					InvokeValueChanged(this.value);
				}
			}
		}

		[SerializeField]
		T step;

		/// <summary>
		/// Step.
		/// </summary>
		[DataBindField]
		public T Step
		{
			get => step;

			set
			{
				step = value;
				Value = this.value;
			}
		}

		/// <summary>
		/// Scroll mode.
		/// </summary>
		[SerializeField]
		public ScrollModes ScrollMode = ScrollModes.UpIncrease;

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Value changed event.
		/// </summary>
		[SerializeField]
		[DataBindEvent(nameof(Value))]
		public UnityEvent OnChange = new UnityEvent();

		/// <summary>
		/// Required delayed update.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected bool DelayedUpdate;

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

		RingEffect ring;

		/// <summary>
		/// Ring effect.
		/// </summary>
		public RingEffect Ring
		{
			get
			{
				if (ring == null)
				{
					TryGetComponent(out ring);
				}

				return ring;
			}
		}

		/// <summary>
		/// Properties tracker.
		/// </summary>
		protected DrivenRectTransformTracker PropertiesTracker;

		/// <summary>
		/// Driver properties.
		/// </summary>
		protected DrivenTransformProperties DrivenProperties = DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Anchors | DrivenTransformProperties.Pivot | DrivenTransformProperties.Rotation;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Handle = handle;
			Arrow = arrow;
			Value = value;
		}

		/// <summary>
		/// Clamp value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Value in range [MinValue, MaxValue].</returns>
		protected abstract T ClampValue(T value);

		/// <summary>
		/// Update tracker.
		/// </summary>
		protected virtual void UpdateTracker()
		{
			PropertiesTracker.Clear();

			if (Handle != null)
			{
				PropertiesTracker.Add(this, Handle.RectTransform, DrivenProperties);

				var v05 = new Vector2(0.5f, 0.5f);
				Handle.RectTransform.anchorMin = v05;
				Handle.RectTransform.anchorMax = v05;
				Handle.RectTransform.pivot = v05;
			}

			if (Arrow != null)
			{
				PropertiesTracker.Add(this, Arrow, DrivenProperties);

				Arrow.anchorMin = new Vector2(0.5f, 0.5f);
				Arrow.anchorMax = new Vector2(0.5f, 0.5f);
				Arrow.pivot = new Vector2(0, 0.5f);
				Arrow.anchoredPosition = Vector2.zero;
			}
		}

		/// <summary>
		/// Update positions.
		/// </summary>
		protected virtual void UpdatePositions()
		{
			var size = RectTransform.rect.size / 2f;
			size.x -= Ring.Thickness / 2f;
			size.y -= Ring.Thickness / 2f;

			var angle = -(Value2Angle(Value) + StartAngle) % 360;
			var rotation = Quaternion.Euler(0f, 0f, angle);

			var pos = new Vector2(
				size.x * Mathf.Cos(angle * Mathf.Deg2Rad),
				size.y * Mathf.Sin(angle * Mathf.Deg2Rad));

			if (Handle != null)
			{
				Handle.RectTransform.rotation = rotation;
				Handle.RectTransform.anchoredPosition = pos;
			}

			if (Arrow != null)
			{
				Arrow.rotation = rotation;
				Arrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pos.magnitude);
			}
		}

		/// <summary>
		/// Convert angle to value.
		/// </summary>
		/// <param name="angle">Angle.</param>
		/// <returns>Value.</returns>
		public abstract T Angle2Value(float angle);

		/// <summary>
		/// Convert value to angle.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Angle.</returns>
		public abstract float Value2Angle(T value);

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData)
		{
			return IsInteractable() && (eventData.button == DragButton);
		}

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			Rotate(eventData);
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			Rotate(eventData);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			Rotate(eventData);
		}

		/// <summary>
		/// Process pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			// do nothing, without it OnPointerClick does not work
		}

		/// <summary>
		/// Process pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			// do nothing, without it OnPointerClick does not work
		}

		/// <summary>
		/// Process pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			Rotate(eventData);
		}

		/// <summary>
		/// Increase value on step.
		/// </summary>
		protected abstract void Increase();

		/// <summary>
		/// Decrease value on step.
		/// </summary>
		protected abstract void Decrease();

		/// <summary>
		/// Process the scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnScroll(PointerEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			switch (ScrollMode)
			{
				case ScrollModes.UpIncrease:
					if (eventData.scrollDelta.y > 0)
					{
						Increase();
					}
					else
					{
						Decrease();
					}

					break;
				case ScrollModes.UpDecrease:
					if (eventData.scrollDelta.y > 0)
					{
						Decrease();
					}
					else
					{
						Increase();
					}

					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Rotate this instance using specified event data.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void Rotate(PointerEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out var point);

			var base_angle = -DragPoint2Angle(point);
			Value = Angle2Value(base_angle - StartAngle);
		}

		/// <summary>
		/// Convert drag point to angle.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Angle.</returns>
		protected float DragPoint2Angle(Vector2 point)
		{
			var size = RectTransform.rect.size;
			var relative = new Vector2(
				point.x + (size.x * (RectTransform.pivot.x - 0.5f)),
				point.y + (size.y * (RectTransform.pivot.y - 0.5f)));

			return Point2Angle(relative);
		}

		/// <summary>
		/// Convert point to the angle.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Angle in range [-180f, 180f].</returns>
		public static float Point2Angle(Vector2 point)
		{
			var angle_rad = Mathf.Atan2(point.y, point.x);
			var angle = angle_rad * Mathf.Rad2Deg;

			angle %= 360f;

			if (angle > 180f)
			{
				angle -= 360f;
			}

			return angle;
		}

		/// <summary>
		/// Invoke value changed events.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void InvokeValueChanged(T value)
		{
			OnChange.Invoke();
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Process the update event.
		/// </summary>
		protected virtual void Update()
		{
			if (DelayedUpdate)
			{
				DelayedUpdate = false;

				UpdateTracker();
				UpdatePositions();
			}
		}

		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			value = ClampValue(value);

			DelayedUpdate = true;
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var ring))
			{
				style.CircularSlider.Ring.ApplyTo(ring);
			}

			if (Ring != null)
			{
				Ring.RingColor = style.CircularSlider.RingColor;
			}

			if ((Handle != null) && Handle.TryGetComponent<Image>(out var h))
			{
				style.CircularSlider.Handle.ApplyTo(h);
			}

			if ((Arrow != null) && Arrow.TryGetComponent<Image>(out var a))
			{
				style.CircularSlider.Arrow.ApplyTo(a);
			}

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var ring))
			{
				style.CircularSlider.Ring.GetFrom(ring);
			}

			if (Ring != null)
			{
				style.CircularSlider.RingColor = Ring.RingColor;
			}

			if ((Handle != null) && Handle.TryGetComponent<Image>(out var h))
			{
				style.CircularSlider.Handle.GetFrom(h);
			}

			if ((Arrow != null) && Arrow.TryGetComponent<Image>(out var a))
			{
				style.CircularSlider.Arrow.GetFrom(a);
			}

			return true;
		}
		#endregion
	}
}