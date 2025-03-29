namespace UIWidgets
{
	using System;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for the AutoScroll.
	/// </summary>
	/// <typeparam name="TOwner">Type of owner.</typeparam>
	public abstract class AutoScrollBase<TOwner> : IAutoScroll
		where TOwner : MonoBehaviour
	{
		/// <summary>
		/// Area.
		/// </summary>
		public abstract float Area
		{
			get;
		}

		/// <summary>
		/// Speed.
		/// </summary>
		public abstract float Speed
		{
			get;
		}

		/// <summary>
		/// Is scrolling?
		/// </summary>
		public bool Scrolling => Coroutine != null;

		/// <summary>
		/// Scroll coroutine.
		/// </summary>
		protected Coroutine Coroutine;

		/// <summary>
		/// Auto scroll direction.
		/// </summary>
		protected int Direction;

		/// <summary>
		/// Auto scroll EventData for callback.
		/// </summary>
		protected PointerEventData EventData;

		/// <summary>
		/// Auto scroll callback.
		/// </summary>
		protected Action<PointerEventData> DragCallback;

		/// <summary>
		/// Owner.
		/// </summary>
		protected TOwner Owner;

		/// <summary>
		/// Target.
		/// </summary>
		protected RectTransform Target;

		/// <summary>
		/// Initializes a new instance of the <see cref="AutoScrollBase{TOwner}"/> class.
		/// </summary>
		/// <param name="owner">Owner.</param>
		/// <param name="target">Target.</param>
		public AutoScrollBase(TOwner owner, RectTransform target)
		{
			Owner = owner;
			Target = target;
		}

		/// <summary>
		/// Start scroll when pointer is near ScrollRect border.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="dragCallback">Callback.</param>
		/// <returns>true if auto scroll started; otherwise false.</returns>
		public virtual bool AutoScrollStart(PointerEventData eventData, Action<PointerEventData> dragCallback)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.position, eventData.pressEventCamera, out var point);

			var rect_start = Target.rect;
			var rect_end = Target.rect;

			if (IsHorizontal())
			{
				rect_start.width = Area;

				rect_end.position = new Vector2(rect_end.position.x + rect_end.width - Area, rect_end.position.y);
				rect_end.width = Area;
			}
			else
			{
				rect_start.position = new Vector2(rect_start.position.x, rect_start.position.y + rect_start.height - Area);
				rect_start.height = Area;

				rect_end.height = Area;
			}

			var new_direction = 0;
			if (rect_start.Contains(point))
			{
				new_direction = -1;
			}
			else if (rect_end.Contains(point))
			{
				new_direction = +1;
			}

			if (new_direction != Direction)
			{
				AutoScrollStop();

				if (new_direction != 0)
				{
					Direction = new_direction;
					EventData = eventData;
					DragCallback = dragCallback;
					Coroutine = Owner.StartCoroutine(Scroll());
				}
			}

			return Direction != 0;
		}

		/// <summary>
		/// Stop scroll.
		/// </summary>
		public void AutoScrollStop()
		{
			if (Coroutine == null)
			{
				return;
			}

			Direction = 0;

			if (Coroutine != null)
			{
				Owner.StopCoroutine(Coroutine);
			}

			Coroutine = null;
			DragCallback = null;
			EventData = null;
		}

		/// <summary>
		/// Auto scroll.
		/// </summary>
		/// <returns>Coroutine.</returns>
		protected abstract IEnumerator Scroll();

		/// <summary>
		/// Is scroll direction horizontal?
		/// </summary>
		/// <returns>true if scroll direction is horizontal; otherwise false.</returns>
		protected abstract bool IsHorizontal();
	}
}