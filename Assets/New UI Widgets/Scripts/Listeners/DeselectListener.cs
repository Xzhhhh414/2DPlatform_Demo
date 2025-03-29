namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Deselect listener.
	/// </summary>
	[Obsolete("Replaced with SelectListener which have both select and deselect events.")]
	public class DeselectListener : MonoBehaviour, IDeselectHandler
	{
		/// <summary>
		/// Deselect event.
		/// </summary>
		public UnityEvent Deselect = new UnityEvent();

		/// <summary>
		/// Process deselect event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDeselect(BaseEventData eventData)
		{
			Deselect.Invoke();
		}
	}
}