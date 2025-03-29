namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;

	/// <summary>
	/// Modifies the drag event to work in only one direction.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/interactions/drag-one-direction.html")]
	public class DragOneDirection : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		Vector2 startPosition = Vector2.zero;

		bool isDrag;

		bool isHorizontal;

		/// <summary>
		/// Minimal drag distance .
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("DeadZone")]
		public float MinDistance = 20f;

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

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
		/// Process begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			startPosition = eventData.position;
		}

		/// <summary>
		/// Process drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				OnEndDrag(eventData);
				return;
			}

			var current = eventData.position;

			if (isDrag)
			{
				eventData.position = isHorizontal
					? new Vector2(current.x, startPosition.y)
					: new Vector2(startPosition.x, current.y);
			}
			else
			{
				var delta = current - startPosition;
				isDrag = delta.magnitude > MinDistance;
				isHorizontal = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);

				eventData.position = startPosition;
			}
		}

		/// <summary>
		/// Process end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			isDrag = false;
		}

		/// <summary>
		/// Process initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
		}
	}
}