namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;

	/// <summary>
	/// Base class for the ScrollBlock.
	/// </summary>
	[RequireComponent(typeof(EasyLayoutNS.EasyLayout))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/scroll-block.html")]
	[DataBindSupport]
	public abstract class ScrollBlockBase : UIBehaviourConditional, IStylable, ILateUpdatable,
		IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler, IScrollHandler
	{
		/// <summary>
		/// Position.
		/// </summary>
		public enum Position
		{
			/// <summary>
			/// Start.
			/// </summary>
			Start = 0,

			/// <summary>
			/// Center.
			/// </summary>
			Center = 1,
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		public static void DoNothing()
		{
		}

		/// <summary>
		/// Default function to get value.
		/// </summary>
		[DomainReloadExclude]
		public static readonly Func<int, string> DefaultValue = step => string.Format("Index: {0}", step.ToString());

		/// <summary>
		/// Default function to check is interactable.
		/// </summary>
		[DomainReloadExclude]
		protected static readonly Func<bool> DefaultInteractable = () => true;

		/// <summary>
		/// Default function to check is action allowed.
		/// </summary>
		[DomainReloadExclude]
		protected static readonly Func<bool> DefaultAllow = () => true;

		/// <summary>
		/// Action to increase the value.
		/// </summary>
		[DataBindField]
		public Action Increase = DoNothing;

		/// <summary>
		/// Action to decrease the value.
		/// </summary>
		[DataBindField]
		public Action Decrease = DoNothing;

		/// <summary>
		/// Function to check is increase allowed.
		/// </summary>
		[DataBindField]
		public Func<bool> AllowIncrease = DefaultAllow;

		/// <summary>
		/// Function to check is decrease allowed.
		/// </summary>
		[DataBindField]
		public Func<bool> AllowDecrease = DefaultAllow;

		/// <summary>
		/// Convert index to the displayed string.
		/// </summary>
		[DataBindField]
		public Func<int, string> Value = DefaultValue;

		/// <summary>
		/// Is interactable?
		/// </summary>
		[DataBindField]
		public Func<bool> IsInteractable = DefaultInteractable;

		/// <summary>
		/// Size of the DefaultItem.
		/// </summary>
		public Vector2 DefaultItemSize
		{
			get;
			protected set;
		}

		/// <summary>
		/// Base position.
		/// </summary>
		[SerializeField]
		protected Position basePosition = Position.Center;

		/// <summary>
		/// Base position.
		/// </summary>
		public Position BasePosition => basePosition;

		/// <summary>
		/// Layout.
		/// </summary>
		[NonSerialized]
		protected EasyLayoutBridge Layout;

		/// <summary>
		/// Count of the visible items.
		/// </summary>
		public abstract int Count
		{
			get;
		}

		/// <summary>
		/// Is horizontal scroll?
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("IsHorizontal")]
		protected bool isHorizontal;

		/// <summary>
		/// Is horizontal scroll?
		/// </summary>
		public bool IsHorizontal => isHorizontal;

		/// <summary>
		/// Drag sensitivity.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public float DragSensitivity = 0.5f;

		/// <summary>
		/// Scroll sensitivity.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public float ScrollSensitivity = 15f;

		/// <summary>
		/// Layout internal padding.
		/// </summary>
		public float Padding
		{
			get => Layout.GetFiller().x;

			set
			{
				var padding = ClampPadding(value);
				Layout.SetFiller(padding, 0f);
			}
		}

		/// <summary>
		/// Animate inertia scroll with unscaled time.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Auto-scroll to center after scroll/drag.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public bool AlwaysCenter = true;

		/// <summary>
		/// Time to stop.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(AlwaysCenter))]
		[DataBindField]
		public float TimeToStop = 0.5f;

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Velocity.
		/// </summary>
		[NonSerialized]
		protected float ScrollVelocity;

		/// <summary>
		/// Inertia velocity.
		/// </summary>
		[NonSerialized]
		protected float InertiaVelocity;

		/// <summary>
		/// Current deceleration rate.
		/// </summary>
		[NonSerialized]
		protected float CurrentDecelerationRate;

		/// <summary>
		/// Inertia distance.
		/// </summary>
		[NonSerialized]
		protected float InertiaDistance;

		/// <summary>
		/// Is drag event occurring?
		/// </summary>
		[NonSerialized]
		protected bool IsDragging;

		/// <summary>
		/// Is scrolling occurring?
		/// </summary>
		[NonSerialized]
		protected bool IsScrolling;

		/// <summary>
		/// Previous scroll value.
		/// </summary>
		[NonSerialized]
		protected float PrevScrollValue;

		/// <summary>
		/// Current scroll value.
		/// </summary>
		[NonSerialized]
		protected float CurrentScrollValue;

		RectTransform rectTransform;

		/// <summary>
		/// Current RectTransform.
		/// </summary>
		protected RectTransform RectTransform
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
		/// Base index for the components.
		/// </summary>
		protected abstract int ComponentsBaseIndex
		{
			get;
		}

		/// <summary>
		/// Distance to center.
		/// </summary>
		[Obsolete("Renamed to DistanceToBase.")]
		public float DistanceToCenter => DistanceToBase;

		/// <summary>
		/// Distance to base position.
		/// </summary>
		public float DistanceToBase => GetBasePosition() - Padding;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			UpdateLayout();

			var resizer = Utilities.RequireComponent<ResizeListener>(this);
			resizer.OnResizeNextFrame.AddListener(Resize);

			Resize();

			AlignComponents();
		}

		/// <summary>
		/// Set DefaultItem size.
		/// </summary>
		/// <param name="size">Size.</param>
		public abstract void SetDefaultItemSize(Vector2 size);

		/// <summary>
		/// Set visible items count.
		/// </summary>
		/// <param name="count">Count.</param>
		public abstract void SetVisibleItems(int count);

		/// <summary>
		/// Clamp padding value.
		/// </summary>
		/// <param name="padding">Padding.</param>
		/// <returns>Clamped value.</returns>
		protected float ClampPadding(float padding)
		{
			var current = Padding;
			var delta = current - padding;
			if (delta >= 0)
			{
				return ProcessIncrease(current, delta);
			}
			else
			{
				return ProcessDecrease(current, delta);
			}
		}

		/// <summary>
		/// Process padding decrease.
		/// </summary>
		/// <param name="current">Current padding.</param>
		/// <param name="delta">Padding change.</param>
		/// <returns>Decreased padding.</returns>
		protected float ProcessDecrease(float current, float delta)
		{
			var sign = Mathf.Sign(delta);
			var abs = Mathf.Abs(delta);
			var steps = Mathf.CeilToInt(abs);

			var size = ItemFullSize();
			var base_position = GetBasePosition();
			var update_view = false;

			var padding = current;
			var last = steps - 1;
			for (var i = 0; i < steps; i++)
			{
				var step = (i == last) ? (last * sign) - delta : -sign;
				padding += step;

				var distance = padding - base_position;
				if ((distance > 0) && !AllowDecrease())
				{
					padding = base_position;
					break;
				}

				var decrease = Position2Count(distance / size, false);
				for (var j = 0; j < decrease; j++)
				{
					if (!AllowDecrease())
					{
						break;
					}

					Decrease();
					padding -= size;
					update_view = true;
				}
			}

			if (update_view)
			{
				UpdateView();
			}

			return padding;
		}

		/// <summary>
		/// Process padding increased.
		/// </summary>
		/// <param name="current">Current padding.</param>
		/// <param name="delta">Padding change.</param>
		/// <returns>Increased padding.</returns>
		protected float ProcessIncrease(float current, float delta)
		{
			var sign = Mathf.Sign(delta);
			var abs = Mathf.Abs(delta);
			var steps = Mathf.CeilToInt(abs);

			var size = ItemFullSize();
			var base_position = GetBasePosition();
			var update_view = false;

			var padding = current;
			var last = steps - 1;
			for (var i = 0; i < steps; i++)
			{
				padding += (i == last) ? (last * sign) - delta : -sign;

				var distance = base_position - padding;
				if ((distance > 0) && !AllowIncrease())
				{
					padding = base_position;
					break;
				}

				var increase = Position2Count(distance / size, true);
				for (var j = 0; j < increase; j++)
				{
					if (!AllowIncrease())
					{
						break;
					}

					Increase();
					padding += size;
					update_view = true;
				}
			}

			if (update_view)
			{
				UpdateView();
			}

			return padding;
		}

		/// <summary>
		/// Scroll on specified amount of steps.
		/// </summary>
		/// <param name="steps">Steps.</param>
		/// <param name="curve">Animation curve.</param>
		public void Scroll(int steps, AnimationCurve curve = null)
		{
			StartCoroutine(ScrollAnimation(steps, curve));
		}

		/// <summary>
		/// Scroll animation on specified amount of steps.
		/// </summary>
		/// <param name="steps">Steps.</param>
		/// <param name="curve">Animation curve.</param>
		/// <returns>Animation coroutine.</returns>
		public IEnumerator ScrollAnimation(int steps, AnimationCurve curve = null)
		{
			curve ??= AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

			var direction = steps > 0 ? -1 : 1;
			var total = steps * ItemFullSize() * direction;
			var prev = 0f;

			var time = 0f;
			var duration = curve[curve.length - 1].time;

			while (time < duration)
			{
				var current = Mathf.Lerp(0f, total, time / duration);
				Padding += current - prev;
				prev = current;
				yield return null;

				time += UtilitiesTime.GetDeltaTime(UnscaledTime);
			}

			Padding += total - prev;
		}

		/// <summary>
		/// Position to items count.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="increase">true if increase; otherwise decrease.</param>
		/// <returns>Items count.</returns>
		protected int Position2Count(float position, bool increase)
		{
			return BasePosition switch
			{
				Position.Start => increase ? Mathf.FloorToInt(position) : Mathf.CeilToInt(position),
				Position.Center => Mathf.RoundToInt(position),
				_ => 0,
			};
		}

		/// <summary>
		/// Update the layout.
		/// </summary>
		protected abstract void UpdateLayout();

		/// <summary>
		/// Container size.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ContainerSize()
		{
			return (IsHorizontal ? RectTransform.rect.width : RectTransform.rect.height) - Layout.GetFullMargin();
		}

		/// <summary>
		/// Content size.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ContentSize() => (ItemFullSize() * Count) - Layout.GetSpacing();

		/// <summary>
		/// Item size.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ItemSize() => IsHorizontal ? DefaultItemSize.x : DefaultItemSize.y;

		/// <summary>
		/// Item size with spacing.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ItemFullSize() => ItemSize() + Layout.GetSpacing();

		/// <summary>
		/// Calculate the maximum count of the visible components.
		/// </summary>
		/// <returns>Maximum count of the visible components.</returns>
		protected int CalculateMax()
		{
			var result = Mathf.CeilToInt((ContainerSize() + Layout.GetSpacing()) / ItemFullSize()) + 1;
			if (result < 0)
			{
				result = 0;
			}

			if ((result % 2) == 0)
			{
				result += 1;
			}

			return result;
		}

		/// <summary>
		/// Is component is null?
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if component is null; otherwise, false.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Reviewed.")]
		protected virtual bool IsNullComponent(ScrollBlockItem component) => component == null;

		/// <summary>
		/// Process RectTransform resize.
		/// </summary>
		protected abstract void Resize();

		/// <summary>
		/// Update view.
		/// </summary>
		public abstract void UpdateView();

		/// <summary>
		/// Returns true if the GameObject and the Component are active.
		/// </summary>
		/// <returns>true if the GameObject and the Component are active; otherwise false.</returns>
		public override bool IsActive() => base.IsActive() && IsInited && IsInteractable();

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData)
		{
			return IsActive() && (eventData.button == DragButton);
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

			IsDragging = true;

			PrevScrollValue = Padding;
			CurrentScrollValue = Padding;

			StopInertia();
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!IsDragging)
			{
				return;
			}

			if (!CanDrag(eventData))
			{
				OnEndDrag(eventData);
				return;
			}

			StopInertia();
			var drag_delta = IsHorizontal ? eventData.delta.x : -eventData.delta.y;
			Scroll(drag_delta * DragSensitivity);
		}

		/// <summary>
		/// Process scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnScroll(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			IsScrolling = true;
			var scroll_delta = IsHorizontal ? eventData.scrollDelta.x : -eventData.scrollDelta.y;
			Scroll(scroll_delta * ScrollSensitivity);
		}

		/// <summary>
		/// Scroll.
		/// </summary>
		/// <param name="delta">Delta.</param>
		protected virtual void Scroll(float delta)
		{
			Padding += delta;

			CurrentScrollValue += delta;
			var time_delta = UtilitiesTime.DefaultGetDeltaTime(UnscaledTime);
			var new_velocity = (PrevScrollValue - CurrentScrollValue) / time_delta;
			ScrollVelocity = Mathf.Lerp(ScrollVelocity, new_velocity, time_delta * 10);
			PrevScrollValue = CurrentScrollValue;
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!IsDragging)
			{
				return;
			}

			IsDragging = false;
			InitIntertia();
		}

		/// <summary>
		/// Init inertia.
		/// </summary>
		protected virtual void InitIntertia()
		{
			InertiaVelocity = -ScrollVelocity;
			CurrentDecelerationRate = -InertiaVelocity / TimeToStop;

			var direction = Mathf.Sign(InertiaVelocity);
			var time_to_stop_sq = Mathf.Pow(TimeToStop, 2f);
			var distance = (-Mathf.Abs(CurrentDecelerationRate) * time_to_stop_sq / 2f) + (Mathf.Abs(InertiaVelocity) * TimeToStop);
			InertiaDistance = ClampDistance(distance, direction);
			InertiaVelocity = (InertiaDistance + (Mathf.Abs(CurrentDecelerationRate) * time_to_stop_sq / 2f)) / TimeToStop;
			InertiaVelocity *= direction;
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Late update.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (IsScrolling)
			{
				IsScrolling = false;
				InitIntertia();
			}
			else if (!IsDragging && (Mathf.Abs(InertiaDistance) >= 0.25f) && AlwaysCenter)
			{
				var delta = UtilitiesTime.DefaultGetDeltaTime(UnscaledTime);
				var distance = InertiaVelocity > 0f
					? Mathf.Min(InertiaDistance, InertiaVelocity * delta)
					: Mathf.Max(-InertiaDistance, InertiaVelocity * delta);

				Padding += distance;
				InertiaDistance -= Mathf.Abs(distance) * Mathf.Sign(InertiaDistance);

				if (Mathf.Abs(InertiaDistance) >= 0.25f)
				{
					InertiaVelocity += CurrentDecelerationRate * delta;
					ScrollVelocity = -InertiaVelocity;
				}
				else
				{
					StopInertia();
					Padding = GetBasePosition();
				}
			}
		}

		/// <summary>
		/// Stop inertia.
		/// </summary>
		protected void StopInertia()
		{
			CurrentDecelerationRate = 0f;
			InertiaDistance = 0f;
		}

		/// <summary>
		/// Clamp distance to stop right at value.
		/// </summary>
		/// <param name="distance">Distance.</param>
		/// <param name="direction">Scroll direction.</param>
		/// <returns>Clamped distance.</returns>
		protected float ClampDistance(float distance, float direction)
		{
			var extra = (GetBasePosition() - Padding) * direction;
			var steps = Mathf.Round((Mathf.Abs(distance) - extra) / ItemFullSize());
			var new_distance = (steps * ItemFullSize()) + extra;
			return new_distance;
		}

		/// <summary>
		/// Get zero position.
		/// </summary>
		/// <returns>Position.</returns>
		protected float GetBasePosition()
		{
			return BasePosition switch
			{
				Position.Start => 0f,
				Position.Center => -(ContentSize() - ContainerSize()) / 2f,
				_ => 0f,
			};
		}

		/// <summary>
		/// Align components.
		/// </summary>
		protected void AlignComponents()
		{
			Padding = GetBasePosition();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			if (TryGetComponent<ResizeListener>(out var resizer))
			{
				resizer.OnResizeNextFrame.RemoveListener(Resize);
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag has been found but before it is valid to begin the drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public abstract bool SetStyle(Style style);

		/// <inheritdoc/>
		public abstract bool GetStyle(Style style);
		#endregion
	}
}