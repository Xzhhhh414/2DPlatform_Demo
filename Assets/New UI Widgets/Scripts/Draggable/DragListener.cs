namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drag events listener.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class DragListener : DragSupportHandle, IScrollHandler, ISelectHandler, IDeselectHandler
	{
		/// <summary>
		/// OnDragStart event.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with OnBeginDragEvent.")]
		public PointerUnityEvent OnDragStartEvent = new PointerUnityEvent();

		/// <summary>
		/// OnDragEnd event.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with OnEndDragEvent.")]
		public PointerUnityEvent OnDragEndEvent = new PointerUnityEvent();

		/// <summary>
		/// OnScroll event.
		/// </summary>
		[SerializeField]
		public PointerUnityEvent OnScrollEvent = new PointerUnityEvent();

		/// <summary>
		/// OnSelect event.
		/// </summary>
		[SerializeField]
		public SelectEvent OnSelectEvent = new SelectEvent();

		/// <summary>
		/// OnDeselect event.
		/// </summary>
		[SerializeField]
		public SelectEvent OnDeselectEvent = new SelectEvent();

		/// <summary>
		/// Called by a BaseInputModule before a drag is started.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			#pragma warning disable 0618
			OnDragStartEvent.Invoke(eventData);
			#pragma warning restore
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			#pragma warning disable 0618
			OnDragEndEvent.Invoke(eventData);
			#pragma warning restore
		}

		/// <summary>
		/// Process scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnScroll(PointerEventData eventData)
		{
			OnScrollEvent.Invoke(eventData);
		}

		/// <summary>
		/// Process select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSelect(BaseEventData eventData)
		{
			OnSelectEvent.Invoke(eventData);
		}

		/// <summary>
		/// Process deselect event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDeselect(BaseEventData eventData)
		{
			OnDeselectEvent.Invoke(eventData);
		}
	}
}