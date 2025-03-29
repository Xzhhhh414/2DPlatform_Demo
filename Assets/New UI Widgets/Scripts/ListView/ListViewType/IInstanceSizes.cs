namespace UIWidgets.Internal
{
	using UnityEngine;

	/// <summary>
	/// Interface for the container to save instance sizes of items for ListView.
	/// </summary>
	/// <typeparam name="TItem">Item type.</typeparam>
	public interface IInstanceSizes<TItem>
	{
		/// <summary>
		/// Count.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Get and set instance size.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Size.</returns>
		Vector2 this[TItem item]
		{
			get;
			set;
		}

		/// <summary>
		/// Check if container has size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if container has size of the specified item; otherwise false.</returns>
		bool Contains(TItem item);

		/// <summary>
		/// Try to get size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="size">Instance size.</param>
		/// <returns>true if container has size of the specified item; otherwise false.</returns>
		bool TryGet(TItem item, out Vector2 size);

		/// <summary>
		/// Remove size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if container has size; otherwise false.</returns>
		bool Remove(TItem item);

		/// <summary>
		/// Remove sizes of items that are not in the specified list.
		/// </summary>
		/// <param name="items">Items.</param>
		void RemoveNotExisting(ObservableList<TItem> items);

		/// <summary>
		/// Get items count that fits into specified area.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="horizontal">If true do calculation using items width; otherwise do calculation using items height.</param>
		/// <param name="visibleArea">Size of the visible area.</param>
		/// <param name="spacing">Spacing between items.</param>
		/// <returns>Maximum items count.</returns>
		int Visible(ObservableList<TItem> items, bool horizontal, float visibleArea, float spacing);

		/// <summary>
		/// Check if has overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if has size of the specified item; otherwise false.</returns>
		bool HasOverridden(TItem item);

		/// <summary>
		/// Set overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="size">Size.</param>
		void SetOverridden(TItem item, Vector2 size);

		/// <summary>
		/// Remove overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if container has overridden size; otherwise false.</returns>
		bool RemoveOverridden(TItem item);

		/// <summary>
		/// Try to get overridden size of the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="size">Instance size.</param>
		/// <returns>true if container has overridden size of the specified item; otherwise false.</returns>
		bool TryGetOverridden(TItem item, out Vector2 size);

		/// <summary>
		/// Get actual item size.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="defaultSize">Default size if any other size not specified.</param>
		/// <returns>Size.</returns>
		Vector2 Get(TItem item, Vector2 defaultSize);

		/// <summary>
		/// Maximum size for each dimenstion.
		/// </summary>
		/// <param name="defaultSize">Default size.</param>
		/// <returns>Size.</returns>
		Vector2 MaxSize(Vector2 defaultSize);
	}
}