namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Swipe.
	/// </summary>
	[RequireComponent(typeof(Graphic))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/mobile/swipe.html")]
	public class Swipe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		/// <summary>
		/// Direction.
		/// </summary>
		public readonly struct Direction
		{
			/// <summary>
			/// Left.
			/// </summary>
			public readonly bool Left;

			/// <summary>
			/// Right.
			/// </summary>
			public readonly bool Right;

			/// <summary>
			/// Top.
			/// </summary>
			public readonly bool Top;

			/// <summary>
			/// Bottom.
			/// </summary>
			public readonly bool Bottom;

			/// <summary>
			/// Initializes a new instance of the <see cref="Direction"/> struct.
			/// </summary>
			/// <param name="direction">Swipe direction.</param>
			/// <param name="distance">Minimum distance to be swiped.</param>
			public Direction(Vector2 direction, float distance)
			{
				Left = direction.x < -distance;
				Right = direction.x > distance;

				Top = direction.y > distance;
				Bottom = direction.y < -distance;
			}

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "Required.")]
			public override string ToString()
			{
				return string.Format(
					"Direction(Left = {0}; Right = {1}; Top = {2}; Bottom: {3})",
					Left.ToString(),
					Right.ToString(),
					Top.ToString(),
					Bottom.ToString());
			}
		}

		/// <summary>
		/// Swipe event.
		/// </summary>
		[Serializable]
		public class SwipeEvent : UnityEvent<Direction>
		{
		}

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// If dragged longer than the specified time then it is not swipe event.
		/// </summary>
		[SerializeField]
		[Tooltip("If dragged longer than the specified time then it is not swipe event.")]
		public float MaxTime = 0.5f;

		/// <summary>
		/// Minimum distance to be swiped.
		/// </summary>
		[SerializeField]
		[Tooltip("Minimum distance to be swiped.")]
		public float RequiredDistance = 100f;

		/// <summary>
		/// Minimum distance at X or Y axis to be swiped at those axes.
		/// </summary>
		[SerializeField]
		[Tooltip("Minimum distance at X or Y axis to be swiped at those axes.")]
		public float MinDistance = 50f;

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Swipe event.
		/// </summary>
		[SerializeField]
		public SwipeEvent OnSwipe = new SwipeEvent();

		bool isDrag;

		float start;

		Vector2 position;

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData)
		{
			return eventData.button == DragButton;
		}

		/// <summary>
		/// Processing the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			start = UtilitiesTime.GetTime(UnscaledTime);
			position = eventData.position;
			isDrag = true;
		}

		/// <summary>
		/// Processing the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!isDrag)
			{
				return;
			}

			if (!CanDrag(eventData))
			{
				OnEndDrag(eventData);
				return;
			}
		}

		/// <summary>
		/// Processing the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!isDrag)
			{
				return;
			}

			isDrag = false;

			var time = UtilitiesTime.GetTime(UnscaledTime);
			if ((time - start) > MaxTime)
			{
				return;
			}

			var distance = eventData.position - position;
			if (distance.magnitude < RequiredDistance)
			{
				return;
			}

			OnSwipe.Invoke(new Direction(distance, MinDistance));
		}
	}
}