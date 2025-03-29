namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Test draggable target.
	/// </summary>
	public class ToggleTarget : MonoBehaviour
	{
		/// <summary>
		/// Target.
		/// </summary>
		[SerializeField]
		protected DraggableResizableRotatable Target;

		/// <summary>
		/// Selectable.
		/// </summary>
		[SerializeField]
		protected List<Selectable> Selectables = new List<Selectable>();

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			Target.Init();
			Target.gameObject.SetActive(false);

			foreach (var s in Selectables)
			{
				if (s == null)
				{
					return;
				}

				var listener = Utilities.RequireComponent<SelectListener>(s);
				listener.onSelect.AddListener(SetTarget);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDestroy()
		{
			foreach (var s in Selectables)
			{
				if (s == null)
				{
					return;
				}

				if (s.TryGetComponent<SelectListener>(out var listener))
				{
					listener.onSelect.RemoveListener(SetTarget);
				}
			}
		}

		/// <summary>
		/// Set target.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void SetTarget(BaseEventData eventData)
		{
			Target.SetTarget(eventData.selectedObject.transform as RectTransform);
		}
	}
}