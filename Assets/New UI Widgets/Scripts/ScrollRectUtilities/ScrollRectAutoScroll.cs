namespace UIWidgets
{
	using System;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Allows scrolling content during drag&amp;drop when the pointer is in less than a specified distance from the border
	/// </summary>
	[RequireComponent(typeof(ScrollRect))]
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/scrollrect/scrollrect-autoscroll.html")]
	public class ScrollRectAutoScroll : UIBehaviourInitiable, IAutoScroll
	{
		/// <summary>
		/// Area.
		/// </summary>
		[SerializeField]
		[Tooltip("Maximum distance from border to scroll content.")]
		public float Area = 40f;

		/// <summary>
		/// Speed.
		/// </summary>
		[SerializeField]
		public float Speed = 200f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Scroll coroutine.
		/// </summary>
		protected Coroutine Coroutine;

		/// <summary>
		/// Auto scroll direction.
		/// </summary>
		protected Vector2 ScrollDirection;

		/// <summary>
		/// Auto scroll EventData for callback.
		/// </summary>
		protected PointerEventData EventData;

		/// <summary>
		/// Auto scroll callback.
		/// </summary>
		protected Action<PointerEventData> DragCallback;

		/// <summary>
		/// Target.
		/// </summary>
		protected RectTransform Target;

		/// <summary>
		/// ScrollRect.
		/// </summary>
		protected ScrollRect ScrollRect;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TryGetComponent(out ScrollRect);
			TryGetComponent(out Target);
		}

		/// <summary>
		/// Get direction.
		/// </summary>
		/// <param name="start">Scroll back if point in this rect.</param>
		/// <param name="end">Scroll forward if point in this rect.</param>
		/// <param name="point">Point.</param>
		/// <returns>Direction.</returns>
		protected virtual int Direction(Rect start, Rect end, Vector2 point)
		{
			if (start.Contains(point))
			{
				return -1;
			}

			if (end.Contains(point))
			{
				return +1;
			}

			return 0;
		}

		/// <summary>
		/// Get horizontal scroll direction.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Horizontal scroll direction.</returns>
		protected virtual int HorizontalDirection(Vector2 point)
		{
			if (!ScrollRect.horizontal)
			{
				return 0;
			}

			var rect_start = Target.rect;
			rect_start.width = Area;

			var rect_end = Target.rect;
			rect_end.position = new Vector2(rect_end.position.x + rect_end.width - Area, rect_end.position.y);
			rect_end.width = Area;

			return -Direction(rect_start, rect_end, point);
		}

		/// <summary>
		/// Get vertical scroll direction.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Vertical scroll direction.</returns>
		protected virtual int VerticalDirection(Vector2 point)
		{
			if (!ScrollRect.vertical)
			{
				return 0;
			}

			var rect_start = Target.rect;
			rect_start.position = new Vector2(rect_start.position.x, rect_start.position.y + rect_start.height - Area);
			rect_start.height = Area;

			var rect_end = Target.rect;
			rect_end.height = Area;

			return Direction(rect_start, rect_end, point);
		}

		/// <summary>
		/// Start scroll when pointer is near ScrollRect border.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="dragCallback">Drag callback.</param>
		/// <returns>true if auto scroll started; otherwise false.</returns>
		public virtual bool AutoScrollStart(PointerEventData eventData, Action<PointerEventData> dragCallback)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.position, eventData.pressEventCamera, out var point);

			var new_direction = new Vector2(
				HorizontalDirection(point),
				VerticalDirection(point));

			if (new_direction != ScrollDirection)
			{
				AutoScrollStop();

				if (new_direction != Vector2.zero)
				{
					ScrollDirection = new_direction;
					EventData = eventData;
					DragCallback = dragCallback;
					Coroutine = StartCoroutine(Scroll());
				}
			}

			return ScrollDirection != Vector2.zero;
		}

		/// <summary>
		/// Stop scroll.
		/// </summary>
		public virtual void AutoScrollStop()
		{
			if (Coroutine == null)
			{
				return;
			}

			ScrollDirection = Vector2.zero;

			if (Coroutine != null)
			{
				StopCoroutine(Coroutine);
			}

			Coroutine = null;
			DragCallback = null;
			EventData = null;
		}

		/// <summary>
		/// Auto scroll.
		/// </summary>
		/// <returns>Coroutine.</returns>
		protected virtual IEnumerator Scroll()
		{
			var min = Vector2.zero;
			var max = new Vector2(
				ScrollRect.content.rect.width - ScrollRect.viewport.rect.width,
				ScrollRect.content.rect.height - ScrollRect.viewport.rect.height);

			while (true)
			{
				var delta = Speed * UtilitiesTime.GetDeltaTime(UnscaledTime) * ScrollDirection;

				var pos = ScrollRect.content.anchoredPosition + delta;
				pos.x = Mathf.Clamp(pos.x, -max.x, min.x);
				pos.y = Mathf.Clamp(pos.y, min.y, max.y);

				ScrollRect.content.anchoredPosition = pos;
				yield return null;

				DragCallback?.Invoke(EventData);
			}
		}
	}
}