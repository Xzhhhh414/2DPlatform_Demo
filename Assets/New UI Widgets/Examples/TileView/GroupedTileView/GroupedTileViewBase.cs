namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// GroupedTileView
	/// </summary>
	public abstract class GroupedTileViewBase : ListViewCustom<GroupedTileViewComponentBase, Photo>
	{
		/// <summary>
		/// Grouped data.
		/// </summary>
		public GroupedList<Photo> GroupedData = new GroupedList<Photo>((groups, item) =>
		{
			var date = item.Created.Date;

			foreach (var group in groups)
			{
				if (group.Created == date)
				{
					return group;
				}
			}

			return new Photo() { Created = item.Created.Date, IsGroup = true };
		});
	}
}