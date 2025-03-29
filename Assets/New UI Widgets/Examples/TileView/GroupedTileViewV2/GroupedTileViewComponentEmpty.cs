namespace UIWidgets.Examples
{
	/// <summary>
	/// Empty.
	/// </summary>
	public class GroupedTileViewComponentEmpty : GroupedTileViewComponentBase
	{
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(Photo item)
		{
			Item = item;

			if (Item.IsGroup)
			{
				gameObject.SetActive(false);
			}
		}
	}
}