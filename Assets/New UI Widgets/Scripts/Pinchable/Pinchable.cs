namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Pinchable component.
	/// </summary>
	[RequireComponent(typeof(Draggable))]
	[RequireComponent(typeof(Rotatable))]
	[RequireComponent(typeof(Resizable))]
	[AddComponentMenu("UI/New UI Widgets/Interactions/Pinchable")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/pinchable.html")]
	public class Pinchable : UIBehaviourInteractable, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		/// <summary>
		/// Touches data.
		/// </summary>
		protected struct Touches : IEquatable<Touches>
		{
			/// <summary>
			/// First touch.
			/// </summary>
			public Vector2 Point1;

			/// <summary>
			/// Second touch.
			/// </summary>
			public Vector2 Point2;

			/// <summary>
			/// Angle between points.
			/// </summary>
			public readonly float Angle => Rotatable.Point2Angle(Point1 - Point2);

			/// <summary>
			/// Initializes a new instance of the <see cref="Touches"/> struct.
			/// </summary>
			/// <param name="point1">First point.</param>
			/// <param name="point2">Second point.</param>
			public Touches(Vector2 point1, Vector2 point2)
			{
				Point1 = point1;
				Point2 = point2;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly override bool Equals(object obj) => (obj is Touches touches) && Equals(touches);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly bool Equals(Touches other) => Point1 == other.Point1 && Point2 == other.Point2;

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public readonly override int GetHashCode() => Point1.GetHashCode() ^ Point2.GetHashCode();

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(Touches left, Touches right) => left.Equals(right);

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(Touches left, Touches right) => !left.Equals(right);
		}

		Draggable draggable;

		/// <summary>
		/// Draggable component.
		/// </summary>
		protected Draggable Draggable
		{
			get
			{
				if (draggable == null)
				{
					TryGetComponent(out draggable);
				}

				return draggable;
			}
		}

		Resizable resizable;

		/// <summary>
		/// Resizable component.
		/// </summary>
		protected Resizable Resizable
		{
			get
			{
				if (resizable == null)
				{
					TryGetComponent(out resizable);
				}

				return resizable;
			}
		}

		Rotatable rotatable;

		/// <summary>
		/// Rotatable component.
		/// </summary>
		protected Rotatable Rotatable
		{
			get
			{
				if (rotatable == null)
				{
					TryGetComponent(out rotatable);
				}

				return rotatable;
			}
		}

		/// <summary>
		/// Target.
		/// </summary>
		protected RectTransform Target => Draggable.Target;

		/// <summary>
		/// Is dragging?
		/// </summary>
		[NonSerialized]
		protected bool IsDrag;

		/// <summary>
		/// Is multi-touch?
		/// </summary>
		[NonSerialized]
		protected bool IsMultitouch;

		/// <summary>
		/// Original touches.
		/// </summary>
		[NonSerialized]
		protected Touches OriginalTouches;

		/// <summary>
		/// Previous touches.
		/// </summary>
		[NonSerialized]
		protected Touches PrevTouches;

		/// <summary>
		/// Press event camera.
		/// </summary>
		[NonSerialized]
		protected Camera PressEventCamera;

		/// <summary>
		/// Raw touches data.
		/// </summary>
		[NonSerialized]
		protected List<Touch> TouchesData = new List<Touch>();

		/// <summary>
		/// Allow drag.
		/// </summary>
		[SerializeField]
		public bool AllowDrag = true;

		/// <summary>
		/// Allow resize.
		/// </summary>
		[SerializeField]
		public bool AllowResize = true;

		/// <summary>
		/// Allow rotate.
		/// </summary>
		[SerializeField]
		public bool AllowRotate = true;

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Start pinch event.
		/// </summary>
		[SerializeField]
		public PinchableEvent OnStartPinch = new PinchableEvent();

		/// <summary>
		/// During pinch event.
		/// </summary>
		[SerializeField]
		public PinchableEvent OnPinch = new PinchableEvent();

		/// <summary>
		/// End pinch event.
		/// </summary>
		[SerializeField]
		public PinchableEvent OnEndPinch = new PinchableEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Draggable.Interactable = false;
			Resizable.Interactable = false;
			Rotatable.Interactable = false;
		}

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
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			Draggable.InitDrag();

			IsDrag = true;
			PressEventCamera = eventData.pressEventCamera;

			OnStartPinch.Invoke(this);
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!IsDrag)
			{
				return;
			}

			if (!CanDrag(eventData))
			{
				OnEndDrag(eventData);
				return;
			}

			using var _ = ListPool<Touch>.Get(out var temp);
			var touches = CompatibilityInput.TouchCount;
			for (int i = 0; i < touches; i++)
			{
				temp.Add(CompatibilityInput.GetTouch(i));
			}

			ProcessPinch(eventData, temp);

			OnPinch.Invoke(this);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!IsDrag)
			{
				return;
			}

			IsDrag = false;
			IsMultitouch = false;

			OnEndPinch.Invoke(this);
		}

		/// <summary>
		/// Convert raw touches data.
		/// </summary>
		/// <param name="rawTouches">Raw touches.</param>
		/// <returns>Touches.</returns>
		protected virtual Touches ConvertTouches(List<Touch> rawTouches)
		{
			var rotation = Target.localRotation;
			Target.localRotation = Quaternion.Euler(Vector3.zero);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, rawTouches[0].position, PressEventCamera, out var point1);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, rawTouches[1].position, PressEventCamera, out var point2);
			Target.localRotation = rotation;

			return new Touches(point1, point2);
		}

		/// <summary>
		/// Process pinch.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="rawTouches">Raw touches.</param>
		protected virtual void ProcessPinch(PointerEventData eventData, List<Touch> rawTouches)
		{
			if (rawTouches.Count >= 2)
			{
				if (!IsMultitouch)
				{
					InitMultitouch(rawTouches);
				}
				else
				{
					ProcessMultitouch(rawTouches);
				}
			}
			else
			{
				if (IsMultitouch)
				{
					IsMultitouch = false;
				}
			}

			ProcessDrag(eventData);
		}

		/// <summary>
		/// Init multi-touch.
		/// </summary>
		/// <param name="rawTouches">Raw touches.</param>
		protected virtual void InitMultitouch(List<Touch> rawTouches)
		{
			IsMultitouch = true;
			OriginalTouches = ConvertTouches(rawTouches);
			PrevTouches = OriginalTouches;

			Rotatable.InitRotate(Target.localEulerAngles.z);
		}

		/// <summary>
		/// Process multi-touch.
		/// </summary>
		/// <param name="rawTouches">Raw touches.</param>
		protected virtual void ProcessMultitouch(List<Touch> rawTouches)
		{
			var touches = ConvertTouches(rawTouches);

			if (AllowResize)
			{
				var delta = touches.Point1 - touches.Point2;
				var point1_right = delta.x >= 0f;
				var point1_top = delta.y >= 0f;
				var point1_regions = new Resizable.Regions { Top = point1_top, Bottom = !point1_top, Left = !point1_right, Right = point1_right };
				var point2_regions = new Resizable.Regions { Top = !point1_top, Bottom = point1_top, Left = point1_right, Right = !point1_right };
				var delta1 = touches.Point1 - PrevTouches.Point1;
				var delta2 = touches.Point2 - PrevTouches.Point2;

				Resizable.InitResize();
				Resizable.Resize(point1_regions, delta1);

				Resizable.InitResize();
				Resizable.Resize(point2_regions, delta2);
			}

			if (AllowRotate)
			{
				Rotatable.Rotate(touches.Angle - OriginalTouches.Angle);
			}

			PrevTouches = ConvertTouches(rawTouches);
		}

		/// <summary>
		/// Process drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void ProcessDrag(PointerEventData eventData)
		{
			if (!AllowDrag)
			{
				return;
			}

			if (IsMultitouch)
			{
				return;
			}

			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.position, eventData.pressEventCamera, out var current_position);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.pressPosition, eventData.pressEventCamera, out var original_position);

			var delta = current_position - original_position;
			Draggable.Drag(delta);
		}
	}
}