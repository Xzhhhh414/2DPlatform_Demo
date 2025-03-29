namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Redirect drag events from current gameobject to specified.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/drag-redirect.html")]
	public class DragRedirect : UIBehaviour, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
	{
		/// <summary>
		/// Drag events will be redirected to this gameobject.
		/// </summary>
		[SerializeField]
		public GameObject RedirectTo;

		/// <summary>
		/// Mark drag event as used.
		/// </summary>
		[SerializeField]
		[Tooltip("Mark drag event as used.")]
		public bool MarkAsUsed = true;

		/// <summary>
		/// Minimal distance from start position to allow event redirect.
		/// </summary>
		[SerializeField]
		public Vector2 MinDistance = Vector2.zero;

		/// <summary>
		/// Start position.
		/// </summary>
		protected Vector2 StartPosition;

		/// <summary>
		/// Is valid distance?
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if distance exceeds min distance; otherwise false.</returns>
		protected virtual bool IsValidDistance(PointerEventData eventData)
		{
			var delta = eventData.position - StartPosition;
			return (Mathf.Abs(delta.x) >= MinDistance.x) && (Mathf.Abs(delta.y) >= MinDistance.y);
		}

		/// <summary>
		/// Gets the handlers.
		/// </summary>
		/// <typeparam name="T">The handler type.</typeparam>
		/// <param name="components">The handlers.</param>
		protected void GetHandlers<T>(List<T> components)
			where T : class
		{
			RedirectTo.GetComponents(components);
		}

		/// <summary>
		/// Called by a BaseInputModule before a drag is started.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			StartPosition = eventData.position;

			using var _ = ListPool<IBeginDragHandler>.Get(out var temp);
			GetHandlers(temp);
			foreach (var handler in temp)
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnBeginDrag(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag has been found but before it is valid to begin the drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			using var _ = ListPool<IInitializePotentialDragHandler>.Get(out var temp);
			GetHandlers(temp);
			foreach (var handler in temp)
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnInitializePotentialDrag(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}

		/// <summary>
		/// When dragging is occurring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!IsValidDistance(eventData))
			{
				return;
			}

			using var _ = ListPool<IDragHandler>.Get(out var temp);
			GetHandlers(temp);
			foreach (var handler in temp)
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnDrag(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!IsValidDistance(eventData))
			{
				return;
			}

			using var _ = ListPool<IEndDragHandler>.Get(out var temp);
			GetHandlers(temp);
			foreach (var handler in temp)
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnEndDrag(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnScroll event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnScroll(PointerEventData eventData)
		{
			using var _ = ListPool<IScrollHandler>.Get(out var temp);
			GetHandlers(temp);
			foreach (var handler in temp)
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnScroll(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}
	}
}