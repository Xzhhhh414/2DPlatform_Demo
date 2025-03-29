namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Handle for the DragSupport.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/drag-and-drop.html")]
	public class DragSupportHandle : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
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

		/// <summary>
		/// OnInitializePotentialDrag event.
		/// </summary>
		[SerializeField]
		public PointerUnityEvent OnInitializePotentialDragEvent = new PointerUnityEvent();

		/// <summary>
		/// OnBeginDrag event.
		/// </summary>
		[SerializeField]
		public PointerUnityEvent OnBeginDragEvent = new PointerUnityEvent();

		/// <summary>
		/// OnDrag event.
		/// </summary>
		[SerializeField]
		public PointerUnityEvent OnDragEvent = new PointerUnityEvent();

		/// <summary>
		/// OnEndDrag event.
		/// </summary>
		[SerializeField]
		public PointerUnityEvent OnEndDragEvent = new PointerUnityEvent();

		/// <summary>
		/// Called by a BaseInputModule when a drag has been found but before it is valid to begin the drag.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			OnInitializePotentialDragEvent.Invoke(eventData);
		}

		/// <summary>
		/// Called by a BaseInputModule before a drag is started.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			OnBeginDragEvent.Invoke(eventData);
		}

		/// <summary>
		/// When dragging is occurring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			OnDragEvent.Invoke(eventData);
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			OnEndDragEvent.Invoke(eventData);
		}
	}
}