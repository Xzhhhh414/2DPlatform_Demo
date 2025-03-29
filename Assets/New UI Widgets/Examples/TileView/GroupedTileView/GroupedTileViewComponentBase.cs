namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// GroupedListViewComponent.
	/// </summary>
	public abstract class GroupedTileViewComponentBase : ListViewItem, IViewData<Photo>
	{
		/// <summary>
		/// Displayed item.
		/// </summary>
		protected Photo Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public abstract void SetData(Photo item);

		/// <inheritdoc/>
		public override void GraphicsColoring(Color foregroundColor, Color backgroundColor, float fadeDuration = 0f)
		{
			// disable coloring
		}
	}
}