namespace UIWidgets.Examples.VHAR
{
	using UIWidgets;

	/// <summary>
	/// ItemView component.
	/// </summary>
	public abstract class ItemView : ListViewItem, IViewData<Item>
	{
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public abstract void SetData(Item item);
	}
}