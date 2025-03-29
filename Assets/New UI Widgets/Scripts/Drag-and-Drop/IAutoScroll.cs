namespace UIWidgets
{
	using System;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Auto scroll interface.
	/// Used by drag script to automatically scroll containers.
	/// </summary>
	public interface IAutoScroll
	{
		/// <summary>
		/// Start scroll when pointer is near ScrollRect border.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="dragCallback">Callback.</param>
		/// <returns>true if auto scroll started; otherwise false.</returns>
		bool AutoScrollStart(PointerEventData eventData, Action<PointerEventData> dragCallback);

		/// <summary>
		/// Stop auto scroll.
		/// </summary>
		void AutoScrollStop();
	}
}